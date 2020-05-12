// ***********************************************************************
// Copyright (c) 2008–2018 Charlie Poole, Rob Prouse
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
using System.ComponentModel;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework
{
    /// <summary>
    /// The IgnoredTestCaseData class represents a ignored TestCaseData. It adds
    /// the ability to set a date until which the test will be ignored.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class IgnoredTestCaseData : TestCaseData
    {
        #region Instance Fields

        /// <summary>
        /// The previous RunState
        /// </summary>
        private RunState _prevRunState;

        #endregion

        #region Constructors

        internal IgnoredTestCaseData(TestCaseData data, RunState prevRunState)
        {
            this.Arguments = data.Arguments;
            this.ArgDisplayNames = data.ArgDisplayNames;
            this.ExpectedResult = data.ExpectedResult;
            this.HasExpectedResult = data.HasExpectedResult;
            this.OriginalArguments = data.OriginalArguments;
            this.Properties = data.Properties;
            this.RunState = data.RunState;
            this.TestName = data.TestName;
            this._prevRunState = prevRunState;
        }

        #endregion

        #region Fluent Instance Modifiers

        /// <summary>
        /// Set the date that the test is being ignored until
        /// </summary>
        /// <param name="datetime">The date that the test is being ignored until</param>
        /// <returns>A modified TestCaseData.</returns>
        public TestCaseData Until(DateTimeOffset datetime)
        {
            if (_prevRunState != RunState.NotRunnable)
            {
                if (datetime > DateTimeOffset.UtcNow)
                {
                    RunState = RunState.Ignored;
                    string reason = (string)Properties.Get(PropertyNames.SkipReason);
                    Properties.AddIgnoreUntilReason(datetime, reason);
                }
                else
                {
                    RunState = _prevRunState;
                }
                Properties.Set(PropertyNames.IgnoreUntilDate, datetime.ToString("u") );
            }
            return this;
        }

        #endregion

    }
}
