using System.Collections.Generic;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Contracts;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal interface ICaseRepository
    {
        [NotNull] IEnumerable<ICase> GetCases(CertType certType);
    }
}