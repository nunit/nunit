using System;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal static class Contract
    {
        public static void Requires<T>(bool condition)
        {
            if (!condition)
            {
                throw new ArgumentNullException();
            }
        }

        public static void Ensures(bool condition)
        {
        }

        public static T Result<T>()
        {
            return default(T);
        }
    }
}
