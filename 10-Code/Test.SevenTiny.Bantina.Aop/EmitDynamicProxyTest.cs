using SevenTiny.Bantina.Aop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Test.SevenTiny.Bantina.Aop
{
    public class EmitDynamicProxyTest
    {
        [Fact]
        [Trait("desc","实现方式动态代理")]
        public void FaultTolerantOfRealize()
        {
            IBusinessClass Instance = DynamicProxy.CreateProxyOfRealize<IBusinessClass, BusinessClass>();
            //IBusinessClass Instance = new BusinessClassProxy();

            Instance.Test();
            Instance.GetInt(123);
            Instance.NoArgument();
            Instance.ThrowException();
            Instance.ArgumentVoid(123, "123");
            Instance.GetBool(false);
            Instance.GetString("123");
            Instance.GetFloat(123f);
            Instance.GetDouble(123.123);
            Instance.GetObject(null);
            Instance.GetOperateResult(123, "123");
            Instance.GetOperateResults(new List<OperateResult>());
            Instance.GetDecimal(123.123m);
            Instance.GetDateTime(DateTime.Now);
        }

        [Fact]
        [Trait("desc", "继承方式动态代理")]
        public void FualtTolerantOfInherit()
        {
            //IBusinessClass Instance = new BusinessClassVirtualProxy();
            IBusinessClass Instance = DynamicProxy.CreateProxyOfInherit<BusinessClassInherit>();

            Instance.Test();
            Instance.NoArgument();
            Instance.GetBool(false);
            Instance.GetInt(123);
            Instance.GetFloat(123f);
            Instance.GetDouble(123.123);
            Instance.GetString("123");
            Instance.GetObject(null);
            Instance.GetOperateResult(123, "123");
            Instance.GetOperateResults(new List<OperateResult>());
            Instance.GetDecimal(123.123m);
            Instance.GetDateTime(DateTime.Now);
        }

        [Fact]
        [Trait("desc", "直接调用-性能测试")]
        public void Performance2()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 1000000; i++)
            {
                var instance = BusinessClass.Instance2;
                instance.Test();
                var result = instance.GetOperateResult(111, "333");
                int intResult = instance.GetInt(222);
            }

            stopwatch.Stop();
            Trace.WriteLine($"直接调用耗时:{stopwatch.ElapsedMilliseconds}");
            //不使用代理类，百万次调用  58 ms
        }

        [Fact]
        [Trait("desc", "接口实现方式动态代理-性能测试")]
        public void Performance1()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 1000000; i++)
            {
                var instance = BusinessClass.Instance;
                instance.Test();
                var result = instance.GetOperateResult(111, "333");
                int intResult = instance.GetInt(222);
            }

            stopwatch.Stop();
            Trace.WriteLine($"动态代理调用耗时:{stopwatch.ElapsedMilliseconds}");
            //使用了代理类，但是没有添加任何类标签和方法标签，百万次调用 73ms    （相当于直接调用）
            //使用了代理类，添加类代理标签，百万次调用  1112ms  （Invoke）
            //使用了代理类，添加了方法代理标签，百万次调用   177ms    (拆装箱）
            //使用了代理类，添加了类代理标签和方法代理标签，百万次调用  1231ms  （Invoke+拆装箱）
        }

        [Fact]
        [Trait("desc", "继承实现方式动态代理-性能测试")]
        public void Performance3()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 1000000; i++)
            {
                var instance = BusinessClassInherit.Instance;
                instance.Test();
                var result = instance.GetOperateResult(111, "333");
                int intResult = instance.GetInt(222);
            }

            stopwatch.Stop();
            Trace.WriteLine($"动态代理调用耗时:{stopwatch.ElapsedMilliseconds}");
            //使用了代理类，但是没有添加任何类标签和方法标签，百万次调用 73ms    （相当于直接调用）
            //使用了代理类，添加类代理标签，百万次调用  1074ms  （Invoke）
            //使用了代理类，添加了方法代理标签，百万次调用   191ms    (拆装箱）
            //使用了代理类，添加了类代理标签和方法代理标签，百万次调用  1216ms  （Invoke+拆装箱）
        }

        [Fact]
        [Trait("desc", "Exception")]
        public void ExceptionFilter()
        {
            IBusinessClass Instance = DynamicProxy.CreateProxyOfRealize<IBusinessClass, BusinessClass>();
            Instance.ThrowException();
        }
    }
}