using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Test.SevenTiny.Bantina.Model;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    public class ExpressionTest
    {
        public static void Test()
        {
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

    }
}
