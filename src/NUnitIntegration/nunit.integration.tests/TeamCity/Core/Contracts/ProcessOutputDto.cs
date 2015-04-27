using JetBrains.Annotations;

namespace JetBrains.TeamCityCert.Tools.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

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
