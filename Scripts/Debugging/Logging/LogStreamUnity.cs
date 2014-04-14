using System;
using System.Diagnostics;
using UnityEngine;

namespace ModestTree
{
    public class LogStreamUnity : ILogStream
    {
        public void RecordLog(LogLevel level, int category, string message, StackTrace stackTrace)
        {
            switch (level)
            {
                case LogLevel.Debug:
                {
                    // do nothing
                    break;
                }
                case LogLevel.Info:
                case LogLevel.Trace:
                {
                    UnityEngine.Debug.Log(message);
                    break;
                }
                case LogLevel.Warn:
                {
                    UnityEngine.Debug.LogWarning(message);
                    break;
                }
                case LogLevel.Error:
                {
                    UnityEngine.Debug.LogError(message + "\n" + stackTrace.ToString());
                    break;
                }
                default:
                {
                    Assert.That(false);
                    break;
                }
            }
        }
    }
}
