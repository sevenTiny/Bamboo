using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Bantina.Validation
{
    public static class Ensure
    {
        [System.Diagnostics.DebuggerStepThrough]
        public static void ArgumentNotNullOrEmpty(object arg, string argName, string message = null)
        {
            bool valid = true;

            if (arg is string a && string.IsNullOrEmpty(a))
                valid = false;

            else if (arg is IEnumerable<int> enumerable_int && (enumerable_int == null || !enumerable_int.Any()))
                valid = false;

            else if (arg is IEnumerable<float> enumerable_float && (enumerable_float == null || !enumerable_float.Any()))
                valid = false;

            else if (arg is IEnumerable<double> enumerable_double && (enumerable_double == null || !enumerable_double.Any()))
                valid = false;

            else if (arg is IEnumerable<object> enumerable && (enumerable == null || !enumerable.Any()))
                valid = false;

            else if (arg == null)
                valid = false;

            if (!valid)
                throw new ArgumentNullException(nameof(argName), message ?? "Parameter cannot be null or empty");
        }
    }
}
