// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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

using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Internal;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Attributes
{
    public class TestMethodBuilderTests
    {
        [Test]
        public void TestAttribute_NoArgs_Runnable()
        {
            var method = GetMethod("MethodWithoutArgs");
            TestMethod test = new TestAttribute().BuildFrom(method, null);
            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void TestAttribute_WithArgs_NotRunnable()
        {
            var method = GetMethod("MethodWithIntArgs");
            TestMethod test = new TestAttribute().BuildFrom(method, null);
            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void TestCaseAttribute_NoArgs_NotRunnable()
        {
            var method = GetMethod("MethodWithoutArgs");
            List<TestMethod> tests = new List<TestMethod>(new TestCaseAttribute(5, 42).BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(1));
            Assert.That(tests[0].RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void TestCaseAttribute_RightArgs_Runnable()
        {
            var method = GetMethod("MethodWithIntArgs");
            List<TestMethod> tests = new List<TestMethod>(new TestCaseAttribute(5, 42).BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(1));
            Assert.That(tests[0].RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void TestCaseAttribute_WrongArgs_NotRunnable()
        {
            var method = GetMethod("MethodWithIntArgs");
            List<TestMethod> tests = new List<TestMethod>(new TestCaseAttribute(5, 42, 99).BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(1));
            Assert.That(tests[0].RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void TestCaseSourceAttribute_NoArgs_NotRunnable()
        {
            var method = GetMethod("MethodWithoutArgs");
            List<TestMethod> tests = new List<TestMethod>(new TestCaseSourceAttribute("GoodData").BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(3));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void TestCaseSourceAttribute_RightArgs_Runnable()
        {
            var method = GetMethod("MethodWithIntArgs");
            List<TestMethod> tests = new List<TestMethod>(new TestCaseSourceAttribute("GoodData").BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(3));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void TestCaseSourceAttribute_WrongArgs_NotRunnable()
        {
            var method = GetMethod("MethodWithIntArgs");
            List<TestMethod> tests = new List<TestMethod>(new TestCaseSourceAttribute("BadData").BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(3));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

#if !PORTABLE
        [Test]
        public void TheoryAttribute_NoArgs_NoCases()
        {
            var method = GetMethod("MethodWithoutArgs");
            List<TestMethod> tests = new List<TestMethod>(new TheoryAttribute().BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(0));
        }

        [Test]
        public void TheoryAttribute_WithArgs_Runnable()
        {
            var method = GetMethod("MethodWithIntArgs");
            List<TestMethod> tests = new List<TestMethod>(new TheoryAttribute().BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(9));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }
#endif

        [Test]
        public void CombinatorialAttribute_NoArgs_NoCases()
        {
            var method = GetMethod("MethodWithoutArgs");
            List<TestMethod> tests = new List<TestMethod>(new CombinatorialAttribute().BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(0));
        }

        [Test]
        public void CombinatorialAttribute_WithArgs_Runnable()
        {
            var method = GetMethod("MethodWithIntValues");
            List<TestMethod> tests = new List<TestMethod>(new CombinatorialAttribute().BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(6));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void PairwiseAttribute_NoArgs_NoCases()
        {
            var method = GetMethod("MethodWithoutArgs");
            List<TestMethod> tests = new List<TestMethod>(new PairwiseAttribute().BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(0));
        }

        [Test]
        public void PairwiseAttribute_WithArgs_Runnable()
        {
            var method = GetMethod("MethodWithIntValues");
            List<TestMethod> tests = new List<TestMethod>(new PairwiseAttribute().BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(6));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void SequentialAttribute_NoArgs_NoCases()
        {
            var method = GetMethod("MethodWithoutArgs");
            List<TestMethod> tests = new List<TestMethod>(new SequentialAttribute().BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(0));
        }

        [Test]
        public void SequentialAttribute_WithArgs_Runnable()
        {
            var method = GetMethod("MethodWithIntValues");
            List<TestMethod> tests = new List<TestMethod>(new SequentialAttribute().BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(3));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        private IMethodInfo GetMethod(string methodName)
        {
            return new MethodWrapper(GetType(), methodName);
        }

        public static void MethodWithoutArgs() { }
        public static void MethodWithIntArgs(int x, int y) { }
        public static void MethodWithIntValues(
            [Values(1, 2, 3)]int x,
            [Values(10, 20)]int y) { }

        static object[] GoodData = new object[] {
            new object[] { 12, 3 },
            new object[] { 12, 4 },
            new object[] { 12, 6 } };

        static object[] BadData = new object[] {
            new object[] { 12, 3, 4 },
            new object[] { 12, 4, 3 },
            new object[] { 12, 6, 2 } };

        [DatapointSource]
        int[] ints = new int[] { 1, 2, 3 };
    }
}
