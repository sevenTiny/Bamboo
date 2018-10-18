/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Peking
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SevenTiny.Bantina
{
    internal delegate object CreateInstanceHandler(object[] parameters);

    public class CreateObjectFactory
    {
        static Dictionary<string, CreateInstanceHandler> mHandlers = new Dictionary<string, CreateInstanceHandler>();

        public static T CreateInstance<T>() where T : class
        {
            return CreateInstance<T>(null);
        }

        public static T CreateInstance<T>(params object[] parameters) where T : class
        {
            return (T)CreateInstance(typeof(T), parameters);
        }

        public static object CreateInstance(Type instanceType, params object[] parameters)
        {
            Type[] ptypes = new Type[0];
            string key = instanceType.FullName;

            if (parameters != null && parameters.Any())
            {
                ptypes = parameters.Select(t => t.GetType()).ToArray();
                key = string.Concat(key, "_", string.Concat(ptypes.Select(t => t.Name)));
            }

            if (!mHandlers.ContainsKey(key))
            {
                CreateHandler(instanceType, key, ptypes);
            }
            return mHandlers[key](parameters);
        }

        static void CreateHandler(Type objtype, string key, Type[] ptypes)
        {
            lock (typeof(CreateObjectFactory))
            {
                if (!mHandlers.ContainsKey(key))
                {
                    DynamicMethod dm = new DynamicMethod(key, typeof(object), new Type[] { typeof(object[]) }, typeof(CreateObjectFactory).Module);
                    ILGenerator il = dm.GetILGenerator();
                    ConstructorInfo cons = objtype.GetConstructor(ptypes);

                    if (cons == null)
                    {
                        throw new MissingMethodException("The constructor for the corresponding parameter was not found");
                    }

                    il.Emit(OpCodes.Nop);

                    for (int i = 0; i < ptypes.Length; i++)
                    {
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldc_I4, i);
                        il.Emit(OpCodes.Ldelem_Ref);
                        if (ptypes[i].IsValueType)
                            il.Emit(OpCodes.Unbox_Any, ptypes[i]);
                        else
                            il.Emit(OpCodes.Castclass, ptypes[i]);
                    }

                    il.Emit(OpCodes.Newobj, cons);
                    il.Emit(OpCodes.Ret);
                    CreateInstanceHandler ci = (CreateInstanceHandler)dm.CreateDelegate(typeof(CreateInstanceHandler));
                    mHandlers.Add(key, ci);
                }
            }
        }
    }
}