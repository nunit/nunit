// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// This innerCommand will add a warning to the test result
    /// </summary>
    public class WarnCommand : TestCommand
    {
        private readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarnCommand"/> class.
        /// </summary>
        /// <param name="test">The current test.</param>
        /// <param name="message">The warning message.</param>
        public WarnCommand(Test test, string message) : base(test)
        {
            _message = message;
        }

        /// <summary>
        /// Overridden to simply set the CurrentResult to the
        /// appropriate warn state with message.
        /// </summary>
        /// <param name="context">The execution context for the test</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            TestResult testResult = context.CurrentResult;
            testResult.SetResult(ResultState.Warning, _message);
            return testResult;
        }
    }
}
