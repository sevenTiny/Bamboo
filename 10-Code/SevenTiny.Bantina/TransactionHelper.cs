using System;
using System.Transactions;

namespace SevenTiny.Bantina
{
    public class TransactionHelper
    {
        public static void Transaction(Action action)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                action();
                scope.Complete();
            }
        }
        public static T Transaction<T>(Func<T> func)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                T t = func();
                scope.Complete();
                return t;
            }
        }
    }
}
