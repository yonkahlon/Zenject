using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

#if !NOT_UNITY && UNITY_EDITOR
using UnityEditor;
#endif

namespace ModestTree
{
    public enum LogLevel
    {
        // Order is important here
        Debug,
        Info,
        Trace,
        Warn,
        Error,
    }

    public static class Log
    {
        public static event Action CategoryAdded = delegate {};

        static List<ILogStream> _streams = new List<ILogStream>();
        static LogLevel _minLogLevel = LogLevel.Info;
        static LogLevel _minStackTraceLevel = LogLevel.Trace;

        static Dictionary<string, int> _categoryIds = new Dictionary<string, int>();
        static int _categoryFilter = (int)0xFFFFFF;

        const string UncategorizedLabel = "Uncategorized";

        const string SettingIdCategoryFilter = "ModestTree.Log.CategoryFilter";
        const string SettingIdMinLogLevel = "ModestTree.Log.MinLogLevel";
        const string SettingIdMinStackTraceLevel = "ModestTree.Log.MinStackTraceLevel";
        const string SettingIdNumCategories = "ModestTree.Log.NumCategories";
        const string SettingIdCategoryLabels = "ModestTree.Log.Category.";

        static bool _enabled = true;
        static bool _isLoggingFromUnity;

        static Log()
        {
            _enabled = false;

#if !NOT_UNITY && UNITY_EDITOR
            InitFromEditorPrefs();
#else
            InitFromConfig();
#endif
            InitStreams();

            _enabled = true;
        }

        public static IEnumerable<ILogStream> Streams
        {
            get
            {
                return _streams;
            }
        }

        public static int CategoryFilter
        {
            get
            {
                return _categoryFilter;
            }
            set
            {
                _categoryFilter = value;
#if !NOT_UNITY && UNITY_EDITOR
                EditorPrefs.SetInt(SettingIdCategoryFilter, value);
#endif
            }
        }

        public static IEnumerable<string> CategoryLabels
        {
            get
            {
                return _categoryIds.Keys;
            }
        }

        public static LogLevel MinimumLogLevel
        {
            get
            {
                return _minLogLevel;
            }
            set
            {
                _minLogLevel = value;
#if !NOT_UNITY && UNITY_EDITOR
                EditorPrefs.SetInt(SettingIdMinLogLevel, (int)value);
#endif
            }
        }

        public static LogLevel MinimumStackTraceLevel
        {
            get
            {
                return _minStackTraceLevel;
            }
            set
            {
                _minStackTraceLevel = value;
#if !NOT_UNITY && UNITY_EDITOR
                EditorPrefs.SetInt(SettingIdMinStackTraceLevel, (int)value);
#endif
            }
        }

        public static bool IsEnabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                // Do this since there's a few places where we set this to false,
                // then set it back to true, which wouldn't work if done twice
                Assert.That(_enabled != value);

                _enabled = value;
            }
        }

#if !NOT_UNITY && UNITY_EDITOR
        static void InitFromEditorPrefs()
        {
            _minLogLevel = (LogLevel)EditorPrefs.GetInt(SettingIdMinLogLevel, (int)LogLevel.Info);
            _minStackTraceLevel = (LogLevel)EditorPrefs.GetInt(SettingIdMinStackTraceLevel, (int)LogLevel.Trace);

            var numCategories = EditorPrefs.GetInt(SettingIdNumCategories, 1);

            _categoryIds.Clear();
            _categoryIds.Add(UncategorizedLabel, 0);

            for (int i = 1; i < numCategories; i++)
            {
                var categoryLabel = EditorPrefs.GetString(SettingIdCategoryLabels + i);
                _categoryIds.Add(categoryLabel, i);
            }

            _categoryFilter = EditorPrefs.GetInt(SettingIdCategoryFilter, (int)0xFFFFFF);
        }
