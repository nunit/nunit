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
using System.Reflection;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// SetUpTearDownDecorator decorates a test command by running
    /// a setup method before the original command and a teardown
    /// method after it has executed.
    /// </summary>
    public class SetUpTearDownDecorator : ICommandDecorator
    {
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
            return new SetUpTearDownCommand(command);
        }
    }

    /// <summary>
    /// TODO: Documentation needed for class
    /// </summary>
    public class SetUpTearDownCommand : DelegatingTestCommand
    {
        private SetUpTearDownList _methods;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetUpTearDownCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        public SetUpTearDownCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
            if (Test.FixtureType != null)
                _methods = new SetUpTearDownList(Test.FixtureType, typeof(SetUpAttribute), typeof(TearDownAttribute));
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
                    _methods.RunTearDown(context);
            }

            return context.CurrentResult;
        }
    }
}
