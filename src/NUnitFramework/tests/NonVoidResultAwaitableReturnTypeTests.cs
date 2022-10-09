// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.TestData;
using F = NUnit.TestData.AwaitableReturnTypeFixture;

namespace NUnit.Framework
{
    [TestFixture(nameof(F.ReturnsNonVoidResultTask))]
    [TestFixture(nameof(F.ReturnsNonVoidResultCustomTask))]
    [TestFixture(nameof(F.ReturnsNonVoidResultCustomAwaitable))]
    [TestFixture(nameof(F.ReturnsNonVoidResultCustomAwaitableWithImplicitOnCompleted))]
    [TestFixture(nameof(F.ReturnsNonVoidResultCustomAwaitableWithImplicitUnsafeOnCompleted))]
    public sealed class NonVoidResultAwaitableReturnTypeTests : AwaitableReturnTypeTests
    {
        public NonVoidResultAwaitableReturnTypeTests(string methodName)
            : base(methodName)
        {

        }

        [Test]
        public void ResultMustMatchExpectedResult()
        {
            var result = RunCurrentTestMethod(new AsyncWorkload(
                isCompleted: true,
                onCompleted: continuation => continuation.Invoke(),
                getResult: () => 41));

            Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(result.Message, Contains.Substring("41"));
            Assert.That(result.Message, Contains.Substring("42"));
        }
    }
}
