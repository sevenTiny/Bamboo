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
            if (ParameterValidationHelper.IsNullOrEmpty(arg))
                throw new ArgumentNullException(argName, message ?? "Parameter cannot be null or empty.");
        }

        /// <summary>
        /// vefify argumetn is not null or whitespace
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="argName"></param>
        /// <param name="message"></param>
        public static void ArgumentNotNullOrWhiteSpace(string arg, string argName, string message = null)
        {
            if (ParameterValidationHelper.IsNullOrWhiteSpace(arg))
                throw new ArgumentNullException(argName, message ?? "Parameter cannot be null or empty.");
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
