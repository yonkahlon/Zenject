using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using SystemStackTrace = System.Diagnostics.StackTrace;
using SystemStackFrame = System.Diagnostics.StackFrame;
using System.Linq;

namespace ModestTree
{
    // This class exists for these reasons:
    // - It's nice to have a mutable stack trace to filter lines
    // - We cannot use System StackTrace when analyzing unity logs received via Application.RegisterLogCallback
    //   and have to convert it to something
    public class StackTrace
    {
        public readonly List<StackFrame> Frames;

        public StackTrace(List<StackFrame> frames)
        {
            Frames = frames;
        }

        public StackTrace(SystemStackTrace trace)
        {
            Frames = trace.GetFrames().Select(x => new StackFrame(x)).ToList();
        }

        public StackTrace(Exception exc)
        {
            Frames = GetFramesForException(exc).ToList();
        }

        IEnumerable<StackFrame> GetFramesForException(Exception exception)
        {
            // new SystemStackTrace apparently does not include inner exceptions
            if (exception.InnerException != null)
            {
                foreach (var frame in GetFramesForException(exception.InnerException))
                {
                    yield return frame;
                }
            }

            var trace = new SystemStackTrace(exception, true);

            foreach (var frame in trace.GetFrames())
            {
                yield return new StackFrame(frame);
            }
        }

        public StackFrame TopFrame
        {
            get
            {
                if (Frames.Any())
                {
                    return Frames[0];
                }

                return null;
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (var frame in Frames)
            {
                result.AppendLine(frame.ToString());
            }

            return result.ToString();
        }

        public string ToStringPretty()
        {
            var result = new StringBuilder();

            foreach (var frame in Frames)
            {
                result.Append(frame.ShortMethodName);

                if (frame.FileName != null)
                {
#if !TEST_BUILD && !UNITY_WEBPLAYER
                    if (PathUtil.IsSubPath(Application.dataPath, frame.FileName))
#endif
                    {
                        result.Append("  (");
#if TEST_BUILD || UNITY_WEBPLAYER
                        result.Append(frame.FileName);
#else
                        result.Append(PathUtil.GetRelativePath(Application.dataPath + "/../", frame.FileName));
#endif
                        result.Append(":" + frame.LineNumber + ")");
                    }
                }

                result.AppendLine();
            }

            return result.ToString();
        }
    }
}
