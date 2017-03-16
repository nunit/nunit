// ***********************************************************************
// Copyright (c) 2017 Charlie Poole
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
    /// TestActionBeforeCommand handles the BeforeTest method of a single 
    /// TestActionItem, relying on the item to remember it has been run.
    /// </summary>
    public class TestActionBeforeCommand : DelegatingTestCommand
    {
        private List<TestActionItem> _actions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestActionBeforeCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        /// <param name="actions">The TestActionItem to run before the inner command.</param>
        public TestActionBeforeCommand(TestCommand innerCommand, List<TestActionItem> actions)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestSuite, "TestActionBeforeCommand may only apply to a TestSuite", "innerCommand");
            Guard.ArgumentNotNull(actions, nameof(actions));

            _actions = actions;
        }

        /// <summary>
        /// Runs the test, saving a TestResult in the supplied TestExecutionContext.
        /// </summary>
        /// <param name="context">The context in which the test should run.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            try
            {
                foreach (var action in _actions)
                    action.BeforeTest(Test);

                context.CurrentResult = innerCommand.Execute(context);
            }
            catch (Exception ex)
            {
#if !PORTABLE && !NETSTANDARD1_6
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif
                context.CurrentResult.RecordException(ex);
            }

            return context.CurrentResult;
        }
    }
}
