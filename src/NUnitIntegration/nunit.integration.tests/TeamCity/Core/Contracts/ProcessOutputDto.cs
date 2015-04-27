using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Common;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public sealed class ProcessOutputDto
    {
        public ProcessOutputDto(int exitCode, [NotNull] IEnumerable<string> output)
        {
            Contract.Requires<ArgumentNullException>(output != null);

            ExitCode = exitCode;
            OutputLines = output.ToList();
        }

        public int ExitCode { get; private set; }

        public IEnumerable<string> OutputLines { [NotNull] get; private set; }
    }
}
