// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests
{
    [TestFixture]
    public sealed class AsyncVoidTests
    {
        [Test]
        public void ExceptionThrownInAsyncVoidMethodIsCaughtByNUnit()
        {
            var test = TestBuilder.MakeTestCase(typeof(AsyncVoidFixture), nameof(AsyncVoidFixture.NonAsyncTestCallingAsyncVoidMethod));
            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));

            var result = TestBuilder.RunTest(test);
            Assert.That(result.ResultState, Is.EqualTo(ResultState.Error));
            Assert.That(result.Message, Does.Contain(AsyncVoidFixture.Message));
        }
    }
}
