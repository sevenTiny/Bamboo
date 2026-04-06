using Bamboo.ScriptEngine.Core;
using SevenTiny.Bantina.Validation;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Bamboo.ScriptEngine.CSharp.SandBox
{
    [Serializable]
    internal class RunContainer : MarshalByRefObject
    {
        public static ExecutionResult<T> ExecuteTrustedCode<T>(Type type, MethodInfo methodInfo, object[] parameters)
        {
            var parms = methodInfo.GetParameters();
            var safeParameters = SafeTypeConvertParameters(methodInfo.Name, parms, parameters);

            var result = InvokeMethod(type, methodInfo, safeParameters);

            return GetExecutionResultFromPossiblyTaskSync<T>(result);
        }

        public static ExecutionResult<T> ExecuteUntrustedCode<T>(Type type, MethodInfo methodInfo, int millisecondsTimeout, object[] parameters)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var task = Task.Run(() => ExecuteTrustedCode<T>(type, methodInfo, parameters), token);

            if (!task.Wait(millisecondsTimeout, token))
            {
                tokenSource.Cancel();
                string errorMessage = string.Format("[Assembly:{0},Method:{1},Timeout:{2}, execution timed out.", type.Assembly.FullName, methodInfo.Name, millisecondsTimeout);
                throw new ScriptEngineException(errorMessage);
            }

            return task.Result;
        }

        public static async Task<ExecutionResult<T>> ExecuteTrustedCodeAsync<T>(Type type, MethodInfo methodInfo, object[] parameters)
        {
            var parms = methodInfo.GetParameters();
            var safeParameters = SafeTypeConvertParameters(methodInfo.Name, parms, parameters);

            var result = InvokeMethod(type, methodInfo, safeParameters);

            return await GetExecutionResultFromPossiblyTaskAsync<T>(result).ConfigureAwait(false);
        }

        public static async Task<ExecutionResult<T>> ExecuteUntrustedCodeAsync<T>(Type type, MethodInfo methodInfo, int millisecondsTimeout, object[] parameters)
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var runningTask = ExecuteTrustedCodeAsync<T>(type, methodInfo, parameters);

            var delayTask = Task.Delay(millisecondsTimeout, token);

            var finished = await Task.WhenAny(runningTask, delayTask).ConfigureAwait(false);

            if (finished == delayTask)
            {
                cts.Cancel();
                string errorMessage = string.Format("[Assembly:{0},Method:{1},Timeout:{2}, execution timed out.", type.Assembly.FullName, methodInfo.Name, millisecondsTimeout);
                throw new ScriptEngineException(errorMessage);
            }

            return await runningTask.ConfigureAwait(false);
        }

        private static object[] SafeTypeConvertParameters(string method, ParameterInfo[] parameterInfos, object[] parameters)
        {
            if (parameterInfos.Length == 0)
                return null;
            Ensure.ArgumentNotNullOrEmpty(parameters, nameof(parameters));

            if (parameterInfos.Length != parameters.Length)
                throw new ArgumentException($"The number of parameters of {method} a does not match.", nameof(parameters));

            object[] result = new object[parameters.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                //这里如果有参数没有实现IConvert接口，则会抛出异常
                if (typeof(IConvertible).IsAssignableFrom(parameterInfos[i].ParameterType))
                    result[i] = Convert.ChangeType(parameters[i], parameterInfos[i].ParameterType);
                else
                    result[i] = parameters[i];
            }

            return result;
        }

        // Helper: invoke method (static or instance) and unwrap TargetInvocationException.InnerException
        private static object InvokeMethod(Type type, MethodInfo methodInfo, object[] safeParameters)
        {
            try
            {
                if (methodInfo.IsStatic)
                {
                    return methodInfo.Invoke(null, safeParameters);
                }

                var instance = Activator.CreateInstance(type);
                return methodInfo.Invoke(instance, safeParameters);
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                throw tie.InnerException;
            }
        }

        private static ExecutionResult<T> GetExecutionResultFromPossiblyTaskSync<T>(object result)
        {
            if (result == null)
                return ExecutionResult<T>.Success(default);

            var resultType = result.GetType();

            if (typeof(Task).IsAssignableFrom(resultType))
            {
                var task = (Task)result;
                task.GetAwaiter().GetResult();

                var prop = resultType.GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                {
                    var ret = prop.GetValue(result);
                    return ExecutionResult<T>.Success((T)ret);
                }

                return ExecutionResult<T>.Success(default);
            }

            if (resultType.IsValueType && resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                var asTaskMethod = resultType.GetMethod("AsTask");
                var taskObj = (Task)asTaskMethod.Invoke(result, null);
                taskObj.GetAwaiter().GetResult();
                var prop = taskObj.GetType().GetProperty("Result");
                var ret = prop.GetValue(taskObj);
                return ExecutionResult<T>.Success((T)ret);
            }

            if (resultType == typeof(ValueTask))
            {
                var asTaskMethod = resultType.GetMethod("AsTask");
                var taskObj = (Task)asTaskMethod.Invoke(result, null);
                taskObj.GetAwaiter().GetResult();
                return ExecutionResult<T>.Success(default);
            }

            return ExecutionResult<T>.Success((T)result);
        }

        private static async Task<ExecutionResult<T>> GetExecutionResultFromPossiblyTaskAsync<T>(object result)
        {
            if (result == null)
                return ExecutionResult<T>.Success(default);

            var resultType = result.GetType();

            if (typeof(Task).IsAssignableFrom(resultType))
            {
                var task = (Task)result;
                await task.ConfigureAwait(false);

                var prop = resultType.GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                {
                    var ret = prop.GetValue(result);
                    return ExecutionResult<T>.Success((T)ret);
                }

                return ExecutionResult<T>.Success(default);
            }

            if (resultType.IsValueType && resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                var asTaskMethod = resultType.GetMethod("AsTask");
                var taskObj = (Task)asTaskMethod.Invoke(result, null);
                await taskObj.ConfigureAwait(false);
                var prop = taskObj.GetType().GetProperty("Result");
                var ret = prop.GetValue(taskObj);
                return ExecutionResult<T>.Success((T)ret);
            }

            if (resultType == typeof(ValueTask))
            {
                var asTaskMethod = resultType.GetMethod("AsTask");
                var taskObj = (Task)asTaskMethod.Invoke(result, null);
                await taskObj.ConfigureAwait(false);
                return ExecutionResult<T>.Success(default);
            }

            return ExecutionResult<T>.Success((T)result);
        }
    }
}