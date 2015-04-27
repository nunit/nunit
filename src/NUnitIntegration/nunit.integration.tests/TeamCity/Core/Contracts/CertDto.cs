using System;
using System.Diagnostics.Contracts;

namespace JetBrains.TeamCityCert.Tools.Contracts
{
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    public sealed class CertDto
    {
        public CertDto(IEnumerable<CmdLineToolDto> cmdLineTools)
        {
            Contract.Requires<ArgumentNullException>(cmdLineTools != null);

            CmdLineTools = cmdLineTools.ToList();
        }

        public IEnumerable<CmdLineToolDto> CmdLineTools { [NotNull] get; private set; }
    }
}
