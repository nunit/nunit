// ***********************************************************************
// Copyright (c) 2020 Charlie Poole, Rob Prouse
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

using NUnit.Framework;
using System.Collections.Generic;

#if NET35 || NET40 || NET45
using System.Runtime.Remoting.Messaging;
#else
using System.Threading;
#endif

namespace NUnit.TestData
{
    public static class ExecutionContextFixture
    {
#if !(NET35 || NET40 || NET45)
        private static readonly AsyncLocal<bool> WasExecutionContextUsed = new AsyncLocal<bool>();
#endif

        public static IEnumerable<int> Source1
        {
            get
            {
                AssertAndUseCurrentExecutionContext();
                return new[] { 1 };
            }
        }

        public static IEnumerable<int> Source2
        {
            get
            {
                AssertAndUseCurrentExecutionContext();
                return new[] { 2 };
            }
        }

        [TestCaseSource(nameof(Source1))]
        [TestCaseSource(nameof(Source2))]
        public static void TestCaseSourceExecutionContextIsNotShared(int testCase)
        {
            AssertAndUseCurrentExecutionContext();
        }

        [Test]
        public static void ValueSourceExecutionContextIsNotShared([ValueSource(nameof(Source1)), ValueSource(nameof(Source2))] int testCase)
        {
            AssertAndUseCurrentExecutionContext();
        }

        public static void ExecutionContextIsNotSharedBetweenTestCases([Range(1, 2)] int testCase)
        {
            AssertAndUseCurrentExecutionContext();
        }

        private static void AssertAndUseCurrentExecutionContext()
        {
#if NET35 || NET40 || NET45
            Assert.Null(CallContext.LogicalGetData("WasUsed"));

            CallContext.LogicalSetData("WasUsed", true);
#else
            Assert.That(!WasExecutionContextUsed.Value);

            WasExecutionContextUsed.Value = true;
#endif
        }
    }
}
