// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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

#if !NETCF
using System;
using System.Threading;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal.Commands
{
    public class ThreadedTestDecorator : ICommandDecorator
    {
        #region ICommandDecorator Members

        CommandStage ICommandDecorator.Stage
        {
            get { return CommandStage.CreateThread; }
        }

        int ICommandDecorator.Priority
        {
            get { return 0; }
        }

        TestCommand ICommandDecorator.Decorate(TestCommand command)
        {
            return new ThreadedTestCommand(command);
        }

        #endregion
    }

    /// <summary>
    /// TODO: Documentation needed for class
    /// </summary>
    public class ThreadedTestCommand : DelegatingTestCommand
    {
        private TestExecutionContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadedTestCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        public ThreadedTestCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
        }

        /// <summary>
        /// Runs the test, saving a TestResult in
        /// TestExecutionContext.CurrentContext.CurrentResult
        /// </summary>
        /// <param name="testObject">The object on which the test should run.</param>
        /// <param name="arguments">The arguments to be used in running the test or null.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            this.context = context;

            Thread thread = new Thread(new ThreadStart(RunTestProc));

            thread.CurrentCulture = context.CurrentCulture;
            thread.CurrentUICulture = context.CurrentUICulture;
            
            // NOTE: Setting to Unknown causes an error under the Mono 1.0 profile
            ApartmentState state = (ApartmentState)Test.Properties.GetSetting(PropertyNames.ApartmentState, ApartmentState.Unknown);
            if (state != ApartmentState.Unknown)
#if CLR_2_0 || CLR_4_0
                thread.SetApartmentState(state);
#else
                thread.ApartmentState = state;
#endif

#if NUNITLITE
            int timeout = Timeout.Infinite;
#else
            InternalTrace.Debug("Starting TestThread");

            int timeout = context.TestCaseTimeout;
            if (timeout <= 0)
                timeout = Timeout.Infinite;
#endif

            thread.Start();

            thread.Join(timeout);

#if !NUNITLITE
            InternalTrace.Debug("Join Complete");
#endif

            // Timeout?
            if (thread.IsAlive)
            {
                thread.Abort();
                context.CurrentResult.SetResult(ResultState.Failure,
                    string.Format("Test exceeded Timeout value of {0}ms", timeout));
            }

            return context.CurrentResult;
        }

        /// <summary>
        /// This is the engine of this class - the actual call to the
        /// inner command. Any exception is saved for later use
        /// </summary>
        private void RunTestProc()
        {
            try
            {
                context.CurrentResult = innerCommand.Execute(context);
            }
            catch (Exception e)
            {
                if (context.CurrentResult == null)
                    context.CurrentResult = this.Test.MakeTestResult();
                context.CurrentResult.RecordException(e);
            }
        }
    }
}
#endif