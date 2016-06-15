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
using System.Diagnostics;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TODO: Documentation needed for class
    /// </summary>
    public class MaxTimeCommand : DelegatingTestCommand
    {
        private int maxTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxTimeCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        /// <param name="maxTime">The max time allowed in milliseconds</param>
        public MaxTimeCommand(TestCommand innerCommand, int maxTime)
            : base(innerCommand)
        {
            this.maxTime = maxTime;
        }

        /// <summary>
        /// Runs the test, saving a TestResult in the supplied TestExecutionContext
        /// </summary>
        /// <param name="context">The context in which the test should run.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            // TODO: This command duplicates the calculation of the
            // duration of the test because that calculation is 
            // normally performed at a higher level. Most likely,
            // we should move the maxtime calculation to the
            // higher level eventually.
            long startTicks = Stopwatch.GetTimestamp();

            TestResult testResult = innerCommand.Execute(context);

            long tickCount = Stopwatch.GetTimestamp() - startTicks;
            double seconds = (double)tickCount / Stopwatch.Frequency;
            testResult.Duration = seconds;

            if (testResult.ResultState == ResultState.Success)
            {
                double elapsedTime = testResult.Duration * 1000d;

                if (elapsedTime > maxTime)
                    testResult.SetResult(ResultState.Failure,
                        string.Format("Elapsed time of {0}ms exceeds maximum of {1}ms",
                            elapsedTime, maxTime));
            }

            return testResult;
        }
    }
}