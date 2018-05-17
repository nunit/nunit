// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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

using System.Linq;
using NUnit.Framework.Internal.Builders;
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
            var test = _builder.BuildFrom(typeof(RegularFixtureWithOneTest)
                .GetFixtureMethod(nameof(RegularFixtureWithOneTest.OneTest)));

            Assert.That(test, Is.TypeOf<TestMethod>());
            Assert.That(test.Arguments, Is.EqualTo(new object[0]));
        }

        [Test]
        public void CaptureArgumentsForParameterizedTestMethod()
        {
            var test = _builder.BuildFrom(typeof(FixtureWithParameterizedTestAndArgsSupplied)
                .GetFixtureMethod(nameof(FixtureWithParameterizedTestAndArgsSupplied.SomeTest)));

            Assert.That(test.HasChildren, Is.True);
            Assert.That(test.Tests[0], Is.TypeOf<TestMethod>());
            Assert.That(test.Tests[0].Arguments, Is.EqualTo(new object[] { 42, "abc" }));
        }

        [Test]
        public void CaptureArgumentsForParameterizedTestMethodWithMultipleArguments()
        {
            var test = _builder.BuildFrom(typeof(FixtureWithParameterizedTestAndMultipleArgsSupplied)
                .GetFixtureMethod(nameof(FixtureWithParameterizedTestAndMultipleArgsSupplied.SomeTest)));

            Assert.That(test.HasChildren, Is.True);
            Assert.That(test.Arguments, Is.EqualTo(new object[0]));
            Assert.That(test.Tests[0], Is.TypeOf<TestMethod>());
            Assert.That(test.Tests[1], Is.TypeOf<TestMethod>());
            var expectedArguments = new[] { new object[] { 42, "abc" }, new object[] { 24, "cba" } };
            var actualArguments = test.Tests.Select(t => t.Arguments);
            Assert.That(actualArguments, Is.EquivalentTo(expectedArguments));
        }
    }
}
