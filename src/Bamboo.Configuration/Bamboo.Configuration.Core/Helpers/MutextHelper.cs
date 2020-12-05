using System;
using System.Reflection;
using System.Threading;

namespace Bamboo.Configuration.Core.Helpers
{
    internal class MutextHelper
    {
        public void Lock(Action action, Action exceptionAction = null)
        {
            //mutext start -------------
            // get application GUID as defined in AssemblyInfo.cs
            string appGuid = Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToString();

            // unique id for global mutex - Global prefix means it is global to the machine
            string mutexId = string.Format("Global\\{{{0}}}", appGuid);

            // Need a place to store a return value in Mutex() constructor call
            bool createdNew;

            // edited by MasonGZhwiti to prevent race condition on security settings via VanNguyen
            using (var mutex = new Mutex(false, mutexId, out createdNew))
            {
                // edited by acidzombie24
                var hasHandle = false;
                try
                {
                    try
                    {
                        // note, you may want to time out here instead of waiting forever
                        // edited by acidzombie24
                        // mutex.WaitOne(Timeout.Infinite, false);
                        hasHandle = mutex.WaitOne(5000, false);
                        if (hasHandle == false)
                            throw new TimeoutException("Timeout waiting for exclusive access");
                    }
                    catch (AbandonedMutexException)
                    {
                        // Log the fact that the mutex was abandoned in another process,
                        // it will still get acquired
                        hasHandle = true;
                    }

                    action.Invoke();
                }
                catch (Exception ex)
                {
                    if (exceptionAction!=null)
                    {
                        exceptionAction.Invoke();
                    }
                    throw ex;
                }
                finally
                {
                    // edited by acidzombie24, added if statement
                    if (hasHandle)
                        mutex.ReleaseMutex();
                }
            }
        }
    }
}
