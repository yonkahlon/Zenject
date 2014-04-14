using System;
using System.Diagnostics;
using UnityEngine;

namespace ModestTree
{
    public class LogStreamConsole : ILogStream
    {
        public void RecordLog(LogLevel level, int category, string message, StackTrace stackTrace)
        {
            switch (level)
            {
                case LogLevel.Debug:
                case LogLevel.Info:
                case LogLevel.Warn:
                case LogLevel.Trace:
                {
                    Console.Out.WriteLine(message);
                    break;
                }
                case LogLevel.Error:
                {
                    Console.Out.WriteLine(message + "\n" + stackTrace.ToString());
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

