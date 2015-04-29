// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

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