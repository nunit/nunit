using System.Collections.Generic;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public interface ICertEngine
    {
        [NotNull]
        IEnumerable<ITestResultEvaluator> Run([NotNull] CertDto cert);
    }
}