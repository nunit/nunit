using System;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal static class ServiceMessageExtensions
    {
        [NotNull]
        public static string GetAttr(this IServiceMessage message, string attrName, [NotNull] string defaultValue = "")
        {
            Contract.Requires<ArgumentNullException>(message != null);
            Contract.Requires<ArgumentNullException>(attrName != null);
            Contract.Requires<ArgumentNullException>(defaultValue != null);
            Contract.Ensures(Contract.Result<string>() != null);

            string val;
            if (message.TryGetAttribute(attrName, out val))
            {
                return val;
            }

            return defaultValue;
        }
    }
}
