using System;

namespace ModestTree.Zenject
{
    public class ZenjectGeneralException : Exception
    {
        public ZenjectGeneralException(
            string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ZenjectGeneralException(string message)
            : base(message)
        {
        }
    }

    public class ZenjectResolveException : Exception
    {
        public ZenjectResolveException(
            string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ZenjectResolveException(string message)
            : base(message)
        {
        }
    }
}

