using System.Collections.Generic;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public interface IProcessManager
    {
        [NotNull]
        ProcessOutputDto StartProcess([NotNull] string cmdLineFileName, [NotNull] IEnumerable<string> args, [NotNull] IDictionary<string, string> environmentVariables);
    }
}
