using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal interface IServiceMessageValidator
    {
        [NotNull]
        ValidationResult Validate([NotNull] IServiceMessage message);
    }
}