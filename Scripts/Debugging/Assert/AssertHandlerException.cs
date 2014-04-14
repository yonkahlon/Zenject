using System;

namespace ModestTree
{
    // Used when running tests
    public class AssertException : Exception
    {
        public StackTrace ParsedStackTrace
        {
            get
            {
                return _stackTrace;
            }
        }

        StackTrace _stackTrace;

        public AssertException(string message, StackTrace stackTrace)
            : base(message)
        {
            _stackTrace = stackTrace;
        }
    }

    public class AssertHandlerException : IAssertHandler
    {
        public void OnAssert(string message, StackTrace stackTrace)
        {
            throw new AssertException(message, stackTrace);
        }
    }
}
