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
        [Fact]
        public void Lock()
        {
            IRedisLock locker = RedisLockManager.Instance;

            string lockName = "lock";

            locker.Lock(lockName,
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
            IRedisLock locker = RedisLockManager.Instance;

            string lockName = "lock";

            string successMsg = "lock execute succeed!";

            string result;

            result = locker.Lock<string>(lockName,
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
            IRedisLock locker = RedisLockManager.Instance;

            string lockName = "lock";
            for (int i = 0; i < 3; i++)
            {
                Task.Run(() =>
                {
                    locker.Lock(lockName,
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
            IRedisLock locker = RedisLockManager.Instance;

            string lockName = "lock";

            bool result = locker.Lock(lockName);

            Assert.True(result);
        }

        [Fact]
        public void LockRelease()
        {
            IRedisLock locker = RedisLockManager.Instance;

            string lockName = "lock";

            bool result = locker.LockRelease(lockName);

            Assert.True(result);
        }
    }
}
