using System;

namespace Bamboo.ScriptEngine.Core
{
    public class ScriptEngineException : Exception
    {
        public ScriptEngineException(string message)
            : base(message)
        {
        }

        public ScriptEngineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
