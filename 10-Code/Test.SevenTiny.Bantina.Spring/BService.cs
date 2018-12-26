using SevenTiny.Bantina.Spring.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Spring
{
    public interface IBService
    {
        void Test();
    }

    public class BService : IBService
    {
        [Service]
        private ICService cService;

        [Service]
        private IAService aService;

        [Service]
        private IStorageProvider storage;

        public void Test()
        {
            cService.WriteLog();
        }
    }

    public interface IStorageProvider { }
    public class StorageProvider : IStorageProvider
    {
        public static IStorageProvider Storage = new StorageProvider();
    }
}
