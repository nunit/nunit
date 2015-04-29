using System;

using JetBrains.Annotations;

namespace NUnit.Integration.Tests.TeamCity.Core.Contracts
{
    public sealed class TestResultDto
    {
        public TestResultDto([NotNull] string toolId, [NotNull] string caseId, TestState state, [NotNull] Details details)
        {
            Contract.Requires<ArgumentNullException>(toolId != null);
            Contract.Requires<ArgumentNullException>(caseId != null);
            Contract.Requires<ArgumentNullException>(details != null);

            ToolId = toolId;
            CaseId = caseId;
            State = state;
            Details = details;
        }

        public string ToolId { [NotNull] get; private set; }

        public string CaseId { [NotNull] get; private set; }

        public TestState State { get; private set; }

        public Details Details { [CanBeNull] get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", ToolId, CaseId);
        }
    }
}
