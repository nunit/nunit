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

using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TODO: Documentation needed for class
    /// </summary>
    public class SetUpTearDownCommand : DelegatingTestCommand
    {
        private readonly MethodInfo[] setUpMethods;
        private readonly MethodInfo[] tearDownMethods;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetUpTearDownCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        public SetUpTearDownCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
            Test parent = Test.Parent as Test;
            if (parent != null)
            {
                this.setUpMethods = parent.SetUpMethods;
                this.tearDownMethods = parent.TearDownMethods;
            }
        }

        /// <summary>
        /// Runs the test, saving a TestResult in
        /// TestExecutionContext.CurrentContext.CurrentResult
        /// </summary>
        /// <param name="testObject"></param>
        public override TestResult Execute(object testObject)
        {
            try
            {
                RunSetUpMethods(testObject);

                CurrentResult = innerCommand.Execute(testObject);
            }
            catch (Exception ex)
            {
#if !NETCF
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif
                CurrentResult.RecordException(ex);
            }
            finally
            {
                RunTearDownMethods(testObject);
            }

            return CurrentResult;
        }

        private void RunSetUpMethods(object testObject)
        {
            if (setUpMethods != null)
                foreach (MethodInfo setUpMethod in setUpMethods)
                    Reflect.InvokeMethod(setUpMethod, setUpMethod.IsStatic ? null : testObject);
        }

        private void RunTearDownMethods(object testObject)
        {
            try
            {
                if (tearDownMethods != null)
                {
                    int index = tearDownMethods.Length;
                    while (--index >= 0)
                        Reflect.InvokeMethod(tearDownMethods[index], tearDownMethods[index].IsStatic ? null : testObject);
                }
            }
            catch (Exception ex)
            {
                if (ex is NUnitException)
                    ex = ex.InnerException;

                // TODO: Can we move this logic into TestResult itself?
                string message = "TearDown : " + ExceptionHelper.BuildMessage(ex);
                if (CurrentResult.Message != null)
                    message = CurrentResult.Message + NUnit.Env.NewLine + message;

#if !NETCF_1_0
                string stackTrace = "--TearDown" + NUnit.Env.NewLine + ExceptionHelper.BuildStackTrace(ex);
                if (CurrentResult.StackTrace != null)
                    stackTrace = CurrentResult.StackTrace + NUnit.Env.NewLine + stackTrace;

                // TODO: What about ignore exceptions in teardown?
                CurrentResult.SetResult(ResultState.Error, message, stackTrace);
#else
                Result.SetResult(ResultState.Error, message);
#endif
            }
        }
    }
}
