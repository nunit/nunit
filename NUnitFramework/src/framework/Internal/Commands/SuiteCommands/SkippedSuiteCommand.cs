// ***********************************************************************
// Copyright (c) 2013 Charlie Poole
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
using System.Reflection;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// A SkipSuiteCommand is used when a TestSuite is being skipped
    /// </summary>
    public class SkippedSuiteCommand : SuiteCommand
    {
        /// <summary>
        /// Construct a SkipSuiteCommand
        /// </summary>
        /// <param name="suite">The TestSuite being skipped</param>
        public SkippedSuiteCommand(TestSuite suite) : base(suite) { }

        /// <summary>
        /// OneTimeSetUp simply sets the test result appropriately.
        /// It's up to the work item running the test to handle
        /// any child tests.
        /// </summary>
        /// <param name="context">The execution context used in running the test</param>
        public override void DoOneTimeSetUp(TestExecutionContext context)
        {
            switch (Suite.RunState)
            {
                default:
                case RunState.Skipped:
                    context.CurrentResult.SetResult(ResultState.Skipped, GetSkipReason());
                    break;
                case RunState.Ignored:
                    context.CurrentResult.SetResult(ResultState.Ignored, GetSkipReason());
                    break;
                case RunState.NotRunnable:
                    context.CurrentResult.SetResult(ResultState.NotRunnable, GetSkipReason(), GetProviderStackTrace());
                    break;
            }
        }

        /// <summary>
        /// The command does nothing on TearDown. Not normally called.
        /// </summary>
        /// <param name="context">The execution contextused in running the test</param>
        public override void DoOneTimeTearDown(TestExecutionContext context)
        {
        }

        private string GetSkipReason()
        {
            return (string)Suite.Properties.Get(PropertyNames.SkipReason);
        }

        private string GetProviderStackTrace()
        {
            return (string)Suite.Properties.Get(PropertyNames.ProviderStackTrace);
        }
    }
}
