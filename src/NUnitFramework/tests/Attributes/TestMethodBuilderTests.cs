// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Attributes
{
    public class TestMethodBuilderTests
    {
        #region TestAttribute

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

        #endregion

        #region TestCaseAttribute

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

        #endregion

        #region TestCaseSourceAttribute

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
        public void TestCaseSourceAttribute_NoArgs_NoData_NotRunnable()
        {
            var method = GetMethod("MethodWithoutArgs");
            List<TestMethod> tests = new List<TestMethod>(new TestCaseSourceAttribute("ZeroData").BuildFrom(method, null));
            Assert.That(tests.Count, Is.EqualTo(1));
            Assert.That(tests[0].RunState, Is.EqualTo(RunState.NotRunnable));
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

        #endregion

        #region TheoryAttribute

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

        #endregion

        #region CombinatorialAttribute

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

        #endregion

        #region PairwiseAttribute

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

        #endregion

        #region SequentialAttribute

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

        #endregion

        #region Helper Methods and Data

        private IMethodInfo GetMethod(string methodName)
        {
            return new MethodWrapper(GetType(), methodName);
        }

        public static void MethodWithoutArgs() { }
        public static void MethodWithIntArgs(int x, int y) { }
        public static void MethodWithIntValues(
            [Values(1, 2, 3)]int x,
            [Values(10, 20)]int y) { }

        static object[] ZeroData = Array.Empty<object>();

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

        #endregion
    }
}
