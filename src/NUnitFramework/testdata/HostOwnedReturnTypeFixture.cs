// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.TestData.HostOwnedReturnTypeFixture
{
    // These methods are not iterators, so calling one counts as an invocation even if nobody enumerates the result.
    public static class Coroutine
    {
        public static int Invocations;

        public static IEnumerator Start()
        {
            Invocations++;
            return Enumerable.Empty<object>().GetEnumerator();
        }
    }

    [TestFixture]
    public class CoroutineTests
    {
        public static readonly int[] Source = { 1, 2 };

        [Test]
        public IEnumerator PlainTest() => Coroutine.Start();

        [TestCase(1)]
        [TestCase(2)]
        public IEnumerator ParameterizedTest(int i) => Coroutine.Start();

        [TestCaseSource(nameof(Source))]
        public IEnumerator SourceTest(int i) => Coroutine.Start();

        [TestCase(1, 2)]
        public IEnumerator TooManyArguments(int i) => Coroutine.Start();

        [Test, Ignore("Ignored on purpose")]
        public IEnumerator IgnoredTest() => Coroutine.Start();

        [Test]
        public void VoidTest()
        {
        }

        [Test]
        public System.Threading.Tasks.Task TaskTest() => System.Threading.Tasks.Task.CompletedTask;
    }

    [TestFixture]
    public class GenericCoroutineTests
    {
        [Test]
        public System.Collections.Generic.IEnumerator<int> GenericEnumeratorTest()
        {
            Coroutine.Invocations++;
            return Enumerable.Empty<int>().GetEnumerator();
        }
    }

    [TestFixture]
    public class CoroutineSetUpFixture
    {
        [OneTimeSetUp]
        public IEnumerator OneTimeSetUp() => Coroutine.Start();

        [SetUp]
        public IEnumerator SetUp() => Coroutine.Start();

        [TearDown]
        public IEnumerator TearDown() => Coroutine.Start();

        [OneTimeTearDown]
        public IEnumerator OneTimeTearDown() => Coroutine.Start();

        [Test]
        public void VoidTest()
        {
        }
    }

    [TestFixture]
    public class CoroutineTearDownFixture
    {
        [TearDown]
        public IEnumerator TearDown() => Coroutine.Start();

        [Test]
        public void VoidTest()
        {
        }
    }

    // A custom builder that gets an IEnumerator test past the signature check the way hosts do today:
    // a dummy expected result, then HasExpectedResult cleared so nothing is asserted.
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DummyExpectedResultTestAttribute : NUnitAttribute, ISimpleTestBuilder, IImplyFixture
    {
        public TestMethod BuildFrom(IMethodInfo method, Test? suite)
        {
            var parms = new TestCaseParameters { ExpectedResult = new object() };
            var test = new NUnitTestCaseBuilder().BuildTestMethod(method, suite, parms);
            parms.HasExpectedResult = false;
            return test;
        }
    }

    public class DummyExpectedResultTests
    {
        [DummyExpectedResultTest]
        public IEnumerator NeverRuns() => Coroutine.Start();
    }
}

namespace NUnit.TestData.HostOwnedReturnTypeFixture.SetUpFixtureCase
{
    [SetUpFixture]
    public class CoroutineNamespaceSetUp
    {
        [OneTimeSetUp]
        public IEnumerator OneTimeSetUp() => Coroutine.Start();

        [OneTimeTearDown]
        public IEnumerator OneTimeTearDown() => Coroutine.Start();
    }

    [TestFixture]
    public class TestsUnderSetUpFixture
    {
        [Test]
        public void VoidTest()
        {
        }
    }
}

namespace NUnit.TestData.HostOwnedReturnTypeExpectedResult
{
    // With an expected result, NUnit runs the method and compares the value, with or without the setting,
    // so the existing "ExpectedResult = null" workaround keeps working.
    [TestFixture]
    public class ExpectedResultTests
    {
        public static int Invocations;

        [TestCase(ExpectedResult = null)]
        public System.Collections.IEnumerator? ReturnsNull()
        {
            Invocations++;
            return null;
        }
    }
}
