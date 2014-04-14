using System;
using System.Collections.Generic;
using System.Text;

namespace ModestTree
{
    public static class ExceptionExtensions
    {
        public static string GetFullMessage(this Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }
    }
}
