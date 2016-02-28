// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

using System.Reflection;
using System.Text;

namespace NUnit.Framework.Internal.Tests
{
    public abstract class TestNamingTests
    {
        protected const string OUTER_CLASS = "NUnit.Framework.Internal.Tests.TestNamingTests";

        protected abstract string FixtureName { get; }
        protected abstract string ClassName { get; }

        protected TestContext.TestAdapter CurrentTest
        {
            get { return TestContext.CurrentContext.Test; }
        }

        [Test]
        public void SimpleTest()
        {
            CheckNames("SimpleTest", "SimpleTest");
        }

        [TestCase(5, 7, "ABC")]
        public void ParameterizedTest(int x, int y, string s)
        {
            CheckNames("ParameterizedTest(5,7,\"ABC\")", "ParameterizedTest");
        }

        [TestCase("abcdefghijklmnopqrstuvwxyz")]
        public void TestCaseWithLongStringArgument(string s)
        {
            CheckNames("TestCaseWithLongStringArgument(\"abcdefghijklmnopqrstuvwxyz\")", "TestCaseWithLongStringArgument");
        }

        [TestCase(42)]
        public void GenericTest<T>(T arg)
        {
            CheckNames("GenericTest<Int32>(42)", "GenericTest");
        }

        [TestCase()]
        [TestCase(1)]
        [TestCase(1, 2)]
        [TestCase(1, 2, 3)]
        public void TestWithParamsArgument(params int[] array)
        {
            var sb = new StringBuilder("TestWithParamsArgument(");

            foreach (int n in array)
            {
                if (n > 1) sb.Append(",");
                sb.Append(n.ToString());
            }

            sb.Append(")");

            CheckNames(sb.ToString(), "TestWithParamsArgument");
        }

        private void CheckNames(string expectedTestName, string expectedMethodName)
        {
            Assert.That(CurrentTest.Name, Is.EqualTo(expectedTestName));
            Assert.That(CurrentTest.FullName, Is.EqualTo(OUTER_CLASS + "+" + FixtureName + "." + expectedTestName));
            Assert.That(CurrentTest.MethodName, Is.EqualTo(expectedMethodName));
            Assert.That(CurrentTest.ClassName, Is.EqualTo(OUTER_CLASS + "+" + ClassName));
        }

        public class SimpleFixture : TestNamingTests
        {
            protected override string FixtureName
            {
                get { return "SimpleFixture"; }
            }

            protected override string ClassName
            {
                get { return "SimpleFixture"; }
            }
        }

        [TestFixture(typeof(int))]
        public class GenericFixture<T> : TestNamingTests
        {
            protected override string FixtureName
            {
                get { return "GenericFixture<Int32>"; }
            }

            protected override string ClassName
            {
                get { return "GenericFixture`1"; }
            }
        }

        [TestFixture(42, "Forty-two")]
        public class ParameterizedFixture : TestNamingTests
        {
            public ParameterizedFixture(int x, string s) { }

            protected override string FixtureName
            {
                get { return "ParameterizedFixture(42,\"Forty-two\")"; }
            }

            protected override string ClassName
            {
                get { return "ParameterizedFixture"; }
            }
        }

        [TestFixture("This is really much too long to be used in the test name!")]
        public class ParameterizedFixtureWithLongStringArgument : TestNamingTests
        {
            public ParameterizedFixtureWithLongStringArgument(string s) { }

            protected override string FixtureName
            {
                get { return "ParameterizedFixtureWithLongStringArgument(\"This is really much too long to be us...\")"; }
            }

            protected override string ClassName
            {
                get { return "ParameterizedFixtureWithLongStringArgument"; }
            }
        }

        [TestFixture(typeof(int), typeof(string), 42, "Forty-two")]
        public class GenericParameterizedFixture<T1,T2> : TestNamingTests
        {
            public GenericParameterizedFixture(T1 x, T2 y) { }

            protected override string FixtureName
            {
                get { return "GenericParameterizedFixture<Int32,String>(42,\"Forty-two\")"; }
            }

            protected override string ClassName
            {
                get { return "GenericParameterizedFixture`2"; }
            }
        }
    }
}
