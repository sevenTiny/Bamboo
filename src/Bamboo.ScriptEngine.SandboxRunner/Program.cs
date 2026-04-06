using Newtonsoft.Json;
using System.Reflection;
using System.Text;

// Args: <assemblyPath> <typeFullName> <methodName> <paramsBase64>
class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            if (args == null || args.Length < 4)
            {
                Console.Error.WriteLine("Usage: runner <assemblyPath> <typeFullName> <methodName> <paramsBase64>");
                return 2;
            }

            var assemblyPath = args[0];
            var typeFullName = args[1];
            var methodName = args[2];
            var paramsBase64 = args[3];

            var paramsJson = Encoding.UTF8.GetString(Convert.FromBase64String(paramsBase64));
            var paramArray = JsonConvert.DeserializeObject<object[]>(paramsJson) ?? Array.Empty<object>();

            if (!File.Exists(assemblyPath))
            {
                Console.Error.WriteLine($"Assembly file not found: {assemblyPath}");
                return 3;
            }

            var alc = System.Runtime.Loader.AssemblyLoadContext.Default;
            var asm = alc.LoadFromAssemblyPath(Path.GetFullPath(assemblyPath));

            var type = asm.GetType(typeFullName);
            if (type == null)
            {
                Console.Error.WriteLine($"Type not found: {typeFullName}");
                return 4;
            }

            var method = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == methodName);

            if (method == null)
            {
                Console.Error.WriteLine($"Method not found: {methodName}");
                return 5;
            }

            var parametersInfo = method.GetParameters();
            object[] invokeParams = BuildInvokeParameters(parametersInfo, paramArray);

            object? instance = null;

            if (!method.IsStatic)
            {
                instance = Activator.CreateInstance(type);
            }

            object result = method.Invoke(instance, invokeParams);

            // handle Task/ValueTask
            object? data = null;

            if (result != null)
            {
                var resultType = result.GetType();

                if (typeof(Task).IsAssignableFrom(resultType))
                {
                    var task = (Task)result;
                    await task.ConfigureAwait(false);

                    var prop = resultType.GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null)
                    {
                        data = prop.GetValue(result);
                    }
                }
                else if (resultType.IsValueType && resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(ValueTask<>))
                {
                    var asTaskMethod = resultType.GetMethod("AsTask");
                    var taskObj = (Task)asTaskMethod.Invoke(result, null);
                    await taskObj.ConfigureAwait(false);
                    var prop = taskObj.GetType().GetProperty("Result");
                    data = prop.GetValue(taskObj);
                }
                else if (resultType == typeof(ValueTask))
                {
                    var asTaskMethod = resultType.GetMethod("AsTask");
                    var taskObj = (Task)asTaskMethod.Invoke(result, null);
                    await taskObj.ConfigureAwait(false);
                }
                else
                {
                    data = result;
                }
            }

            var wrapper = new { Data = data, Message = (string?)null };
            var outJson = JsonConvert.SerializeObject(wrapper);
            Console.Out.Write(outJson);
            return 0;
        }
        catch (TargetInvocationException tie) when (tie.InnerException != null)
        {
            Console.Error.WriteLine(tie.InnerException.ToString());
            return 20;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    static object[] BuildInvokeParameters(ParameterInfo[] parameterInfos, object[] provided)
    {
        if (parameterInfos.Length == 0)
            return Array.Empty<object>();

        var providedCount = provided?.Length ?? 0;

        if (parameterInfos.Length != providedCount)
        {
            // try to match when last parameter is CancellationToken -> pass CancellationToken.None
            if (parameterInfos.Length == providedCount + 1 && parameterInfos.Last().ParameterType == typeof(System.Threading.CancellationToken))
            {
                var result = new object[parameterInfos.Length];
                for (int i = 0; i < providedCount; i++)
                {
                    result[i] = ConvertJsonToType(provided[i], parameterInfos[i].ParameterType);
                }
                result[^1] = System.Threading.CancellationToken.None;
                return result;
            }

            throw new ArgumentException("Parameter count mismatch");
        }

        var ret = new object[providedCount];
        for (int i = 0; i < providedCount; i++)
        {
            ret[i] = ConvertJsonToType(provided[i], parameterInfos[i].ParameterType);
        }
        return ret;
    }

    static object? ConvertJsonToType(object? src, Type target)
    {
        if (src == null) return null;
        var token = src as Newtonsoft.Json.Linq.JToken;
        if (token != null)
        {
            return token.ToObject(target);
        }

        // primitive conversion attempt
        try
        {
            if (target.IsAssignableFrom(src.GetType())) return src;
            if (typeof(IConvertible).IsAssignableFrom(target))
            {
                return Convert.ChangeType(src, target);
            }

            // fallback: serialize src and deserialize into target
            var json = JsonConvert.SerializeObject(src);
            return JsonConvert.DeserializeObject(json, target);
        }
        catch
        {
            // last resort
            var json = JsonConvert.SerializeObject(src);
            return JsonConvert.DeserializeObject(json, target);
        }
    }
}