#endif

        static void InitFromConfig()
        {
            ClearCategories();

            var prefix = "Logging/Settings/" + Config.GetBuildConfiguration() + "/";

            _minLogLevel = Config.Get<LogLevel>(prefix + "MinLogLevel");
            _minStackTraceLevel = Config.Get<LogLevel>(prefix + "MinStackTraceLevel");
        }

        public static void ClearCategories()
        {
            _categoryFilter = (int)0xFFFFFF;

            _categoryIds.Clear();
            _categoryIds.Add(UncategorizedLabel, 0);

#if !NOT_UNITY && UNITY_EDITOR
            EditorPrefs.SetInt(SettingIdNumCategories, 1);
            EditorPrefs.SetInt(SettingIdCategoryFilter, _categoryFilter);
#endif
        }

        public static string GetCategoryLabel(int categoryId)
        {
            var label = _categoryIds.Where(x => PowerOf2(x.Value) == categoryId).Select(x => x.Key).SingleOrDefault();

            if (label == null)
            {
                return "<unknown category>";
            }

            return label;
        }

        static int GetCategoryId(string categoryStr)
        {
            if (string.IsNullOrEmpty(categoryStr))
            {
                categoryStr = UncategorizedLabel;
            }

            int categoryNum;

            if (!_categoryIds.TryGetValue(categoryStr, out categoryNum))
            {
                categoryNum = _categoryIds.Count;
                _categoryIds.Add(categoryStr, categoryNum);

#if !NOT_UNITY && UNITY_EDITOR
                EditorPrefs.SetString(SettingIdCategoryLabels + categoryNum, categoryStr);
                EditorPrefs.SetInt(SettingIdNumCategories, categoryNum+1);
#endif
                CategoryAdded();
            }

            return PowerOf2(categoryNum);
        }

        static int PowerOf2(int power)
        {
            int x = 1;

            for (int i = 0; i < power; i++)
            {
                x = (x << 1);
            }

            return x;
        }

        static void InitStreams()
        {
            foreach (var streamTypeName in Config.GetAll<string>("Logging/LogStreams/" + Config.GetBuildConfiguration() + "/Stream"))
            {
                var type = Type.GetType(streamTypeName);

                if (!type.DerivesFrom(typeof(ILogStream)))
                {
                    throw new ConfigException("Invalid log stream with type '" + streamTypeName + "'");
                }

                var instance = (ILogStream)Activator.CreateInstance(type);

                if (instance == null)
                {
                    throw new ConfigException("Invalid log stream with type '" + streamTypeName + "'");
                }

                _streams.Add(instance);
            }

            if (_streams.Count == 0)
            {
                _streams.Add(new LogStreamUnity());
                Log.Warn("No log streams found, using Unity's default");
            }
        }

        public static void AddStream(ILogStream strm)
        {
            Assert.That(!_streams.Contains(strm));

            _streams.Add(strm);
        }

        public static void RemoveStream(ILogStream strm)
        {
            Assert.That(_streams.Contains(strm));

            _streams.Remove(strm);
        }

        public static void OnUnityLog(string msg, string stackTraceStr, LogType type)
        {
            if (!Log.IsEnabled || _isLoggingFromUnity)
            {
                return;
            }

            try
            {
                _isLoggingFromUnity = true; // avoid infinite loops.  We can allow direct logs that aren't through unity however

                switch (type)
                {
                    case LogType.Log:
                    {
                        InternalWrite(new LogMessageInfo(LogLevel.Info) { Category = "Unity", Message = msg });
                        break;
                    }
                    case LogType.Warning:
                    {
                        StackTrace stackTrace = null;

                        if (ShouldGenerateStackTrace(LogLevel.Warn))
                        {
                            stackTrace = UnityStackTraceConverter.Convert(stackTraceStr);
                        }

                        InternalWrite(new LogMessageInfo(LogLevel.Warn) { Category = "Unity", Message = msg, StackTrace = stackTrace });
                        break;
                    }
                    case LogType.Assert:
                    case LogType.Exception:
                    case LogType.Error:
                    {
                        StackTrace stackTrace = UnityStackTraceConverter.Convert(stackTraceStr);

                        try
                        {
                            // Now ignore all logs, even those that call Log.Info() directly
                            // to avoid logging it twice from the assert
                            Log.IsEnabled = false;
                            Assert.WithStackTrace(false, msg, stackTrace);
                        }
                        finally
                        {
                            // Ensure this always gets reset using try-finally
                            Log.IsEnabled = true;
                        }

                        // The assert would normally trigger this anyway but won't in this case because of Log.IsEnabled set to false
                        InternalWrite(new LogMessageInfo(LogLevel.Error) { Category = "Unity", Message = msg, StackTrace = stackTrace });
                        break;
                    }
                    default:
                    {
                        Assert.That(false);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                // Should never happen
                InternalWrite(
                    new LogMessageInfo(LogLevel.Error)
                    {
                        Category = "Unity",
                        Message = "Error in logging system!",
                        Exception = e,
                    });
            }
            finally
            {
                // Use try-finally to guarantee this gets reset even in the case of an exception
                _isLoggingFromUnity = false;
            }
        }

        static bool IsLogLevelEnabled(LogLevel level)
        {
            return ((int)level >= (int)_minLogLevel);
        }

        static bool ShouldGenerateStackTrace(LogLevel level)
        {
            return ((int)level >= (int)_minStackTraceLevel);
        }

        static StackTrace GetFullStackTrace(LogMessageInfo messageInfo)
        {
            var stackTrace = messageInfo.StackTrace;

            if (stackTrace == null)
            {
                stackTrace = StackAnalyzer.GetStackTrace();
            }

            if (messageInfo.Exception != null)
            // Add exception info to message
            {
                var exceptionTrace = StackAnalyzer.GetStackTraceFromException(messageInfo.Exception);
                stackTrace = new StackTrace(
                    exceptionTrace.Frames.Append(stackTrace.Frames).ToList());
            }

            return stackTrace;

        }

        static string GetFullMessage(LogMessageInfo messageInfo, StackTrace stackTrace)
        {
            string message = StringUtil.Format(messageInfo.Message, messageInfo.FormatArguments);

            if (messageInfo.Level == LogLevel.Trace && message.Length == 0 && stackTrace != null && stackTrace.Frames.Any())
            {
                var topFrame = stackTrace.Frames[0];
                message = topFrame.FullMethodName;
            }

            // Add exception messages as well
            if (messageInfo.Exception != null)
            {
                message += messageInfo.Exception.GetFullMessage();
            }

            return message;
        }

        static bool IsCategoryEnabled(int category)
        {
            return (_categoryFilter & category) > 0;
        }

        public static bool IsCategoryEnabled(string categoryStr)
        {
            int categoryId = GetCategoryId(categoryStr);
            return IsCategoryEnabled(categoryId);
        }

        static string GenerateCategoryStringFromStackFrame(StackFrame frame)
        {
            if (frame.Class == null)
            {
                return "";
            }

            return frame.Class.Namespace;
        }

        static void InternalWrite(LogMessageInfo messageInfo)
        {
            Assert.That(messageInfo.Category != null);
            Assert.That(messageInfo.Message != null);

            if (!IsLogLevelEnabled(messageInfo.Level))
            {
                return;
            }

            bool shouldGenerateStackTrace = ShouldGenerateStackTrace(messageInfo.Level);
            StackTrace stackTrace = null;
            string categoryStr = messageInfo.Category;

            if (string.IsNullOrEmpty(categoryStr) && shouldGenerateStackTrace)
            {
                stackTrace = GetFullStackTrace(messageInfo);

                if (stackTrace.TopFrame != null)
                {
                    categoryStr = GenerateCategoryStringFromStackFrame(stackTrace.TopFrame);
                }
            }

            int categoryId = GetCategoryId(categoryStr);

            if (IsCategoryEnabled(categoryId))
            {
                // Generating stack trace is a costly operation so only do it for certain levels

                if (stackTrace == null && shouldGenerateStackTrace)
                {
                    stackTrace = GetFullStackTrace(messageInfo);
                }

                string message = GetFullMessage(messageInfo, stackTrace);

                foreach (var strm in _streams.ToList())
                {
                    strm.RecordLog(messageInfo.Level, categoryId, message, stackTrace);
                }
            }
        }

        public static void EnableHijackUnityLog()
        {
#if !NOT_UNITY
            Application.RegisterLogCallback(OnUnityLog);
#endif
        }

        static void Write(LogMessageInfo messageInfo)
        {
            if (!Log.IsEnabled)
            {
                // avoid infinite loops
                return;
            }

            // Ensure this is always the case
            // in case other code tries to steal it
            EnableHijackUnityLog();

            try
            {
                Log.IsEnabled = false;
                InternalWrite(messageInfo);
            }
            finally
            {
                // Use try-finally in case an exception is thrown
                Log.IsEnabled = true;
            }
        }

        /////////////

        public static void Debug(string message)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Debug)
                    {
                        Message = message,
                    });
            }
        }

        public static void Debug(string message, StackTrace trace)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Debug)
                    {
                        Message = message,
                        StackTrace = trace,
                    });
            }
        }

        public static void DebugFormat(string message, params object[] param)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Debug)
                    {
                        Message = message,
                        FormatArguments = param,
                    });
            }
        }

        /////////////

        public static void Info(string message)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Info)
                    {
                        Message = message,
                    });
            }
        }

        public static void Info(string message, StackTrace trace)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Info)
                    {
                        Message = message,
                        StackTrace = trace,
                    });
            }
        }

        public static void InfoFormat(string message, params object[] param)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Info)
                    {
                        Message = message,
                        FormatArguments = param,
                    });
            }
        }

        /////////////

        public static void Trace()
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Trace)
                    {
                    });
            }
        }

        public static void Trace(string message)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Trace)
                    {
                        Message = message,
                    });
            }
        }

        public static void Trace(string message, StackTrace trace)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Trace)
                    {
                        Message = message,
                        StackTrace = trace,
                    });
            }
        }

        public static void TraceFormat(string message, params object[] param)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Trace)
                    {
                        Message = message,
                        FormatArguments = param,
                    });
            }
        }

        /////////////

        public static void Warn(string message)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Warn)
                    {
                        Message = message,
                    });
            }
        }

        public static void Warn(string message, Exception e)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Warn)
                    {
                        Message = message,
                        Exception = e,
                    });
            }
        }

        public static void Warn(string message, StackTrace stackTrace)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Warn)
                    {
                        Message = message,
                        StackTrace = stackTrace,
                    });
            }
        }

        public static void WarnFormat(string message, params object[] param)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Warn)
                    {
                        Message = message,
                        FormatArguments = param,
                    });
            }
        }

        /////////////

        public static void Error(string message)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Error)
                    {
                        Message = message,
                    });
            }
        }

        public static void Error(Exception e)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Error)
                    {
                        Message = "", // Will use exception message below
                        Exception = e,
                    });
            }
        }

        public static void Error(string message, Exception e)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Error)
                    {
                        Message = message,
                        Exception = e,
                    });
            }
        }

        public static void Error(string message, StackTrace stackTrace)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Error)
                    {
                        Message = message,
                        StackTrace = stackTrace,
                    });
            }
        }

        public static void ErrorFormat(string message, params object[] param)
        {
            if (_enabled)
            {
                Write(
                    new LogMessageInfo(LogLevel.Error)
                    {
                        Message = message,
                        FormatArguments = param,
                    });
            }
        }

        class LogMessageInfo
        {
            public LogLevel Level;
            public string Message = "";
            public string Category = "";
            public StackTrace StackTrace;
            public Exception Exception;
            public object[] FormatArguments = new object[0];

            public LogMessageInfo(LogLevel level)
            {
                Level = level;
            }
        }
    }
}
