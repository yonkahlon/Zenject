using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace ModestTree
{
    public interface IAssertHandler
    {
        void OnAssert(string message, StackTrace stackTrace);
    }

    public static class Assert
    {
        static bool _isAsserting = false;
        static List<IAssertHandler> _handlers = new List<IAssertHandler>();

        static Assert()
        {
            // Load handlers
            foreach (var handlerTypeName in Config.GetAll<string>("AssertHandlers/" + Config.GetBuildConfiguration() + "/Handler"))
            {
                var type = Type.GetType(handlerTypeName);

                if (type == null)
                {
                    throw new ConfigException("Invalid assert handler given with type name '" + handlerTypeName + "'");
                }

                if (!typeof(IAssertHandler).IsAssignableFrom(type))
                {
                    throw new ConfigException("Invalid assert handler with type '" + type.FullName + "'. Should derive from IAssertHandler");
                }

                var instance = (IAssertHandler)Activator.CreateInstance(type);
                _handlers.Add(instance);
            }

            if (_handlers.Count == 0)
            {
                _handlers.Add(new AssertHandlerLogger());
                Log.Warn("No assert handlers found for build config '" + Config.GetBuildConfiguration() + "'.  Added simple unity logging for asserts.");
            }
        }

        public static IEnumerable<IAssertHandler> Handlers
        {
            get
            {
                return _handlers;
            }
        }

        public static bool IsEnabled
        {
            get
            {
                // TODO: Compile out asserts for non-dev builds
                return true;
            }
        }

        public static void RemoveHandler(IAssertHandler handler)
        {
            bool removed = _handlers.Remove(handler);
            Assert.That(removed);
        }

        public static void AddHandler(IAssertHandler handler)
        {
            Assert.That(!_handlers.Contains(handler));
            _handlers.Add(handler);
        }

        //[Conditional("UNITY_EDITOR")]
        public static void That(bool condition)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit!");
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsType<T>(object obj)
        {
            IsType<T>(obj, "");
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsType<T>(object obj, string message)
        {
            if (!(obj is T))
            {
                TriggerAssert("Assert Hit! Wrong type found. Expected '"+ typeof(T).Name + "' but found '" + obj.GetType().Name + "'. " + message);
            }
        }

        // Use AssertEquals to get better error output (with values)
        //[Conditional("UNITY_EDITOR")]
        public static void IsEqual(object left, object right)
        {
            IsEqual(left, right, "");
        }

        // Use AssertEquals to get better error output (with values)
        //[Conditional("UNITY_EDITOR")]
        public static void IsEqual(object left, object right, Func<string> messageGenerator)
        {
            if (!object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                TriggerAssert("Assert Hit! Expected '" + right.ToString() + "' but found '" + left.ToString() + "'. " + messageGenerator());
            }
        }

        // Use AssertEquals to get better error output (with values)
        //[Conditional("UNITY_EDITOR")]
        public static void IsEqual(object left, object right, string message)
        {
            if (!object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                TriggerAssert("Assert Hit! Expected '" + right.ToString() + "' but found '" + left.ToString() + "'. " + message);
            }
        }

        // Use Assert.IsNotEqual to get better error output (with values)
        //[Conditional("UNITY_EDITOR")]
        public static void IsNotEqual(object left, object right)
        {
            IsNotEqual(left, right, "");
        }

        // Use Assert.IsNotEqual to get better error output (with values)
        //[Conditional("UNITY_EDITOR")]
        public static void IsNotEqual(object left, object right, Func<string> messageGenerator)
        {
            if(object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                TriggerAssert("Assert Hit! Expected '" + right.ToString() + "' to differ from '" + left.ToString() + "'. " + messageGenerator());
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsNull(object val)
        {
            if (val != null)
            {
                TriggerAssert("Assert Hit! Expected null pointer but instead found '" + val.ToString() + "'");
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object val)
        {
            if (val == null)
            {
                TriggerAssert("Assert Hit! Found null pointer when value was expected");
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object val, string message)
        {
            if (val == null)
            {
                TriggerAssert("Assert Hit! Found null pointer when value was expected. " + message);
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void IsNotNull(object val, string message, params object[] parameters)
        {
            if (val == null)
            {
                TriggerAssert("Assert Hit! Found null pointer when value was expected. " + StringUtil.Format(message, parameters));
            }
        }

        // Use Assert.IsNotEqual to get better error output (with values)
        //[Conditional("UNITY_EDITOR")]
        public static void IsNotEqual(object left, object right, string message)
        {
            if (object.Equals(left, right))
            {
                left = left ?? "<NULL>";
                right = right ?? "<NULL>";
                TriggerAssert("Assert Hit! Expected '" + right.ToString() + "' to differ from '" + left.ToString() + "'. " + message);
            }
        }

        // Pass a function instead of a string for cases that involve a lot of processing to generate a string
        // This way the processing only occurs when the assert fails
        //[Conditional("UNITY_EDITOR")]
        public static void That(bool condition, Func<string> messageGenerator)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit! " + messageGenerator());
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void That(
            bool condition, string message, params object[] parameters)
        {
            if (!condition)
            {
                TriggerAssert("Assert hit! " + StringUtil.Format(message, parameters));
            }
        }

        //[Conditional("UNITY_EDITOR")]
        public static void WithStackTrace(bool condition, string message, StackTrace trace)
        {
            if (!condition)
            {
                TriggerAssert(message, trace);
            }
        }

        //[Conditional("UNITY_EDITOR")]
        static void TriggerAssert(string message)
        {
            TriggerAssert(message, StackAnalyzer.GetStackTrace());
        }

        //[Conditional("UNITY_EDITOR")]
        static void TriggerAssert(string message, StackTrace stackTrace)
        {
            if (!_isAsserting)
            {
                try
                {
                    // Avoid infinite loops if one of the event handlers assert
                    _isAsserting = true;

                    foreach (var handler in _handlers.ToList())
                    {
                        handler.OnAssert(message, stackTrace);
                    }
                }
                finally
                {
                    // Make sure this gets unset in case an exception is thrown
                    _isAsserting = false;
                }
            }
        }

        public static void TriggerFromException(Exception e)
        {
            TriggerAssert(
                e.GetFullMessage(),
                StackAnalyzer.GetStackTraceFromException(e));
        }
    }
}
