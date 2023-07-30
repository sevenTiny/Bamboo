using SevenTiny.Bantina.Spring;
using SevenTiny.Bantina.Spring.Aop;
using System.Diagnostics;

namespace Test.SevenTiny.Bantina.Spring
{
    public class InterceptorAttribute : InterceptorBaseAttribute
    {
        public override object Invoke(object @object, string method, object[] parameters)
        {
            Trace.WriteLine($"inner interceptor,method:{method},parameters:[{string.Join(",", parameters)}]");
            SpringContext context = new SpringContext(204233, 100373299, @object, method, parameters);
            StartUp.RequestDelegate(context);
            return context.Result;
        }
    }

    public class Action1Attribute : ActionBaseAttribute
    {
        public override object After(string method, object result)
        {
            Trace.WriteLine("action1 after ...");
            return base.After(method, result);
        }

        public override void Before(string method, object[] parameters)
        {
            Trace.WriteLine("action1 before ...");
            base.Before(method, parameters);
        }
    }

    public class Action2Attribute : ActionBaseAttribute
    {
        public override object After(string method, object result)
        {
            Trace.WriteLine("action2 after ...");
            return base.After(method, result);
        }

        public override void Before(string method, object[] parameters)
        {
            Trace.WriteLine("action2 before ...");
            base.Before(method, parameters);
        }
    }
}
