using System;

using JetBrains.Annotations;

using NUnit.Integration.Tests.TeamCity.Core.Common;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public sealed class TestResultDto
    {
        public TestResultDto([NotNull] string toolId, [NotNull] string caseId, TestState state)
        {
            Contract.Requires<ArgumentNullException>(toolId != null);
            Contract.Requires<ArgumentNullException>(caseId != null);

            ToolId = toolId;
            CaseId = caseId;
            State = state;
        }

        public string ToolId { [NotNull] get; private set; }

        public string CaseId { [NotNull] get; private set; }

        public TestState State { get; private set; }

        public string Details { [CanBeNull] get; [CanBeNull] set; }
    }
}
