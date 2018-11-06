using SevenTiny.Bantina.Aop.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SevenTiny.Bantina.Aop
{
    public class DynamicProxy
    {
        private static readonly string[] _ignoreMethodName = new[] { "GetType", "ToString", "GetHashCode", "Equals" };

        public static TInterface CreateProxyOfRealize<TInterface, TImp>() where TImp : class, new() where TInterface : class
        {
            return Invoke<TInterface, TImp>();
        }

        public static TProxyClass CreateProxyOfInherit<TProxyClass>() where TProxyClass : class, new()
        {
            return Invoke<TProxyClass, TProxyClass>(true);
        }

        private static TInterface Invoke<TInterface, TImp>(bool inheritMode = false, Type interceptorType = null) where TImp : class where TInterface : class
        {
            if (inheritMode)
                return CreateProxyOfInherit(typeof(TImp), interceptorType) as TInterface;
            else
                return CreateProxyOfRealize(typeof(TInterface), typeof(TImp), interceptorType) as TInterface;
        }

        public static object CreateProxyOfRealize(Type interfaceType, Type impType, Type interceptorType = null)
        {
            string nameOfAssembly = impType.Name + "ProxyAssembly";
            string nameOfModule = impType.Name + "ProxyModule";
            string nameOfType = impType.Name + "Proxy";

            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(nameOfAssembly), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assembly.DefineDynamicModule(nameOfModule);
            TypeBuilder typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, null, new[] { interfaceType });
            MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

            return Invoke(impType, typeBuilder, methodAttributes, interceptorType);
        }

        public static object CreateProxyOfInherit(Type impType, Type interceptorType = null)
        {
            string nameOfAssembly = impType.Name + "ProxyAssembly";
            string nameOfModule = impType.Name + "ProxyModule";
            string nameOfType = impType.Name + "Proxy";

            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(nameOfAssembly), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assembly.DefineDynamicModule(nameOfModule);
            TypeBuilder typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, impType);
            MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual;

            return Invoke(impType, typeBuilder, methodAttributes, interceptorType);
        }

        private static object Invoke(Type impType, TypeBuilder typeBuilder, MethodAttributes methodAttributes, Type interceptorType = null)
        {
            Type interceptorAttributeType = impType.GetCustomAttribute(typeof(InterceptorBaseAttribute))?.GetType() ?? interceptorType;

            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            var ilOfCtor = constructorBuilder.GetILGenerator();
            // ---- define fields ----
            FieldBuilder fieldInterceptor = null;
            if (interceptorAttributeType != null)
            {
                fieldInterceptor = typeBuilder.DefineField("_interceptor", interceptorAttributeType, FieldAttributes.Private);
                ilOfCtor.Emit(OpCodes.Ldarg_0);
                ilOfCtor.Emit(OpCodes.Newobj, interceptorAttributeType.GetConstructor(new Type[0]));
                ilOfCtor.Emit(OpCodes.Stfld, fieldInterceptor);
            }

            //initial field of impObj
            FieldBuilder _serviceImpObj = typeBuilder.DefineField("_serviceImpObj", impType, FieldAttributes.Private);
            ilOfCtor.Emit(OpCodes.Ldarg_0);
            ilOfCtor.Emit(OpCodes.Newobj, impType.GetConstructor(new Type[0]));
            ilOfCtor.Emit(OpCodes.Stfld, _serviceImpObj);

            ilOfCtor.Emit(OpCodes.Ret);

            // ---- define methods ----

            var methodsOfType = impType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methodsOfType)
            {
                //ignore method
                if (_ignoreMethodName.Contains(method.Name))
                    continue;

                var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

                var methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes, CallingConventions.Standard, method.ReturnType, methodParameterTypes);

                var ilMethod = methodBuilder.GetILGenerator();

                // set local field
                var methodName = ilMethod.DeclareLocal(typeof(string));     //instance of method name
                var parameters = ilMethod.DeclareLocal(typeof(object[]));   //instance of parameters
                var result = ilMethod.DeclareLocal(typeof(object));         //instance of result

                Dictionary<Type, LocalBuilder> actionTypeBuilders = new Dictionary<Type, LocalBuilder>();

                //attribute init
                if (method.GetCustomAttributes<ActionBaseAttribute>().Any() || impType.GetCustomAttributes<ActionBaseAttribute>().Any())
                {
                    //method can override class attrubute
                    if (method.GetCustomAttributes<ActionBaseAttribute>().Any())
                    {
                        foreach (var item in method.GetCustomAttributes<ActionBaseAttribute>().ToDictionary(k => k.GetType(), v => default(LocalBuilder)))
                        {
                            actionTypeBuilders.AddOrUpdate(item.Key, item.Value);
                        }
                    }
                    else if (impType.GetCustomAttributes<ActionBaseAttribute>().Any())
                    {
                        foreach (var item in impType.GetCustomAttributes<ActionBaseAttribute>().ToDictionary(k => k.GetType(), v => default(LocalBuilder)))
                        {
                            actionTypeBuilders.AddOrUpdate(item.Key, item.Value);
                        }
                    }

                    foreach (var item in actionTypeBuilders.Select(t => t.Key).ToArray())
                    {
                        var actionAttributeObj = ilMethod.DeclareLocal(item);
                        ilMethod.Emit(OpCodes.Newobj, item.GetConstructor(new Type[0]));
                        ilMethod.Emit(OpCodes.Stloc, actionAttributeObj);
                        actionTypeBuilders[item] = actionAttributeObj;
                    }
                }

                //if no attribute
                if (fieldInterceptor != null || actionTypeBuilders.Any())
                {
                    ilMethod.Emit(OpCodes.Ldstr, method.Name);
                    ilMethod.Emit(OpCodes.Stloc, methodName);

                    ilMethod.Emit(OpCodes.Ldc_I4, methodParameterTypes.Length);
                    ilMethod.Emit(OpCodes.Newarr, typeof(object));
                    ilMethod.Emit(OpCodes.Stloc, parameters);

                    // build the method parameters
                    for (var j = 0; j < methodParameterTypes.Length; j++)
                    {
                        ilMethod.Emit(OpCodes.Ldloc, parameters);
                        ilMethod.Emit(OpCodes.Ldc_I4, j);
                        ilMethod.Emit(OpCodes.Ldarg, j + 1);
                        //box
                        ilMethod.Emit(OpCodes.Box, methodParameterTypes[j]);
                        ilMethod.Emit(OpCodes.Stelem_Ref);
                    }
                }

                //dynamic proxy action before
                if (actionTypeBuilders.Any())
                {
                    //load arguments
                    foreach (var item in actionTypeBuilders)
                    {
                        ilMethod.Emit(OpCodes.Ldloc, item.Value);
                        ilMethod.Emit(OpCodes.Ldloc, methodName);
                        ilMethod.Emit(OpCodes.Ldloc, parameters);
                        ilMethod.Emit(OpCodes.Call, item.Key.GetMethod("Before"));
                    }
                }

                if (interceptorAttributeType != null)
                {
                    //load arguments
                    ilMethod.Emit(OpCodes.Ldarg_0);//this
                    ilMethod.Emit(OpCodes.Ldfld, fieldInterceptor);
                    ilMethod.Emit(OpCodes.Ldarg_0);//this
                    ilMethod.Emit(OpCodes.Ldfld, _serviceImpObj);
                    ilMethod.Emit(OpCodes.Ldloc, methodName);
                    ilMethod.Emit(OpCodes.Ldloc, parameters);
                    // call Invoke() method of Interceptor
                    ilMethod.Emit(OpCodes.Callvirt, interceptorAttributeType.GetMethod("Invoke"));
                }
                else
                {
                    //direct call method
                    if (method.ReturnType == typeof(void) && !actionTypeBuilders.Any())
                    {
                        ilMethod.Emit(OpCodes.Ldnull);
                    }

                    ilMethod.Emit(OpCodes.Ldarg_0);//this
                    ilMethod.Emit(OpCodes.Ldfld, _serviceImpObj);
                    for (var j = 0; j < methodParameterTypes.Length; j++)
                    {
                        ilMethod.Emit(OpCodes.Ldarg, j + 1);
                    }
                    ilMethod.Emit(OpCodes.Callvirt, impType.GetMethod(method.Name));
                    //box
                    if (actionTypeBuilders.Any())
                    {
                        if (method.ReturnType != typeof(void))
                            ilMethod.Emit(OpCodes.Box, method.ReturnType);
                        else
                            ilMethod.Emit(OpCodes.Ldnull);
                    }
                }

                //dynamic proxy action after
                if (actionTypeBuilders.Any())
                {
                    ilMethod.Emit(OpCodes.Stloc, result);

                    //1->2 before and 2->1 after
                    foreach (var item in actionTypeBuilders.Reverse())
                    {
                        ilMethod.Emit(OpCodes.Ldloc, item.Value);
                        ilMethod.Emit(OpCodes.Ldloc, methodName);
                        ilMethod.Emit(OpCodes.Ldloc, result);
                        ilMethod.Emit(OpCodes.Callvirt, item.Key.GetMethod("After"));

                        //if no void return,set result
                        if (method.ReturnType == typeof(void))
                            ilMethod.Emit(OpCodes.Pop);
                        else
                            ilMethod.Emit(OpCodes.Stloc, result);
                    }
                }

                // pop the stack if return void
                if (method.ReturnType == typeof(void))
                {
                    //if no action attribute，void method need pop(action attribute method has done before)
                    if (!actionTypeBuilders.Any())
                        ilMethod.Emit(OpCodes.Pop);
                }
                else
                {
                    //unbox,if direct invoke,no box
                    if (fieldInterceptor != null || actionTypeBuilders.Any())
                    {
                        ilMethod.Emit(OpCodes.Ldloc, result);

                        if (method.ReturnType.IsValueType)
                            ilMethod.Emit(OpCodes.Unbox_Any, method.ReturnType);
                        else
                            ilMethod.Emit(OpCodes.Castclass, method.ReturnType);
                    }
                }
                // complete
                ilMethod.Emit(OpCodes.Ret);
            }

            var typeInfo = typeBuilder.CreateTypeInfo();

            return Activator.CreateInstance(typeInfo);
        }
    }
}