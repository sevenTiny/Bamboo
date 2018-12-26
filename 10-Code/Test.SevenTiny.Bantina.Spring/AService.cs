using SevenTiny.Bantina.Spring;
using SevenTiny.Bantina.Spring.DependencyInjection;

namespace Test.SevenTiny.Bantina.Spring
{
    public interface IAService
    {
        void ServiceTest();
    }

    public class AService : IAService
    {
        [Service]
        private IBService bService;

        [Action2]
        [Action1]
        public void ServiceTest()
        {
            bService.Test();
        }
    }
}
