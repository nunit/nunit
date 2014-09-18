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
    /// SetUpTearDownDecorator decorates a test command by running
    /// a setup method before the original command and a teardown
    /// method after it has executed.
    /// </summary>
    public class SetUpTearDownDecorator : ICommandDecorator
    {
        private TestMethod _testMethod;

        public SetUpTearDownDecorator(TestMethod testMethod)
        {
            _testMethod = testMethod;
        }

        CommandStage ICommandDecorator.Stage
        {
            get { return CommandStage.SetUpTearDown; }
        }

        int ICommandDecorator.Priority
        {
            get { return 0; }
        }

        TestCommand ICommandDecorator.Decorate(TestCommand command)
        {
            return new SetUpTearDownCommand(_testMethod, command);
        }
    }

    /// <summary>
    /// TODO: Documentation needed for class
    /// </summary>
    public class SetUpTearDownCommand : DelegatingTestCommand
    {
        private SetUpTearDownList _methods;
        private IList<TestActionItem> _actions = new List<TestActionItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SetUpTearDownCommand"/> class.
        /// </summary>
        /// <param name="testMethod">The TestMethod to which this command applies</param>
        /// <param name="innerCommand">The inner command.</param>
        public SetUpTearDownCommand(TestMethod testMethod, TestCommand innerCommand)
            : base(innerCommand)
        {
            if (Test.FixtureType != null)
                _methods = new SetUpTearDownList(Test.FixtureType, typeof(SetUpAttribute), typeof(TearDownAttribute));

            foreach (ITestAction action in ActionsHelper.GetActionsFromAttributeProvider(testMethod.Method))
                if (action.Targets == ActionTargets.Test || action.Targets == ActionTargets.Default)
                    _actions.Add(new TestActionItem(action));
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
                _methods.RunSetUp(context);

                RunBeforeActions(context);

                context.CurrentResult = innerCommand.Execute(context);
            }
            catch (Exception ex)
            {
#if !NETCF && !SILVERLIGHT
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif
                context.CurrentResult.RecordException(ex);
            }
            finally
            {
                if (context.ExecutionStatus != TestExecutionStatus.AbortRequested)
                {
                    RunAfterActions(context);

                    _methods.RunTearDown(context);
                }
            }

            return context.CurrentResult;
        }

        private void RunBeforeActions(TestExecutionContext context)
        {
            for (int i = 0; i < _actions.Count; i++)
                _actions[i].BeforeTest(Test);
        }

        private void RunAfterActions(TestExecutionContext context)
        {
            for (int i = _actions.Count; i > 0; )
                _actions[--i].AfterTest(Test);
        }
    }

    public class TestActionItem
    {
        private ITestAction _action;
        private bool _beforeTestWasRun;

        public TestActionItem(ITestAction action)
        {
            _action = action;
        }

        public void BeforeTest(Interfaces.ITest test)
        {
            _beforeTestWasRun = true;
            _action.BeforeTest(test);
        }

        public void AfterTest(Interfaces.ITest test)
        {
            if (_beforeTestWasRun)
                _action.AfterTest(test);
        }
    }
}
