using System;

namespace SevenTiny.Bantina.Validation
{
    public static class Ensure
    {
        [System.Diagnostics.DebuggerStepThrough]
        public static void ArgumentNotNullOrEmpty(object arg, string argName, string message = null)
        {
            if (FormatValidationExtension.IsNullOrEmpty(arg))
                throw new ArgumentNullException(nameof(argName), message ?? "Parameter cannot be null or empty.");
        }
    }
}
