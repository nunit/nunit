using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools
{
    internal interface IServiceMessageValidator
    {
        [NotNull]
        ValidationResult Validate([NotNull] IServiceMessage message);
    }
}