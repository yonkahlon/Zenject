#if !NOT_UNITY3D && !(UNITY_WSA && ENABLE_DOTNET)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace SceneTester
{
    // NOTE: This is really meant for use with unity editor scripts
    // not exe builds that you've made yourself
    public static class UnityCustomCommandLineHandler
    {
        const string ArgPrefix = "-CustomArg:";

        static Dictionary<string, string> _customArgs;

        static UnityCustomCommandLineHandler()
        {
            Assert.That(Application.isEditor);

            string[] args = Environment.GetCommandLineArgs();

            _customArgs = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                if (!arg.StartsWith(ArgPrefix))
                {
                    continue;
                }

                var assignStr = arg.Substring(ArgPrefix.Length);

                var equalsPos = assignStr.IndexOf("=");

                if (equalsPos == -1)
                {
                    continue;
                }

                var name = assignStr.Substring(0, equalsPos).Trim();
                var value = assignStr.Substring(equalsPos + 1).Trim();

                if (name.Length > 0 && value.Length > 0)
                {
                    _customArgs[name] = value;
                }
            }
        }

        public static string GetArgument(string name)
        {
            Assert.That(_customArgs.ContainsKey(name), "Could not find custom command line argument '{0}'", name);
            return _customArgs[name];
        }

        public static string TryGetArgument(string name)
        {
            if (!_customArgs.ContainsKey(name))
            {
                return null;
            }

            return _customArgs[name];
        }
    }
}

#endif
