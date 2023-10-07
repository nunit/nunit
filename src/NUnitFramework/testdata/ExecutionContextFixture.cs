// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using System.Collections.Generic;

#if NETFRAMEWORK
using System.Runtime.Remoting.Messaging;
#else
using System.Threading;
#endif

namespace NUnit.TestData
{
    public static class ExecutionContextFixture
    {
#if !NETFRAMEWORK
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
#if NETFRAMEWORK
            Assert.That(CallContext.LogicalGetData("WasUsed"), Is.Null);

            CallContext.LogicalSetData("WasUsed", true);
#else
            Assert.That(!WasExecutionContextUsed.Value);

            WasExecutionContextUsed.Value = true;
#endif
        }
    }
}
