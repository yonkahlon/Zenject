using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace ModestTree
{
    public class ConfigException : Exception
    {
        public ConfigException(string message)
            : base(message)
        {
        }

        public ConfigException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public static class Config
    {
        static XmlDocument _xml;
        static XmlDocument _xmlOverride;

        static Config()
        {
            LoadConfigFile();
        }

        static XmlDocument LoadXml(string resourceName)
        {
            try
            {
                var xml = new XmlDocument();

#if TEST_BUILD
                // For non-unity test builds just assume it's in the starting directory
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, resourceName + ".xml");
                xml.Load(path);
#else
                // Use Resources.Load so it works with web builds
                var obj = Resources.Load(resourceName) as TextAsset;
                xml.LoadXml(obj.text);
#endif
                return xml;
            }
            catch (Exception)
            {
            }

            return null;
        }

        static void LoadConfigFile()
        {
            _xml = LoadXml("CoreConfig");
            _xmlOverride = LoadXml("CoreConfigCustom");
        }

        public static string GetBuildConfiguration()
        {
#if TEST_BUILD
            return "TestBuild";
#elif UNITY_EDITOR
            return "UnityEditor";
#elif UNITY_WEBPLAYER
            return "UnityWebPlayer";
#elif UNITY_ANDROID
            return "UnityAndroid";
#else
            return "StandaloneWin32";
#endif
        }

        static bool TryGet<TValue>(XmlDocument xml, string identifier, ref TValue value)
        {
            if (xml == null)
            {
                return false;
            }

            var child = xml.SelectSingleNode("/Config/" + identifier);

            if (child == null)
            {
                return false;
            }

            var asString = child.InnerText.Trim();

            try
            {
                if (typeof(TValue).IsEnum)
                {
                    value = (TValue)Enum.Parse(typeof(TValue), asString);
                }
                else
                {
                    value = (TValue)Convert.ChangeType(asString, typeof(TValue));
                }
            }
            catch(Exception ex)
            {
                throw new ConfigException("Failed during type conversion while loading setting '" + identifier + "'", ex);
            }

            return true;
        }

        public static TValue Get<TValue>(string identifier)
        {
            TValue value = default(TValue);

            if (TryGet(_xmlOverride, identifier, ref value) || TryGet(_xml, identifier, ref value))
            {
                return value;
            }

            throw new ConfigException("Unable to find setting '" + identifier + "'");
        }

        public static TValue Get<TValue>(string identifier, TValue defaultValue)
        {
            var value = default(TValue);
            if (TryGet(_xmlOverride, identifier, ref value) || TryGet(_xml, identifier, ref value))
            {
                return value;
            }

            return defaultValue;
        }

        static List<TValue> GetAll<TValue>(XmlDocument xml, string identifier)
        {
            if (xml == null)
            {
                return new List<TValue>();
            }

            var nodeList = xml.SelectNodes("/Config/" + identifier);

            if (nodeList == null)
            {
                throw new ConfigException("Unable to find config identifier '" + identifier + "'");
            }

            var valueList = new List<TValue>();
            foreach (XmlNode node in nodeList)
            {
                try
                {
                    TValue value = (TValue)Convert.ChangeType(node.InnerText.Trim(), typeof(TValue));
                    valueList.Add(value);
                }
                catch (Exception ex)
                {
                    throw new ConfigException(
                        "Failed during type conversion while loading setting '" + identifier + "'", ex);
                }
            }

            return valueList;
        }

        public static List<TValue> GetAll<TValue>(string identifier)
        {
            return GetAll<TValue>(_xmlOverride, identifier).Concat(GetAll<TValue>(_xml, identifier)).ToList();
        }
    }
}
