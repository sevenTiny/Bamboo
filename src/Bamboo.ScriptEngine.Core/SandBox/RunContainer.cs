using Fasterflect;
using Microsoft.Extensions.Logging;
using Bamboo.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bamboo.ScriptEngine.SandBox
{
    [Serializable]
    internal class RunContainer : MarshalByRefObject
    {
        private static readonly ILogger _logger = new BambooLogger<RunContainer>();

        public object ExecuteUntrustedCode(Type type, string methodName, params object[] parameters)
        {
            var obj = Activator.CreateInstance(type);
            try
            {
                return obj.TryCallMethodWithValues(methodName, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"call method [{methodName}] error.");
            }
            return null;
        }

        public object ExecuteUntrustedCode(Type type, string methodName, int millisecondsTimeout, params object[] parameters)
        {
            object result = null;
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var t = Task.Factory.StartNew(() => { result = ExecuteUntrustedCode(type, methodName, parameters); }, token);

            if (!t.Wait(millisecondsTimeout, token))
            {
                tokenSource.Cancel();
                _logger.LogError(string.Format("[Tag:{0},Method:{1},Timeout:{2}, execution timed out", type.Assembly.FullName, methodName, millisecondsTimeout));
                return null;
            }
            return result;
        }
    }
}
