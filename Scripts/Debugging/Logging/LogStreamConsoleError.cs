using System;
using System.Diagnostics;
using UnityEngine;

namespace ModestTree
{
    public class LogStreamConsoleError : ILogStream
    {
        public void RecordLog(LogLevel level, int category, string message, StackTrace stackTrace)
        {
            switch (level)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Info:
                case LogLevel.Warn:
                {
                    Console.Error.WriteLine(message);
                    break;
                }
                case LogLevel.Error:
                {
                    Console.Error.WriteLine(message + "\n" + stackTrace.ToString());
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


