using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools
{
    using System.Collections.Generic;

    internal interface IServiceMessageStructureValidator
    {
        [NotNull]
        ValidationResult Validate([NotNull] IEnumerable<IServiceMessage> messages);
    }
}