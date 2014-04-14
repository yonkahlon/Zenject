using System;
using System.Diagnostics;
using UnityEngine;

namespace ModestTree
{
    public interface ILogStream
    {
        void RecordLog(
            LogLevel level, int category,
            string message, StackTrace stackTrace);
    }
}
