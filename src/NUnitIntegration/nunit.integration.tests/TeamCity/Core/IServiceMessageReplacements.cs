using System.Collections.Generic;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal interface IServiceMessageReplacements
    {
        /// <summary>
        /// Performs TeamCity-format escaping of a string.
        /// </summary>
        IEnumerable<char> Encode([NotNull] IEnumerable<char> chars);

        IEnumerable<char> Decode([NotNull] IEnumerable<char> chars);
    }
}