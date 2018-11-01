using SevenTiny.Bantina.Spring.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Spring
{
    public interface IBusinessService
    {
        void Test();
    }

    public class BusinessService : IBusinessService
    {
        [Service]
        private IDomainService domainService;

        [Service]
        private IStorageProvider storage;

        public void Test()
        {
            domainService.WriteLog();
        }
    }

    public interface IStorageProvider { }
    public class StorageProvider : IStorageProvider
    {
        public static IStorageProvider Storage = new StorageProvider();
    }
}
