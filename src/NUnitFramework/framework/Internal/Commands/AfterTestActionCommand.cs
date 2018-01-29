// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
using System.Threading;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TestActionAfterCommand handles the AfterTest method of a single 
    /// TestActionItem, provided the items BeforeTest has been run.
    /// </summary>
    public class AfterTestActionCommand : AfterTestCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AfterTestActionCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        /// <param name="action">The TestActionItem to run before the inner command.</param>
        public AfterTestActionCommand(TestCommand innerCommand, TestActionItem action)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestSuite, "BeforeTestActionCommand may only apply to a TestSuite", nameof(innerCommand));
            Guard.ArgumentNotNull(action, nameof(action));

            AfterTest = (context) =>
            {
                if (action.BeforeTestWasRun)
                {
                    var oldCount = context.CurrentResult.AssertionResults.Count;

                    action.AfterTest(Test);

                    // If there are new assertion results here, they are warnings issued
                    // in teardown. Redo test completion so they are listed properly.
                    if (context.CurrentResult.AssertionResults.Count > oldCount)
                        context.CurrentResult.RecordTestCompletion();
                };
            };
        }
    }
}
