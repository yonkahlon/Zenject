using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

namespace ModestTree
{
    public static class UnityStackTraceConverter
    {
        static Regex _unityRegex;

        static UnityStackTraceConverter()
        {
            _unityRegex = new Regex(
                @"([^:(]*)[:|.]([^. :(]+) ?\(([^()]*)\) \(at ([^(]*):(\d*)\)$",
                RegexOptions.Singleline);
        }

        public static StackTrace Convert(string unityTrace)
        {
            return Convert(Application.dataPath, unityTrace);
        }

        public static StackTrace Convert(string dataPath, string unityTrace)
        {
            var allLines = Regex.Split(unityTrace, "\r\n|\r|\n").ToList();
            var frames = new List<StackFrame>();

            foreach (var line in allLines)
            {
                var frame = ParseLine(dataPath, line);

                if (frame != null && !StackAnalyzer.ShouldFilter(frame))
                {
                    frames.Add(frame);
                }
            }

            return new StackTrace(frames);
        }

        static StackFrame ParseLine(string dataPath, string stackLine)
        {
            if (stackLine.Length == 0)
            {
                return null;
            }

            var match = _unityRegex.Match(stackLine);

            if (!match.Success)
            {
                // This would be the case for third party libraries for which we don't have source info
                // And therefore don't print file or line numbers
                return null;
            }

            var className = match.Groups[1].Value;
            var functionName = match.Groups[2].Value;
            var paramListStr = match.Groups[3].Value;
            var fileName = match.Groups[4].Value;
            var lineNoStr = match.Groups[5].Value;

            Assert.That(fileName.Length > 0);

            int lineNo;
            bool success = Int32.TryParse(lineNoStr, out lineNo);
            Assert.That(success);

            if (!Path.IsPathRooted(fileName))
            {
                fileName = Path.Combine(dataPath, "../" + fileName);
            }

            MethodBase method = null;

            var type = TypeUtil.SearchForType(className);

            if (type == null)
            {
                // Note that this log statement probably only works for tests since this is likely called with Log.IsEnabled == false
                Log.Warn("UnityStackTraceConverter: Unable to find class with name '" + className + "'");
            }
            else
            {
                try
                {
                    method = type.GetMethod(functionName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                }
                catch (AmbiguousMatchException)
                {
                    // Try filtering by the parameter list
                    var paramStrings = paramListStr.Split(',').Select(s => s.Trim());
                    var paramTypes = paramStrings.Select(s => TypeUtil.SearchForTypeByClassName(s));

                    if (!paramTypes.Contains(null))
                    {
                        method = type.GetMethod(functionName, paramTypes.ToArray());
                    }

                    if (method == null)
                    {
                        // Note that this log statement probably only works for tests since this is likely called with Log.IsEnabled == false
                        Log.Warn("UnityStackTraceConverter: Unable to find function with name '" + functionName + "'");
                    }
                }
            }

            return new StackFrame(fileName, lineNo, 1, method);
        }
    }
}
