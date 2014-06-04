using System;

namespace ModestTree
{
    public static class MiscExtensions
    {
        // We'd prefer to use the name Format here but that conflicts with
        // the existing string.Format method
        public static string With(this string s, params object[] args)
        {
            return String.Format(s, args);
        }
    }
}
