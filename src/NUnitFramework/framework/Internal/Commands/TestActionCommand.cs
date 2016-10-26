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
using System.Threading;

namespace NUnit.Framework.Internal.Commands
{
    using Interfaces;

    /// <summary>
    /// TestActionCommand runs the BeforeTest actions for a test,
    /// then runs the test and finally runs the AfterTestActions.
    /// </summary>
    public class TestActionCommand : DelegatingTestCommand
    {
        private IList<TestActionItem> _actions = new List<TestActionItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestActionCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        public TestActionCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestMethod, "TestActionCommand may only apply to a TestMethod", "innerCommand");
        }

        /// <summary>
        /// Runs the test, saving a TestResult in the supplied TestExecutionContext.
        /// </summary>
        /// <param name="context">The context in which the test should run.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            if (Test.Fixture == null)
                Test.Fixture = context.TestObject;

            // In the current implementation, upstream actions only apply to tests. If that should change in the future,
            // then actions would have to be tested for here. For now we simply assert it in Debug. We allow 
            // ActionTargets.Default, because it is passed down by ParameterizedMethodSuite.
            foreach (ITestAction action in context.UpstreamActions)
            {
                System.Diagnostics.Debug.Assert(
                    action.Targets == ActionTargets.Default || (action.Targets & ActionTargets.Test) == ActionTargets.Test,
                    "Invalid target on upstream action: " + action.Targets.ToString());

                _actions.Add(new TestActionItem(action));
            }

            foreach (ITestAction action in ActionsHelper.GetActionsFromAttributeProvider(((TestMethod)Test).Method.MethodInfo))
                if (action.Targets == ActionTargets.Default || (action.Targets & ActionTargets.Test) == ActionTargets.Test)
                    _actions.Add(new TestActionItem(action));

            try
            {
                for (int i = 0; i < _actions.Count; i++)
                    _actions[i].BeforeTest(Test);

                context.CurrentResult = innerCommand.Execute(context);
            }
            catch (Exception ex)
            {
#if !PORTABLE
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif
                context.CurrentResult.RecordException(ex);
            }
            finally
            {
                if (context.ExecutionStatus != TestExecutionStatus.AbortRequested)
                    for (int i = _actions.Count - 1; i >= 0; i--)
                        _actions[i].AfterTest(Test);
            }

            return context.CurrentResult;
        }
    }
}
