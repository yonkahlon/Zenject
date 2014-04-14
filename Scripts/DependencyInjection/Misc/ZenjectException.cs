using System;

namespace ModestTree.Zenject
{
    public class ZenjectException : Exception
    {
        public ZenjectException(
            string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ZenjectException(string message)
            : base(message)
        {
        }
    }
}
