using System;
using System.Collections.Generic;

namespace NUnit.Common
{
    static class StringHelper
    {
        public static string Enquote(this string source)
        {
            return "'" + source + "'";
        }

        public static IEnumerable<string> Enquote(this IEnumerable<string> source)
        {
            foreach (var item in source)
            {
                yield return item.Enquote();
            }            
        }

        public static string Join(this IEnumerable<string> source, string delimiter)
        {
            return string.Join(delimiter, new List<string>(source).ToArray());
        }
    }
}
