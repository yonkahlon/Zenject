using System;

namespace ModestTree
{
    public static class StringUtil
    {
        // This is like string.Format except it will print NULL instead of just
        // a blank character when a parameter is null
        public static string Format(string format, params object[] parameters)
        {
            // doin this funky loop to ensure nulls are replaced with "NULL"
            // and that the original parameters array will not be modified
            if (parameters != null && parameters.Length > 0)
            {
                object[] paramToUse = parameters;

                foreach (object cur in parameters)
                {
                    if (cur == null)
                    {
                        paramToUse = new object[parameters.Length];

                        for (int i = 0; i < parameters.Length; ++i)
                        {
                            paramToUse[i] = parameters[i] ?? "NULL";
                        }

                        break;
                    }
                }

                format = string.Format(format, paramToUse);
            }

            return format;
        }
    }
}
