using System;
using ModestTree;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public class ZenjectException : Exception
    {
        public ZenjectException(string message, params object[] parameters)
            : base(Assert.SafeFormatString(message, parameters))
        {
        }

        public ZenjectException(
            Exception innerException, string message, params object[] parameters)
            : base(Assert.SafeFormatString(message, parameters), innerException)
        {
        }
    }
}
