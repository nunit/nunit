namespace JetBrains.TeamCityCert.Tools.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using JetBrains.Annotations;

    public sealed class CmdLineToolDto
    {
        public CmdLineToolDto(
            CertType certType,
            [NotNull] string toolId,
            [NotNull] string cmdLineFileName,
            [NotNull] IEnumerable<string> args, 
            [NotNull] IEnumerable<CaseDto> cases)
        {
            Contract.Requires<ArgumentNullException>(toolId != null);
            Contract.Requires<ArgumentNullException>(cmdLineFileName != null);
            Contract.Requires<ArgumentNullException>(args != null);
            Contract.Requires<ArgumentNullException>(cases != null);

            CertType = certType;
            ToolId = toolId;
            CmdLineFileName = cmdLineFileName;
            Args = args.ToList();
            Cases = cases.ToList();
        }

        public CertType CertType { get; private set; }

        public string ToolId { [NotNull] get; private set; }

        public string CmdLineFileName { [NotNull] get; private set; }

        public IEnumerable<string> Args { [NotNull] get; set; }

        public IEnumerable<CaseDto> Cases { [NotNull] get; private set; }
    }
}