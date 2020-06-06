using System;

namespace SevenTiny.Bantina.Validation
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class Ensure
    {
        /// <summary>
        /// vefify argumetn is not null or empty
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="argName"></param>
        /// <param name="message"></param>
        public static void ArgumentNotNullOrEmpty(object arg, string argName, string message = null)
        {
            if (FormatValidationExtension.IsNullOrEmpty(arg))
                throw new ArgumentNullException(nameof(argName), message ?? "Parameter cannot be null or empty.");
        }

        /// <summary>
        /// assert verification
        /// </summary>
        /// <param name="assert">expect value</param>
        /// <param name="exception">exception to throw</param>
        public static void Assert(bool assert, Exception exception)
        {
            if (!assert)
                throw exception;
        }
    }
}
