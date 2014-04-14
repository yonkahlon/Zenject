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
    public class StackFrame
    {
        // might be null
        public readonly string FileName;

        // 0 = unknown
        public readonly int LineNumber;

        // might be null
        public readonly int ColumnNumber;
        public readonly MethodBase Method;

        public StackFrame(
            string fileName, int lineNumber, int colNumber, MethodBase method)
        {
            FileName = fileName;
            LineNumber = lineNumber;
            ColumnNumber = colNumber;
            Method = method;
        }

        public StackFrame(SystemStackFrame frame)
        {
            FileName = frame.GetFileName();
            LineNumber = frame.GetFileLineNumber();
            ColumnNumber = frame.GetFileColumnNumber();
            Method = frame.GetMethod();
        }

        public Type Class
        {
            get
            {
                return Method == null ? null : Method.DeclaringType;
            }
        }

        public string FullMethodName
        {
            get
            {
                if (Method == null)
                {
                    return "<Unknown Method>";
                }

                return (Class == null ? "" : Class.FullName + ".") + Method.Name;
            }
        }

        public string ShortMethodName
        {
            get
            {
                if (Method == null)
                {
                    return "<Unknown Method>";
                }

                return (Class == null ? "" : Class.Name + ".") + Method.Name;
            }
        }

        // Follow the same convention that System.StackFrame uses
        public override string ToString()
        {
            var result = new StringBuilder();

            result.Append("  at ");
            result.Append(FullMethodName);
            result.Append(" in ");
            result.Append(FileName);
            result.Append(":line ");
            result.Append(LineNumber);

            return result.ToString();
        }
    }

}
