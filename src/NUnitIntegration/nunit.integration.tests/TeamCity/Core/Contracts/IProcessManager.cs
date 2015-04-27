using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools.Contracts
{
    using System.Collections.Generic;

    public interface IProcessManager
    {
        [NotNull] ProcessOutputDto StartProcess([NotNull] string cmdLineFileName, IEnumerable<string> args);
    }
}
