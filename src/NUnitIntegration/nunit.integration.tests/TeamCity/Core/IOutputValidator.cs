using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools
{
    using System.Collections.Generic;

    internal interface IOutputValidator
    {
        [NotNull]
        ValidationResult Validate([NotNull] IEnumerable<string> output);
    }
}