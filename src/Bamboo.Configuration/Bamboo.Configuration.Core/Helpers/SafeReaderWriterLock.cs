using System;
using System.Threading;

namespace Bamboo.Configuration.Core.Helpers
{
    internal interface ISafeLock : IDisposable
    {
        void Lock();
    }

    /// <summary>
    /// 读写锁
    /// </summary>
    internal class SafeReaderWriterLock
    {
        SafeUpgradeReaderLock upgradableReaderLock;
        SafeReaderLock readerLock;
        SafeWriterLock writerLock;

        ReaderWriterLockSlim locker;
        public SafeReaderWriterLock()
        {
            locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            upgradableReaderLock = new SafeUpgradeReaderLock(locker);
            readerLock = new SafeReaderLock(locker);
            writerLock = new SafeWriterLock(locker);
        }

        public ISafeLock AcquireReaderLock()
        {
            readerLock.Lock();
            return readerLock;
        }
        public ISafeLock AcquireWriterLock()
        {
            writerLock.Lock();
            return writerLock;
        }
        public ISafeLock AcquireUpgradeableReaderLock()
        {
            upgradableReaderLock.Lock();
            return upgradableReaderLock;
        }
    }

    internal class SafeReaderLock : ISafeLock
    {
        ReaderWriterLockSlim locker;
        public SafeReaderLock(ReaderWriterLockSlim locker)
        {
            this.locker = locker;
        }

        public void Dispose()
        {
            locker.ExitReadLock();
        }
        public void Lock()
        {
            locker.EnterReadLock();
        }
        public void UnLock()
        {
            locker.ExitUpgradeableReadLock();
        }
    }

    internal class SafeWriterLock : ISafeLock
    {
        ReaderWriterLockSlim locker;
        public SafeWriterLock(ReaderWriterLockSlim locker)
        {
            this.locker = locker;
        }

        public void Dispose()
        {
            locker.ExitWriteLock();
        }
        public void Lock()
        {
            locker.EnterWriteLock();

        }
        public void UnLock()
        {
            locker.ExitUpgradeableReadLock();
        }
    }

    internal class SafeUpgradeReaderLock : ISafeLock
    {
        ReaderWriterLockSlim locker;
        public SafeUpgradeReaderLock(ReaderWriterLockSlim locker)
        {
            this.locker = locker;
        }

        public void Dispose()
        {
            locker.ExitUpgradeableReadLock();
        }
        public void Lock()
        {
            locker.EnterUpgradeableReadLock();
        }
        public void UnLock()
        {
            locker.ExitUpgradeableReadLock();
        }
    }
}
