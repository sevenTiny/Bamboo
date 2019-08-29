using SevenTiny.Bantina.Spring.Aop;
using System.Diagnostics;

namespace Test.SevenTiny.Bantina.Spring
{
    public class ActionAttribute : ActionBaseAttribute
    {
        public override void Before(string method, object[] parameters)
        {
            Trace.WriteLine("action before");
            base.Before(method, parameters);
        }
    }

    public interface ICService
    {
        void WriteLog();
    }

    [Action]
    public class CService : ICService
    {
        public void WriteLog()
        {
            Trace.WriteLine("大吉大利，今晚吃鸡！");
        }
    }
}
