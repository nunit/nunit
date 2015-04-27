using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public interface ITest
    {
        CertType CertType { get; }

        string CaseId { [NotNull] get; }

        string Description { [NotNull] get; }
    }
}