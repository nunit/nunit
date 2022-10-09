// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.TestData;

namespace NUnit.Framework
{
    public static class ExecutionContextTests
    {
        [Test]
        public static void ExecutionContextIsNotSharedBetweenTestCases()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(
                typeof(ExecutionContextFixture),
                nameof(ExecutionContextFixture.ExecutionContextIsNotSharedBetweenTestCases));

            result.AssertPassed();
        }

        [Test]
        public static void TestCaseSourceExecutionContextIsNotShared()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(
                typeof(ExecutionContextFixture),
                nameof(ExecutionContextFixture.TestCaseSourceExecutionContextIsNotShared));

            result.AssertPassed();
        }

        [Test]
        public static void ValueSourceExecutionContextIsNotShared()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(
                typeof(ExecutionContextFixture),
                nameof(ExecutionContextFixture.ValueSourceExecutionContextIsNotShared));

            result.AssertPassed();
        }

        [Test]
        public static void ExecutionContextFlow()
        {
            var result = TestBuilder.RunTestFixture(typeof(ExecutionContextFlowFixture));

            result.AssertPassed();
        }
    }
}
