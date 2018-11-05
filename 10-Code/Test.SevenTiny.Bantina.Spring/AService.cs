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
        public static IAService Instance = SpringContext.RequestServices.GetService<IAService>();

        [Service]
        private IBusinessService businessService;

        [Action2]
        [Action1]
        public void ServiceTest()
        {
            businessService.Test();
        }
    }
}
