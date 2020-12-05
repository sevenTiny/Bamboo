using SevenTiny.Bantina.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Test.SevenTiny.Bantina.Redis
{
    public class RedisLockTest
    {
        private IRedisLock Instance { get; set; }
        private void Init()
        {
            Instance = new RedisLockManager("Default");
        }

        [Fact]
        public void Lock()
        {
            string lockName = "lock";

            Instance.Lock(lockName,
            () =>
            {
                Trace.WriteLine("lock success! time out 3 second");
                Thread.Sleep(3000);
            },
            () =>
            {
                Trace.WriteLine("lock error");
            });
        }

        [Fact]
        public void Lock2()
        {
            string lockName = "lock";

            string successMsg = "lock execute succeed!";

            string result;

            result = Instance.Lock<string>(lockName,
            () =>
            {
                Trace.WriteLine("lock success!");
                return successMsg;
            },
            () =>
            {
                Trace.WriteLine("lock error");
                return "lock execute error!";
            });

            Trace.WriteLine($"result={result}");

            Assert.Equal(result, successMsg);
        }

        [Fact]
        public void LockThread()
        {
            string lockName = "lock";
            for (int i = 0; i < 3; i++)
            {
                Task.Run(() =>
                {
                    Instance.Lock(lockName,
                    () =>
                    {
                        Trace.WriteLine("-------- lock success!");
                    },
                    () =>
                    {
                        Trace.WriteLine("lock error");
                    });
                });
            }
        }

        [Fact]
        public void LockNoRelease()
        {
            string lockName = "lock";

            bool result = Instance.Lock(lockName);

            Assert.True(result);
        }

        [Fact]
        public void LockRelease()
        {
            string lockName = "lock";

            bool result = Instance.LockRelease(lockName);

            Assert.True(result);
        }
    }
}
