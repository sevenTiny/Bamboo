/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 10/10/2018, 1:19:24 PM
* Modify: 
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 
* Thx , Best Regards ~
*********************************************************/
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SevenTiny.Bantina
{
    public class ObjectHelper
    {
    }

    public class ObjectHelper<T> where T : class
    {
        public static Func<T> objCreator = null;

        public static T New()
        {
            if (objCreator == null)
            {
                Type objectType = typeof(T);

                ConstructorInfo defaultCtor = objectType.GetConstructor(new Type[] { });

                DynamicMethod dynMethod = new DynamicMethod(
                    name: string.Format("_{0:N}", Guid.NewGuid()),
                    returnType: objectType,
                    parameterTypes: null);

                var gen = dynMethod.GetILGenerator();
                gen.Emit(OpCodes.Newobj, defaultCtor);
                gen.Emit(OpCodes.Ret);

                objCreator = dynMethod.CreateDelegate(typeof(Func<T>)) as Func<T>;
            }
            return objCreator();
        }
    }
}