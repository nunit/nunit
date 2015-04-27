using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools.Contracts
{
    using System.Collections.Generic;

    public interface ICheckList
    {
        [NotNull] IEnumerable<ITest> GetCheckList(CertType certType);
    }
}
