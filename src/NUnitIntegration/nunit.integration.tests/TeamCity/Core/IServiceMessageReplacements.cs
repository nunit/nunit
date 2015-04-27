using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools
{
    using System.Collections.Generic;

    internal interface IServiceMessageReplacements
    {
        /// <summary>
        /// Performs TeamCity-format escaping of a string.
        /// </summary>
        IEnumerable<char> Encode([NotNull] IEnumerable<char> chars);

        IEnumerable<char> Decode([NotNull] IEnumerable<char> chars);
    }
}