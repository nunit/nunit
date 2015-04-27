using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Common;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
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
