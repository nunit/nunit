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

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TODO: Documentation needed for class
    /// </summary>
    public class TestMethodCommand : TestCommand
    {
        private readonly TestMethod testMethod;
        private readonly object[] arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseCommand"/> class.
        /// </summary>
        /// <param name="test">The test.</param>
        public TestMethodCommand(Test test) : base(test)
        {
            this.testMethod = test as TestMethod;
            this.arguments = test.arguments;
        }

        /// <summary>
        /// Runs the test, saving a TestResult in
        /// TestExecutionContext.CurrentContext.CurrentResult
        /// </summary>
        /// <param name="testObject"></param>
        public override TestResult Execute(TestExecutionContext context)
        {
            object result = Reflect.InvokeMethod(testMethod.Method, context.TestObject, arguments);

            if (testMethod.hasExpectedResult)
                NUnit.Framework.Assert.AreEqual(testMethod.expectedResult, result);

            context.CurrentResult.SetResult(ResultState.Success);
            return context.CurrentResult;
        }
    }
}