using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ModestTree;

namespace Zenject
{
    internal static class SignalInternalUtil
    {
        public static string MethodToString(MethodInfo method)
        {
            return "{0}.{1}".Fmt(method.DeclaringType.Name(), method.Name);
        }
    }
}

