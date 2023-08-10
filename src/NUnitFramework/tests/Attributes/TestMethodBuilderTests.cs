// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Attributes
{
    public class TestMethodBuilderTests
    {
        #region TestAttribute

        [Test]
        public void TestAttribute_NoArgs_Runnable()
        {
            var method = GetMethod(nameof(MethodWithoutArgs));
            TestMethod test = new TestAttribute().BuildFrom(method, null);
            Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void TestAttribute_WithArgs_NotRunnable()
        {
            var method = GetMethod(nameof(MethodWithIntArgs));
            TestMethod test = new TestAttribute().BuildFrom(method, null);
            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        #endregion

        #region TestCaseAttribute

        [Test]
        public void TestCaseAttribute_NoArgs_NotRunnable()
        {
            var method = GetMethod(nameof(MethodWithoutArgs));
            List<TestMethod> tests = new List<TestMethod>(new TestCaseAttribute(5, 42).BuildFrom(method, null));
            Assert.That(tests, Has.Count.EqualTo(1));
            Assert.That(tests[0].RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void TestCaseAttribute_RightArgs_Runnable()
        {
            var method = GetMethod(nameof(MethodWithIntArgs));
            List<TestMethod> tests = new List<TestMethod>(new TestCaseAttribute(5, 42).BuildFrom(method, null));
            Assert.That(tests, Has.Count.EqualTo(1));
            Assert.That(tests[0].RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void TestCaseAttribute_WrongArgs_NotRunnable()
        {
            var method = GetMethod(nameof(MethodWithIntArgs));
            List<TestMethod> tests = new List<TestMethod>(new TestCaseAttribute(5, 42, 99).BuildFrom(method, null));
            Assert.That(tests, Has.Count.EqualTo(1));
            Assert.That(tests[0].RunState, Is.EqualTo(RunState.NotRunnable));
        }

        #endregion

        #region TestCaseSourceAttribute

        [Test]
        public void TestCaseSourceAttribute_NoArgs_NotRunnable()
        {
            var method = GetMethod(nameof(MethodWithoutArgs));
            List<TestMethod> tests = new List<TestMethod>(new TestCaseSourceAttribute(nameof(GoodData)).BuildFrom(method, null));
            Assert.That(tests, Has.Count.EqualTo(3));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void TestCaseSourceAttribute_NoArgs_NoData_NotRunnable()
        {
            var method = GetMethod(nameof(MethodWithoutArgs));
            List<TestMethod> tests = new List<TestMethod>(new TestCaseSourceAttribute(nameof(ZeroData)).BuildFrom(method, null));
            Assert.That(tests, Has.Count.EqualTo(1));
            Assert.That(tests[0].RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void TestCaseSourceAttribute_RightArgs_Runnable()
        {
            var method = GetMethod(nameof(MethodWithIntArgs));
            List<TestMethod> tests = new List<TestMethod>(new TestCaseSourceAttribute(nameof(GoodData)).BuildFrom(method, null));
            Assert.That(tests, Has.Count.EqualTo(3));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        [Test]
        public void TestCaseSourceAttribute_WrongArgs_NotRunnable()
        {
            var method = GetMethod(nameof(MethodWithIntArgs));
            List<TestMethod> tests = new List<TestMethod>(new TestCaseSourceAttribute(nameof(BadData)).BuildFrom(method, null));
            Assert.That(tests, Has.Count.EqualTo(3));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
        }

        #endregion

        #region TheoryAttribute

        [Test]
        public void TheoryAttribute_NoArgs_NoCases()
        {
            var method = GetMethod(nameof(MethodWithoutArgs));
            List<TestMethod> tests = new List<TestMethod>(new TheoryAttribute().BuildFrom(method, null));
            Assert.That(tests, Is.Empty);
        }

        [Test]
        public void TheoryAttribute_WithArgs_Runnable()
        {
            var method = GetMethod(nameof(MethodWithIntArgs));
            List<TestMethod> tests = new List<TestMethod>(new TheoryAttribute().BuildFrom(method, null));
            Assert.That(tests, Has.Count.EqualTo(9));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        #endregion

        #region CombinatorialAttribute

        [Test]
        public void CombinatorialAttribute_NoArgs_NoCases()
        {
            var method = GetMethod(nameof(MethodWithoutArgs));
            List<TestMethod> tests = new List<TestMethod>(new CombinatorialAttribute().BuildFrom(method, null));
            Assert.That(tests, Is.Empty);
        }

        [Test]
        public void CombinatorialAttribute_WithArgs_Runnable()
        {
            var method = GetMethod(nameof(MethodWithIntValues));
            List<TestMethod> tests = new List<TestMethod>(new CombinatorialAttribute().BuildFrom(method, null));
            Assert.That(tests, Has.Count.EqualTo(6));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        #endregion

        #region PairwiseAttribute

        [Test]
        public void PairwiseAttribute_NoArgs_NoCases()
        {
            var method = GetMethod(nameof(MethodWithoutArgs));
            List<TestMethod> tests = new List<TestMethod>(new PairwiseAttribute().BuildFrom(method, null));
            Assert.That(tests, Is.Empty);
        }

        [Test]
        public void PairwiseAttribute_WithArgs_Runnable()
        {
            var method = GetMethod(nameof(MethodWithIntValues));
            List<TestMethod> tests = new List<TestMethod>(new PairwiseAttribute().BuildFrom(method, null));
            Assert.That(tests, Has.Count.EqualTo(6));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        #endregion

        #region SequentialAttribute

        [Test]
        public void SequentialAttribute_NoArgs_NoCases()
        {
            var method = GetMethod(nameof(MethodWithoutArgs));
            List<TestMethod> tests = new List<TestMethod>(new SequentialAttribute().BuildFrom(method, null));
            Assert.That(tests, Is.Empty);
        }

        [Test]
        public void SequentialAttribute_WithArgs_Runnable()
        {
            var method = GetMethod(nameof(MethodWithIntValues));
            List<TestMethod> tests = new List<TestMethod>(new SequentialAttribute().BuildFrom(method, null));
            Assert.That(tests, Has.Count.EqualTo(3));
            foreach (var test in tests)
                Assert.That(test.RunState, Is.EqualTo(RunState.Runnable));
        }

        #endregion

        #region Helper Methods and Data

        private IMethodInfo GetMethod(string methodName)
        {
            return new MethodWrapper(GetType(), methodName);
        }

        // These are indirect test method and therefore need to be public
#pragma warning disable NUnit1028 // The non-test method is public
        public static void MethodWithoutArgs() { }
        public static void MethodWithIntArgs(int x, int y) { }
        public static void MethodWithIntValues(
            [Values(1, 2, 3)] int x,
            [Values(10, 20)] int y)
        { }
#pragma warning restore NUnit1028 // The non-test method is public

        private static readonly object[] ZeroData = Array.Empty<object>();

        private static readonly object[] GoodData = new object[] {
            new object[] { 12, 3 },
            new object[] { 12, 4 },
            new object[] { 12, 6 } };

        private static readonly object[] BadData = new object[] {
            new object[] { 12, 3, 4 },
            new object[] { 12, 4, 3 },
            new object[] { 12, 6, 2 } };

        [DatapointSource]
        private readonly int[] _ints = new int[] { 1, 2, 3 };

        #endregion
    }
}
