using SevenTiny.Bantina.Spring.Aop;
using System.Diagnostics;

namespace Test.SevenTiny.Bantina.Spring
{
    public class ActionAttribute : ActionBaseAttribute
    {
        public override void Before(string method, object[] parameters)
        {
            Trace.WriteLine("DomainService Action");
            base.Before(method, parameters);
        }
    }
    public interface IDomainService
    {
        void WriteLog();
    }

    [Action]
    public class DomainService : IDomainService
    {
        public void WriteLog()
        {
            Trace.WriteLine("大吉大利，今晚吃鸡！");
        }
    }
}
