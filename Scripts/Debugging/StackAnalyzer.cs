using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using SystemStackTrace = System.Diagnostics.StackTrace;

namespace ModestTree
{
    public static class StackAnalyzer
    {
        static Regex _ignoreMethodRegex;
        static Regex _ignoreFileRegex;

        static StackAnalyzer()
        {
            _ignoreMethodRegex = BuildRegex(Config.GetAll<string>("StackAnalyzer/MethodNameIgnorePatterns/Pattern"));
            _ignoreFileRegex = BuildRegex(Config.GetAll<string>("StackAnalyzer/FileNameIgnorePatterns/Pattern"));
        }

        static Regex BuildRegex(List<string> patterns)
        {
            if (patterns.Count == 0)
            {
                return null;
            }

            string fullPattern = "";

            foreach (var pattern in patterns)
            {
                if (fullPattern.Length > 0)
                {
                    fullPattern += "|";
                }

                fullPattern += pattern;
            }

            return new Regex(fullPattern, RegexOptions.Singleline);
        }

        public static StackTrace GetStackTraceFromException(Exception e)
        {
            var trace = new StackTrace(e);
            FilterStackTrace(trace);
            return trace;
        }

        public static StackTrace GetStackTrace()
        {
            var fullTrace = new StackTrace(new SystemStackTrace(true));
            FilterStackTrace(fullTrace);
            return fullTrace;
        }

        static void FilterStackTrace(StackTrace trace)
        {
            foreach (var frame in trace.Frames.ToList())
            {
                if (ShouldFilter(frame))
                {
                    trace.Frames.Remove(frame);
                }
            }
        }

        public static bool ShouldFilter(StackFrame frame)
        {
            if (frame.FileName != null && _ignoreFileRegex != null && _ignoreFileRegex.Match(frame.FileName).Success)
            {
                return true;
            }

            return frame.Method != null && _ignoreMethodRegex != null && _ignoreMethodRegex.Match(frame.FullMethodName).Success;
        }
    }
}

