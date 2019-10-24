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
            bool notValid = arg == null;

            switch (arg)
            {
                case string b when string.IsNullOrEmpty(b):
                case IEnumerable<int> c when !c.Any():
                case IEnumerable<float> d when !d.Any():
                case IEnumerable<double> e when !e.Any():
                case IEnumerable<object> enumerable when !enumerable.Any():
                    notValid = true;
                    break;
            }

            if (notValid)
                throw new ArgumentNullException(nameof(argName), message ?? "Parameter cannot be null or empty");
        }
    }
}
