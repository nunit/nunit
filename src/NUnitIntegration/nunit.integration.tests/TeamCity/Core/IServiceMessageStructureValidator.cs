using System.Collections.Generic;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal interface IServiceMessageStructureValidator
    {
        [NotNull]
        ValidationResult Validate([NotNull] IEnumerable<IServiceMessage> messages);
    }
}