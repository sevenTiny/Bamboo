using SevenTiny.Bantina.Spring;
using SevenTiny.Bantina.Spring.Aop;

namespace Test.SevenTiny.Bantina.Spring
{
    public class InterceptorAttribute : InterceptorBaseAttribute
    {
        public override object Invoke(object @object, string method, object[] parameters)
        {
            SpringContext context = new SpringContext(204233, 100373299, @object, method, parameters);
            StartUp.RequestDelegate(context);
            return context.Result = base.Invoke(@object, method, parameters);
        }
    }
}
