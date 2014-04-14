using System;

namespace ModestTree
{
    public class AssertHandlerLogger : IAssertHandler
    {
        public void OnAssert(string message, StackTrace stackTrace)
        {
            Log.Error(message, stackTrace);
        }
    }
}

