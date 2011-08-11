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

using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TODO: Documentation needed for class
    /// </summary>
    public class RepeatedTestCommand : DelegatingTestCommand
    {
        private int repeatCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatedTestCommand"/> class.
        /// TODO: Add a comment about where the repeat count is retrieved. 
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        public RepeatedTestCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
            this.repeatCount = Test.Properties.GetSetting(PropertyNames.RepeatCount, 1);
        }

        /// <summary>
        /// Runs the test, saving a TestResult in
        /// TestExecutionContext.CurrentContext.CurrentResult
        /// </summary>
        /// <param name="testObject">The object on which the test should run.</param>
        /// <param name="arguments">The arguments to be used in running the test or null.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(object testObject, ITestListener listener)
        {
            int count = repeatCount;

            while (count-- > 0)
            {
                CurrentResult = innerCommand.Execute(testObject, listener);

                // TODO: We may want to change this so that all iterations are run
                if (CurrentResult.ResultState == ResultState.Failure ||
                    CurrentResult.ResultState == ResultState.Error ||
                    CurrentResult.ResultState == ResultState.Cancelled)
                {
                    break;
                }
            }

            return CurrentResult;
        }
    }
}
