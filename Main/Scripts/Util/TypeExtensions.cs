using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace ModestTree
{
    public static class TypeExtensions
    {
        public static bool DerivesFrom<T>(this Type a)
        {
            return DerivesFrom(a, typeof(T));
        }

        // This seems easier to think about than IsAssignableFrom
        public static bool DerivesFrom(this Type a, Type b)
        {
            return b != a && b.IsAssignableFrom(a);
        }

        public static bool DerivesFromOrEqual<T>(this Type a)
        {
            return DerivesFromOrEqual(a, typeof(T));
        }

        public static bool DerivesFromOrEqual(this Type a, Type b)
        {
            return b == a || b.IsAssignableFrom(a);
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        // Returns name without generic arguments
        public static string GetSimpleName(this Type type)
        {
            var name = type.Name;

            var quoteIndex = name.IndexOf("`");

            if (quoteIndex == -1)
            {
                return name;
            }

            // Remove the backtick
            return name.Substring(0, quoteIndex);
        }

        public static IEnumerable<Type> GetParentTypes(this Type type)
        {
            if (type.BaseType == typeof(object))
            {
                yield break;
            }

            yield return type.BaseType;

            foreach (var ancestor in type.BaseType.GetParentTypes())
            {
                yield return ancestor;
            }
        }

        public static string NameWithParents(this Type type)
        {
            var typeList = type.GetParentTypes().Prepend(type).Select(x => x.Name()).ToArray();
            return string.Join(":", typeList);
        }

        public static bool IsClosedGenericType(this Type type)
        {
            return type.IsGenericType && type != type.GetGenericTypeDefinition();
        }

        public static bool IsOpenGenericType(this Type type)
        {
            return type.IsGenericType && type == type.GetGenericTypeDefinition();
        }
    }
}
