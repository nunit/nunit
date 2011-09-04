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
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal.Commands
{
    ///<summary>
    /// TODO: Documentation needed for class
    ///</summary>
    public class TestExecutionContextCommand : DelegatingTestCommand
    {
        /// <summary>
        /// TODO: Documentation needed for constructor
        /// </summary>
        /// <param name="innerCommand"></param>
        public TestExecutionContextCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
        }

        /// <summary>
        /// TODO: Documentation needed for constructor
        /// </summary>
        /// <param name="testObject">The object on which the test should run.</param>
        /// <param name="arguments">The arguments to be used in running the test or null.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(object testObject, ITestListener listener)
        {
            listener.TestStarted(Test);

            long startTime = DateTime.Now.Ticks;

            TestExecutionContext.Save();

            TestExecutionContext.CurrentContext.CurrentTest = this.Test;
            TestExecutionContext.CurrentContext.CurrentResult = this.Test.MakeTestResult();

            try
            {
                return innerCommand.Execute(testObject, listener);
            }
                // TODO: Ensure no exceptions escape
            finally
            {
                CurrentResult.AssertCount = TestExecutionContext.CurrentContext.AssertCount;

                long stopTime = DateTime.Now.Ticks;
                double time = ((double)(stopTime - startTime)) / (double)TimeSpan.TicksPerSecond;
                CurrentResult.Time = time;

                listener.TestFinished(CurrentResult);

                TestExecutionContext.Restore();
            }
        }
    }
}
