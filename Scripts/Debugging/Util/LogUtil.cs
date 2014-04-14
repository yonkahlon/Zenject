using System;

namespace ModestTree
{
    public static class LogUtil
    {
        // This method can be used in root-level methods that are called
        // by unity to catch exceptions before they get to unity and
        // properly extract the stack trace even on non-dev builds
        //
        // When the app is run in debug mode and an exception occurs,
        // Unity will catch the exception and trigger LogError, which
        // in turn will trigger our own Logger which will use the stack
        // trace information provided by unity
        // However, when the app is run in non-dev mode, the stack trace
        // that unity provides is empty.  It is still very useful to get
        // stack trace info during non dev builds however, hence this method.
        public static void CallAndCatchExceptions(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (Assert.IsEnabled)
                {
                    // Assert so that we can stack trace information in the log on non-dev builds
                    Assert.TriggerFromException(e);
                }
                else
                {
                    throw e;
                }
            }
        }
    }
}
