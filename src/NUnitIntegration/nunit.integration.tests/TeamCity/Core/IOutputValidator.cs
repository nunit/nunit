using System.Collections.Generic;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal interface IOutputValidator
    {
        [NotNull]
        ValidationResult Validate([NotNull] IEnumerable<IServiceMessage> rawMessages);
    }
}