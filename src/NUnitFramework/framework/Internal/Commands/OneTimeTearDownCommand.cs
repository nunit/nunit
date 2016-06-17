// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// OneTimeTearDownCommand performs any teardown actions
    /// specified for a suite and calls Dispose on the user
    /// test object, if any.
    /// </summary>
    public class OneTimeTearDownCommand : TestCommand
    {
        private List<SetUpTearDownItem> _setUpTearDownItems;
        private List<TestActionItem> _actions;

        /// <summary>
        /// Construct a OneTimeTearDownCommand
        /// </summary>
        /// <param name="suite">The test suite to which the command applies</param>
        /// <param name="setUpTearDownItems">A SetUpTearDownList for use by the command</param>
        /// <param name="actions">A List of TestActionItems to be run before teardown.</param>
        public OneTimeTearDownCommand(TestSuite suite, List<SetUpTearDownItem> setUpTearDownItems, List<TestActionItem> actions)
            : base(suite)
        {
            _setUpTearDownItems = setUpTearDownItems;
            _actions = actions;
        }

        /// <summary>
        /// Overridden to run the teardown methods specified on the test.
        /// </summary>
        /// <param name="context">The TestExecutionContext to be used.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            TestResult suiteResult = context.CurrentResult;

            try
            {
                for (int i = _actions.Count; i > 0; )
                    _actions[--i].AfterTest(Test);

                if (_setUpTearDownItems != null)
                    foreach(var item in _setUpTearDownItems)
                        item.RunTearDown(context);

                IDisposable disposable = context.TestObject as IDisposable;
                if (disposable != null && Test is IDisposableFixture)
                    disposable.Dispose();
            }
            catch (Exception ex)
            {
                suiteResult.RecordTearDownException(ex);
            }

            return suiteResult;
        }
    }
}
