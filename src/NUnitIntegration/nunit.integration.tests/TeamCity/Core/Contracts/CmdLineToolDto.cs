using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public sealed class CmdLineToolDto
    {
        public CmdLineToolDto(
            CertType certType, 
            [NotNull] string toolId, 
            [NotNull] string cmdLineFileName, 
            [NotNull] IEnumerable<string> args, 
            IDictionary<string, string> environmentVariables, 
            [NotNull] IEnumerable<CaseDto> cases)
        {
            Contract.Requires<ArgumentNullException>(toolId != null);
            Contract.Requires<ArgumentNullException>(cmdLineFileName != null);
            Contract.Requires<ArgumentNullException>(args != null);
            Contract.Requires<ArgumentNullException>(environmentVariables != null);
            Contract.Requires<ArgumentNullException>(cases != null);

            CertType = certType;
            ToolId = toolId;
            CmdLineFileName = cmdLineFileName;
            EnvironmentVariables = environmentVariables;
            Args = args.ToList();
            Cases = cases.ToList();
        }

        public CertType CertType { get; private set; }

        public string ToolId { [NotNull] get; private set; }

        public string CmdLineFileName { [NotNull] get; private set; }

        public IEnumerable<string> Args { [NotNull] get; private set; }

        public IDictionary<string, string> EnvironmentVariables { [NotNull] get; private set; }

        public IEnumerable<CaseDto> Cases { [NotNull] get; private set; }
    }
}