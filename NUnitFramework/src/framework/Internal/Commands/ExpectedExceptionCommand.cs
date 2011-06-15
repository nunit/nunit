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
using System.Threading;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TODO: Documentation needed for class
    /// </summary>
    public class ExpectedExceptionCommand : DelegatingTestCommand
    {
        private readonly ExpectedExceptionProcessor exceptionProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedExceptionCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        public ExpectedExceptionCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
            this.exceptionProcessor = ((TestMethod)Test).ExceptionProcessor;
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
                CurrentResult = innerCommand.Execute(testObject);

                if (CurrentResult.ResultState == ResultState.Success && exceptionProcessor != null)
                    exceptionProcessor.ProcessNoException(CurrentResult);
            }
            catch (Exception ex)
            {
#if !NETCF
                if (ex is ThreadAbortException)
                    Thread.ResetAbort();
#endif
                if (exceptionProcessor == null)
                    CurrentResult.RecordException(ex);
                else
                    exceptionProcessor.ProcessException(ex, CurrentResult);
            }

            return CurrentResult;
        }
    }
}