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
