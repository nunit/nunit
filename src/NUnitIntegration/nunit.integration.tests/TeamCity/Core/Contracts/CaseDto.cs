using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Common;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public sealed class CaseDto
    {
        public CaseDto(
            [NotNull] string сaseId, 
            [NotNull] IEnumerable<string> args)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(сaseId));
            Contract.Requires<ArgumentNullException>(args != null);

            CaseId = сaseId;
            Args = args.ToList();
        }

        public string CaseId { [NotNull] get; private set; }

        public IEnumerable<string> Args { [NotNull] get; private set; }
    }
}