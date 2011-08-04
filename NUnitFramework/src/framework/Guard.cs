using System;

namespace NUnit.Framework
{
    static class Guard
    {
        public static void ArgumentNotNull(object value, string name)
        {
            if (value == null)
                throw new ArgumentNullException("Argument " + name + " must not be null", name);
        }

        public static void ArgumentNotNullOrEmpty(string value, string name)
        {
            ArgumentNotNull(value, name);

            if (value == string.Empty)
                throw new ArgumentException("Argument " + name +" must not be the empty string", name);
        }
    }
}
