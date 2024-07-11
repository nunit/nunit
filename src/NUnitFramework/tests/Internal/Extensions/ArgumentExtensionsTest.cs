// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;

namespace NUnit.Framework.Internal.Extensions
{
    [TestFixture]
    internal sealed class ArgumentExtensionsTest
    {
        [Test]
        public void TestLastArgumentIsCancellationToken()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Array.Empty<object>().LastArgumentIsCancellationToken(), Is.False);
                Assert.That(new object[] { 42 }.LastArgumentIsCancellationToken(), Is.False);
                Assert.That(new object[] { CancellationToken.None }.LastArgumentIsCancellationToken(), Is.True);
                Assert.That(new object[] { 42, CancellationToken.None }.LastArgumentIsCancellationToken(), Is.True);
                Assert.That(new object[] { CancellationToken.None, 42 }.LastArgumentIsCancellationToken(), Is.False);
            });
        }

        [TestCase(nameof(MethodWithNoParameters), ExpectedResult = false)]
        [TestCase(nameof(MethodWithIntegerParameter), ExpectedResult = false)]
        [TestCase(nameof(MethodWithCancellationTokenParameter), ExpectedResult = true)]
        [TestCase(nameof(MethodWithIntAndCancellationTokenParameter), ExpectedResult = true)]
        [TestCase(nameof(MethodWithCancellationTokenAndIntParameter), ExpectedResult = false)]
        public bool TestLastParameterAcceptsCancellationToken(string methodName)
        {
            var methodWrapper = new MethodWrapper(GetType(), methodName);

            return methodWrapper.GetParameters().LastParameterAcceptsCancellationToken();
        }

        private void MethodWithNoParameters()
        {
        }

#pragma warning disable IDE0060 // Remove unused parameter
        private void MethodWithIntegerParameter(int i)
        {
        }

        private void MethodWithCancellationTokenParameter(CancellationToken cancellationToken)
        {
        }

        private void MethodWithIntAndCancellationTokenParameter(int i, CancellationToken cancellationToken)
        {
        }

        private void MethodWithCancellationTokenAndIntParameter(CancellationToken cancellationToken, int i)
        {
        }
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
