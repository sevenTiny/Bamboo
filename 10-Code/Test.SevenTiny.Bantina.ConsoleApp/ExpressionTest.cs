using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Test.SevenTiny.Bantina.Model;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    public class ExpressionTest<TIn, TOut>
    {
        public static void Test()
        {
            var instance = GetCreateFunc<Student>();
            NewExpression newExp = Expression.New(typeof(Student));
            Expression<Func<Student>> lambdaExp = Expression.Lambda<Func<Student>>(newExp, null);
            Func<Student> func = lambdaExp.Compile();
            Student stu = func.Invoke();
            stu.Name = "123";

            Console.WriteLine(stu.GetName());
        }
        public List<T> ExpressionTree<T>(List<T> collection, object propertyName, string propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

            ParameterExpression value = Expression.Parameter(typeof(string), "propertyValue");

            MethodInfo setter = typeof(T).GetMethod("set_" + propertyName);

            MethodCallExpression call = Expression.Call(parameter, setter, value);
            var lambda = Expression.Lambda<Action<T, string>>(call, parameter, value);
            var exp = lambda.Compile();
            for (int i = 0; i < collection.Count; i++)
            {
                exp(collection[i], propertyValue);
            }
            return collection;
        }
        static T GetCreateFunc<T>() where T : class
        {
            var newExpression = Expression.New(typeof(T));
            Func<T> func = Expression.Lambda<Func<T>>(newExpression, null).Compile();
            return func();
        }

        static T GetEmitCreateFunc<T>() where T : class
        {
            var ctor = typeof(T).GetConstructors()[0];
            DynamicMethod method = new DynamicMethod(String.Empty, typeof(T), null);
            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ret);
            Func<T> func = method.CreateDelegate(typeof(Func<T>)) as Func<T>;
            return func();
        }

        private static readonly Func<TIn, TOut> cache = GetFunc();
        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                PropertyInfo propertyInfo = typeof(TIn).GetProperty(item.Name);
                if (item.CanWrite && propertyInfo != null)
                {
                    MemberExpression property = Expression.Property(parameterExpression, propertyInfo);
                    MemberBinding memberBinding = Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        public static TOut Trans(TIn tIn)
        {
            return cache(tIn);
        }
    }
}
