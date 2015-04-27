using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools.Contracts
{
    public interface ITest
    {
        CertType CertType { get; }

        string CaseId { [NotNull] get; }

        string Description { [NotNull] get; }
    }
}