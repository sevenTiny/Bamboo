using System;

namespace SevenTiny.Bantina.Redis
{
    public interface IRedisLock
    {
        void Lock(string lockName, Action accessLockMethod, Action refuseMethod);
         T Lock<T>(string lockName, Func<T> accessLockMethod, Func<T> refuseMethod);
    }
}
