// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.TestData.TestFixtureTests;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    class TestMethodTests
    {
        private DefaultTestCaseBuilder _builder;

        [SetUp]
        public void Setup()
        {
            _builder = new DefaultTestCaseBuilder();
        }

        [Test]
        public void CaptureNoArgumentsForRegularTestMethod()
        {
            var test = _builder.BuildFrom(new MethodWrapper(typeof(RegularFixtureWithOneTest), nameof(RegularFixtureWithOneTest.OneTest)));

            Assert.That(test, Is.TypeOf<TestMethod>());
            Assert.That(test.Arguments, Is.EqualTo(Array.Empty<object>()));
        }

        [Test]
        public void CaptureArgumentsForParameterizedTestMethod()
        {
            var test = _builder.BuildFrom(new MethodWrapper(typeof(FixtureWithParameterizedTestAndArgsSupplied), nameof(FixtureWithParameterizedTestAndArgsSupplied.SomeTest)));

            Assert.That(test.HasChildren, Is.True);
            Assert.That(test.Tests[0], Is.TypeOf<TestMethod>());
            Assert.That(test.Tests[0].Arguments, Is.EqualTo(new object[] { 42, "abc" }));
        }

        [Test]
        public void CaptureArgumentsForParameterizedTestMethodWithMultipleArguments()
        {
            var test = _builder.BuildFrom(new MethodWrapper(typeof(FixtureWithParameterizedTestAndMultipleArgsSupplied), nameof(FixtureWithParameterizedTestAndMultipleArgsSupplied.SomeTest)));

            Assert.That(test.HasChildren, Is.True);
            Assert.That(test.Arguments, Is.EqualTo(Array.Empty<object>()));
            Assert.That(test.Tests[0], Is.TypeOf<TestMethod>());
            Assert.That(test.Tests[1], Is.TypeOf<TestMethod>());
            var expectedArguments = new[] { new object[] { 42, "abc" }, new object[] { 24, "cba" } };
            var actualArguments = test.Tests.Select(t => t.Arguments);
            Assert.That(actualArguments, Is.EquivalentTo(expectedArguments));
        }
    }
}
