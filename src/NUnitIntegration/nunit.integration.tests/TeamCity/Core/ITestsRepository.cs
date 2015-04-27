using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools
{
    using System.Collections.Generic;

    using JetBrains.TeamCityCert.Tools.Contracts;

    internal interface ITestsRepository
    {
        [NotNull] IEnumerable<ICmdLineToolTest> GetCmdLineToolTests(CertType certType);
    }
}