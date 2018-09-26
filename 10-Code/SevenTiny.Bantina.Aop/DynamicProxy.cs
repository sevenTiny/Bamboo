using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SevenTiny.Bantina.Aop
{
    public class DynamicProxy
    {
        public static TInterface CreateProxyOfRealize<TInterface, TImp>() where TImp : class, new() where TInterface : class
        {
            return Invoke<TInterface, TImp>();
        }

        public static TProxyClass CreateProxyOfInherit<TProxyClass>() where TProxyClass : class, new()
        {
            return Invoke<TProxyClass, TProxyClass>(true);
        }

        private static TInterface Invoke<TInterface, TImp>(bool inheritMode = false) where TImp : class, new() where TInterface : class
        {
            var impType = typeof(TImp);

            string nameOfAssembly = impType.Name + "ProxyAssembly";
            string nameOfModule = impType.Name + "ProxyModule";
            string nameOfType = impType.Name + "Proxy";

            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(nameOfAssembly), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assembly.DefineDynamicModule(nameOfModule);

            TypeBuilder typeBuilder;
            if (inheritMode)
                typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, impType);
            else
                typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, null, new[] { typeof(TInterface) });

            Type interceptorAttributeType = impType.GetCustomAttribute(typeof(InterceptorBaseAttribute))?.GetType();

            // ---- define fields ----
            FieldBuilder fieldInterceptor = null;
            if (interceptorAttributeType != null)
            {
                fieldInterceptor = typeBuilder.DefineField("_interceptor", interceptorAttributeType, FieldAttributes.Private);
            }
            // ---- define costructors ----
            if (interceptorAttributeType != null)
            {
                var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
                var ilOfCtor = constructorBuilder.GetILGenerator();

                ilOfCtor.Emit(OpCodes.Ldarg_0);
                ilOfCtor.Emit(OpCodes.Newobj, interceptorAttributeType.GetConstructor(new Type[0]));
                ilOfCtor.Emit(OpCodes.Stfld, fieldInterceptor);
                ilOfCtor.Emit(OpCodes.Ret);
            }

            // ---- define methods ----

            var methodsOfType = impType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            string[] ignoreMethodName = new[] { "GetType", "ToString", "GetHashCode", "Equals" };

            foreach (var method in methodsOfType)
            {
                //ignore method
                if (ignoreMethodName.Contains(method.Name))
                    continue;

                var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

                MethodAttributes methodAttributes;

                if (inheritMode)
                    methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual;
                else
                    methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

                var methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes, CallingConventions.Standard, method.ReturnType, methodParameterTypes);

                var ilMethod = methodBuilder.GetILGenerator();

                // set local field
                var impObj = ilMethod.DeclareLocal(impType);                //instance of imp object
                var methodName = ilMethod.DeclareLocal(typeof(string));     //instance of method name
                var parameters = ilMethod.DeclareLocal(typeof(object[]));   //instance of parameters
                var result = ilMethod.DeclareLocal(typeof(object));         //instance of result
                LocalBuilder actionAttributeObj = null;

                //attribute init
                Type actionAttributeType = null;
                if (method.GetCustomAttribute(typeof(ActionBaseAttribute)) != null || impType.GetCustomAttribute(typeof(ActionBaseAttribute)) != null)
                {
                    //method can override class attrubute
                    if (method.GetCustomAttribute(typeof(ActionBaseAttribute)) != null)
                    {
                        actionAttributeType = method.GetCustomAttribute(typeof(ActionBaseAttribute)).GetType();
                    }
                    else if (impType.GetCustomAttribute(typeof(ActionBaseAttribute)) != null)
                    {
                        actionAttributeType = impType.GetCustomAttribute(typeof(ActionBaseAttribute)).GetType();
                    }

                    actionAttributeObj = ilMethod.DeclareLocal(actionAttributeType);
                    ilMethod.Emit(OpCodes.Newobj, actionAttributeType.GetConstructor(new Type[0]));
                    ilMethod.Emit(OpCodes.Stloc, actionAttributeObj);
                }

                //instance imp
                ilMethod.Emit(OpCodes.Newobj, impType.GetConstructor(new Type[0]));
                ilMethod.Emit(OpCodes.Stloc, impObj);

                //if no attribute
                if (fieldInterceptor != null || actionAttributeObj != null)
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
                if (actionAttributeType != null)
                {
                    //load arguments
                    ilMethod.Emit(OpCodes.Ldloc, actionAttributeObj);
                    ilMethod.Emit(OpCodes.Ldloc, methodName);
                    ilMethod.Emit(OpCodes.Ldloc, parameters);
                    ilMethod.Emit(OpCodes.Call, actionAttributeType.GetMethod("Before"));
                }

                if (interceptorAttributeType != null)
                {
                    //load arguments
                    ilMethod.Emit(OpCodes.Ldarg_0);//this
                    ilMethod.Emit(OpCodes.Ldfld, fieldInterceptor);
                    ilMethod.Emit(OpCodes.Ldloc, impObj);
                    ilMethod.Emit(OpCodes.Ldloc, methodName);
                    ilMethod.Emit(OpCodes.Ldloc, parameters);
                    // call Invoke() method of Interceptor
                    ilMethod.Emit(OpCodes.Callvirt, interceptorAttributeType.GetMethod("Invoke"));
                }
                else
                {
                    //direct call method
                    if (method.ReturnType == typeof(void) && actionAttributeType == null)
                    {
                        ilMethod.Emit(OpCodes.Ldnull);
                    }

                    ilMethod.Emit(OpCodes.Ldloc, impObj);
                    for (var j = 0; j < methodParameterTypes.Length; j++)
                    {
                        ilMethod.Emit(OpCodes.Ldarg, j + 1);
                    }
                    ilMethod.Emit(OpCodes.Callvirt, impType.GetMethod(method.Name));
                    //box
                    if (actionAttributeType != null)
                    {
                        if (method.ReturnType != typeof(void))
                            ilMethod.Emit(OpCodes.Box, method.ReturnType);
                        else
                            ilMethod.Emit(OpCodes.Ldnull);
                    }
                }

                //dynamic proxy action after
                if (actionAttributeType != null)
                {
                    ilMethod.Emit(OpCodes.Stloc, result);
                    //load arguments
                    ilMethod.Emit(OpCodes.Ldloc, actionAttributeObj);
                    ilMethod.Emit(OpCodes.Ldloc, methodName);
                    ilMethod.Emit(OpCodes.Ldloc, result);
                    ilMethod.Emit(OpCodes.Call, actionAttributeType.GetMethod("After"));
                }

                // pop the stack if return void
                if (method.ReturnType == typeof(void))
                {
                    ilMethod.Emit(OpCodes.Pop);
                }
                else
                {
                    //unbox,if direct invoke,no box
                    if (fieldInterceptor != null || actionAttributeObj != null)
                    {
                        if (method.ReturnType.IsValueType)
                            ilMethod.Emit(OpCodes.Unbox_Any, method.ReturnType);
                        else
                            ilMethod.Emit(OpCodes.Castclass, method.ReturnType);
                    }
                }
                // complete
                ilMethod.Emit(OpCodes.Ret);
            }

            var t = typeBuilder.CreateTypeInfo();

            return Activator.CreateInstance(t) as TInterface;
        }
    }
}