using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools.Contracts
{
    using System.Collections.Generic;

    public interface ICertEngine
    {
        [NotNull] IEnumerable<TestResultDto> Run([NotNull] CertDto cert);
    }
}