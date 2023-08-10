// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

namespace NUnit.TestData
{
    public class IncompatibleGenericTestCaseData
    {
        [TestCase(5, 2, "ABC")]
        [TestCase(5, "Y", "ABC")]
        [TestCase("X", 2, "ABC")]
        [TestCase("X", "Y", "ABC")]
        public void TestCase_OneTypeParameterOnTwoArgs<T>(T x, T y, string label)
        {
        }
    }

    public class IncompatibleGenericTestCaseSourceData
    {
        [TestCaseSource(nameof(Source))]
        public void TestCaseSource_OneTypeParameterOnTwoArgs<T>(T x, T y, string label)
        {
        }

        private static readonly object[] Source = new object[] {
            new object[] { 5, 2, "ABC" },
            new object[] { 5, "Y", "ABC" },
            new object[] { "X", 2, "ABC" },
            new object[] { "X", "Y", "ABC" }
        };
    }

    public class IncompatibleGenericCombinatorialData
    {
        [Test]
        public void Combinatorial_OneTypeParameterOnTwoArgs<T>(
            [Values(5, "X")] T x,
            [Values(2, "Y")] T y)
        {
        }
    }

    public class NotRunnableGenericData
    {
        [Test]
        public void TestWithGeneric_ReturningVoid_ThatIsUnRunnable<T>()
        {
        }

        [TestCase(ExpectedResult = default)]
        public T TestWithGeneric_ReturningGenericType_ThatIsUnRunnable<T>()
        {
            return default;
        }

        [TestCase(1)]
        public void TestWithGeneric_PassingInGenericParameter_ThatIsRunnable<T>(T parameter)
        {
            Assert.That(parameter, Is.EqualTo(1));
        }
    }
}
