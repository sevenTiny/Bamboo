/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 10/10/2018, 1:19:24 PM
* Modify: 
* E-mailGenerator: dong@7tiny.com | sevenTiny@foxmailGenerator.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 
* Thx , Best Regards ~
*********************************************************/
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SevenTiny.Bantina
{
    public class ObjectHelper
    {

    }

    public class ObjectHelper<T> where T : class
    {
        private static Func<T> objCreator = null;
        private delegate T ObjectCreateInvoker (object[] parameters);
        private static ObjectCreateInvoker objectCreateInvoker;

        public static T New()
        {
            if (objCreator == null)
            {
                Type objectType = typeof(T);

                ConstructorInfo defaultCtor = objectType.GetConstructor(new Type[0]);

                DynamicMethod dynMethod = new DynamicMethod(
                    name: string.Format("_{0:N}", Guid.NewGuid()),
                    returnType: objectType,
                    parameterTypes: null);

                var ilGenerator = dynMethod.GetILGenerator();
                ilGenerator.Emit(OpCodes.Newobj, defaultCtor);
                ilGenerator.Emit(OpCodes.Ret);

                objCreator = dynMethod.CreateDelegate(typeof(Func<T>)) as Func<T>;
            }
            return objCreator();
        }

        public static T New(params object[] constructorParameters)
        {
            if (objCreator == null)
            {
                Type objectType = typeof(T);

                Type[] parameterTypes = constructorParameters.Select(t => t.GetType()).ToArray();

                ConstructorInfo defaultCtor = objectType.GetConstructor(parameterTypes);

                if (defaultCtor == null)
                {
                    throw new MissingMethodException("The constructor for the corresponding parameter was not found");
                }

                //DynamicMethod dynMethod = new DynamicMethod(name: , returnType: objectType, parameterTypes: null);
                DynamicMethod dynMethod = parameterTypes == null || parameterTypes.Length == 0 ? new DynamicMethod(string.Format("_{0:N}", Guid.NewGuid()), objectType, null) : new DynamicMethod(string.Format("_{0:N}", Guid.NewGuid()), objectType, parameterTypes);
                var ilGenerator = dynMethod.GetILGenerator();

                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldc_I4, i);
                    ilGenerator.Emit(OpCodes.Ldelem, typeof(object));

                    var constructorParameterType = parameterTypes[i];

                    if (constructorParameterType.IsValueType)
                        ilGenerator.Emit(OpCodes.Unbox_Any, constructorParameterType);
                    else
                        ilGenerator.Emit(OpCodes.Castclass, constructorParameterType);
                }

                ilGenerator.Emit(OpCodes.Newobj, defaultCtor);
                ilGenerator.Emit(OpCodes.Ret);

                objectCreateInvoker = (ObjectCreateInvoker)dynMethod.CreateDelegate(typeof(ObjectCreateInvoker));
            }
            return objectCreateInvoker(constructorParameters);
        }
    }
}