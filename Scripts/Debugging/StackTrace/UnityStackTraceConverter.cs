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
        static Regex _paramRegex;

        static UnityStackTraceConverter()
        {
            _unityRegex = new Regex(
                @"([^:(]*[^.])[:|.](\.?[^. :(]+) ?\(([^()]*)\) \(at ([^(]*):(\d*)\)$",
                RegexOptions.Singleline);

            _paramRegex = new Regex(
                @"^\s*(\S+)\s*(\S+)\s*$", RegexOptions.Singleline);
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

        static bool ParseParams(string paramListStr, Type forType, out List<Type> paramTypes)
        {
            paramTypes = new List<Type>();

            if (paramListStr.Trim() == "")
            {
                return true;
            }

            var paramsWithName = paramListStr.Split(',');

            foreach (var paramWithName in paramsWithName)
            {
                var match = _paramRegex.Match(paramWithName);

                if (!match.Success)
                {
                    return false;
                }

                string typeName = match.Groups[1].Value;

                var paramType = TypeUtil.SearchForType(typeName);

                if (paramType == null)
                {
                    var classNameOnly = typeName.Substring(typeName.LastIndexOf('.')+1);
                    paramType = TypeUtil.SearchForType(forType.FullName + "+" + classNameOnly);

                    if (paramType == null)
                    {
                        // System types are abbrevriated for some reason
                        paramType = TypeUtil.SearchForTypeByClassName(typeName);

                        if (paramType == null)
                        {
                            return false;
                        }
                    }
                }

                paramTypes.Add(paramType);
            }

            return true;
        }

        static MethodBase FindMethod(string className, string functionName, string paramListStr)
        {
            var type = TypeUtil.SearchForType(className);

            if (type == null)
            {
                return null;
            }

            // .NET framework actually does include the dot here when doing Method.Name but only for constructors
            bool isConstructor = (functionName == ".ctor");

            var bindingFlags = (BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (!isConstructor)
            {
                try
                {
                    var method = type.GetMethod(functionName, bindingFlags);

                    if (method != null)
                    {
                        return method;
                    }
                }
                catch (AmbiguousMatchException)
                {
                    // Try filtering by the parameter list below instead
                }
            }

            List<Type> paramTypes;
            if (!ParseParams(paramListStr, type, out paramTypes))
            {
                return null;
            }

            if (isConstructor)
            {
                return type.GetConstructor(bindingFlags, null, paramTypes.ToArray(), null);
            }

            return type.GetMethod(functionName, bindingFlags, null, paramTypes.ToArray(), null);
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
            var filePath = match.Groups[4].Value;
            var lineNoStr = match.Groups[5].Value;

            filePath = filePath.Replace('/', '\\');

            Assert.That(filePath.Length > 0);

            int lineNo;
            bool success = Int32.TryParse(lineNoStr, out lineNo);
            Assert.That(success);

            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(dataPath, "../" + filePath);
            }

            var method = FindMethod(className, functionName, paramListStr);

            if (method == null)
            {
                // TODO: Add support for generic methods
                // In the meantime just ignore these cases
                //Log.WarnFormat(
                    //"UnityStackTraceConverter: Unable to find method with name '{0}' for class '{1}' with params '{2}'",
                    //functionName, className, paramListStr);
            }

            return new StackFrame(filePath, lineNo, 1, method);
        }
    }
}
