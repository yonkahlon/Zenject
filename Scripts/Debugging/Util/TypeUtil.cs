using System;
using System.Linq;

namespace ModestTree
{
    public static class TypeUtil
    {
        // This might fail in cases where an assembly is referenced but not loaded
        // Note the fullTypeName should contain the namespace
        public static Type SearchForType(string fullTypeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullTypeName);

                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        // Note that this class will find the wrong type in a lot of cases
        public static Type SearchForTypeByClassName(string className)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Just return the first one found
                var matchType = assembly.GetTypes().Where(t => t.Name == className).FirstOrDefault();

                if (matchType != null)
                {
                    return matchType;
                }
            }

            return null;
        }
    }
}
