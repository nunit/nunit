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

        [Test]
        public void Unpack_EmptyArray_ReturnsEmptyArray()
        {
            Array array = Array.Empty<int>();
            Assert.That(array.Unpack(), Is.Empty);
        }

        [Test]
        public void Unpack_SingleElement_ReturnsSingleElementArray()
        {
            Array array = new int[] { 42 };
            Assert.That(array.Unpack(), Is.EqualTo(new object?[] { 42 }));
        }

        [Test]
        public void Unpack_MultipleElements_ReturnsAllElements()
        {
            Array array = new int[] { 1, 2, 3 };
            Assert.That(array.Unpack(), Is.EqualTo(new object?[] { 1, 2, 3 }));
        }

        [TestCase(nameof(MethodWithNoParameters), ExpectedResult = false)]
        [TestCase(nameof(MethodWithIntegerParameter), ExpectedResult = true)]
        [TestCase(nameof(MethodWithCancellationTokenParameter), ExpectedResult = false)]
        [TestCase(nameof(MethodWithIntAndCancellationTokenParameter), ExpectedResult = false)]
        public bool HasSingleParameterOfType_IntType(string methodName)
        {
            var methodWrapper = new MethodWrapper(GetType(), methodName);
            return methodWrapper.GetParameters().HasSingleParameterOfType(typeof(int));
        }

        [Test]
        public void ShouldUnpackArrayAsArguments_ParamsParameter_AlwaysUnpacks()
        {
            var parameters = new MethodWrapper(GetType(), nameof(MethodWithParamsIntArray)).GetParameters();
            Assert.That(parameters.ShouldUnpackArrayAsArguments(new int[] { 1, 2, 3 }), Is.True);
        }

        [Test]
        public void ShouldUnpackArrayAsArguments_NoParameters_ReturnsFalse()
        {
            var parameters = new MethodWrapper(GetType(), nameof(MethodWithNoParameters)).GetParameters();
            Assert.That(parameters.ShouldUnpackArrayAsArguments(new int[] { 1 }), Is.False);
        }

        [Test]
        public void ShouldUnpackArrayAsArguments_ArrayShorterThanParamCount_ReturnsFalse()
        {
            var parameters = new MethodWrapper(GetType(), nameof(MethodWithTwoIntParameters)).GetParameters();
            Assert.That(parameters.ShouldUnpackArrayAsArguments(new int[] { 1 }), Is.False);
        }

        [Test]
        public void ShouldUnpackArrayAsArguments_ExactTypeMatch_ReturnsFalse()
        {
            // Method takes int[], source yields int[] — the array IS the argument, not a container
            var parameters = new MethodWrapper(GetType(), nameof(MethodWithIntArrayParameter)).GetParameters();
            Assert.That(parameters.ShouldUnpackArrayAsArguments(new int[] { 1, 2, 3 }), Is.False);
        }

        [Test]
        public void ShouldUnpackArrayAsArguments_MultipleParameters_ReturnsTrue()
        {
            // Each element maps to one parameter
            var parameters = new MethodWrapper(GetType(), nameof(MethodWithTwoIntParameters)).GetParameters();
            Assert.That(parameters.ShouldUnpackArrayAsArguments(new int[] { 1, 2 }), Is.True);
        }

        [Test]
        public void ShouldUnpackArrayAsArguments_SingleParam_CountMatches_ReturnsTrue()
        {
            // Classic container pattern: new object[] { actualArg } — count equals argsNeeded
            var parameters = new MethodWrapper(GetType(), nameof(MethodWithObjectParameter)).GetParameters();
            Assert.That(parameters.ShouldUnpackArrayAsArguments(new object[] { "hello" }), Is.True);
        }

        [Test]
        public void ShouldUnpackArrayAsArguments_SingleParam_AssignableArrayType_ReturnsFalse()
        {
            // int[] is assignable to Array — the array IS the argument, pass it directly
            var parameters = new MethodWrapper(GetType(), nameof(MethodWithArrayParameter)).GetParameters();
            Assert.That(parameters.ShouldUnpackArrayAsArguments(new int[] { 1, 2, 3 }), Is.False);
        }

        [Test]
        public void ShouldUnpackArrayAsArguments_SingleParam_NonAssignableType_ReturnsTrue()
        {
            // int[] is not assignable to int — unpack (will produce a count-mismatch error downstream)
            var parameters = new MethodWrapper(GetType(), nameof(MethodWithIntegerParameter)).GetParameters();
            Assert.That(parameters.ShouldUnpackArrayAsArguments(new int[] { 1, 2, 3 }), Is.True);
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

        private void MethodWithParamsIntArray(params int[] args)
        {
        }

        private void MethodWithTwoIntParameters(int a, int b)
        {
        }

        private void MethodWithIntArrayParameter(int[] array)
        {
        }

        private void MethodWithArrayParameter(Array array)
        {
        }

        private void MethodWithObjectParameter(object o)
        {
        }
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
