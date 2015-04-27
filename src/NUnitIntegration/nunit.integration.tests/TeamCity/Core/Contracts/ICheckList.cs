using System.Collections.Generic;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public interface ICheckList
    {
        [NotNull] IEnumerable<ITest> GetCheckList(CertType certType);
    }
}
