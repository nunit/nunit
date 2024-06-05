// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    internal class GenericTestMethodTests
    {
#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
        [TestCase(5, 2, "ABC")]
        [TestCase(5.0, 2.0, "ABC")]
        [TestCase(5, 2.0, "ABC")]
        [TestCase(5.0, 2L, "ABC")]
        public void TestCase_OneTypeParameterOnTwoArgs<T>(T x, T y, string label)
        {
            Assert.Multiple(() =>
            {
                Assert.That(x, Is.EqualTo(5));
                Assert.That(y, Is.EqualTo(2));
                Assert.That(label, Is.EqualTo("ABC"));
            });
        }

        [Test]
        public void TestCase_IncompatibleArgsAreNotRunnable()
        {
            var result = TestBuilder.RunTestFixture(typeof(IncompatibleGenericTestCaseData));
            Assert.Multiple(() =>
            {
                Assert.That(result.PassCount, Is.EqualTo(2), "PassCount");
                Assert.That(result.FailCount, Is.EqualTo(2), "FailCount");
            });

            int invalid = 0;
            // Examine grandchildren - child is parameterized method suite
            var suiteResult = result.Children.ToArray()[0];
            foreach (var childResult in suiteResult.Children)
            {
                if (childResult.ResultState == ResultState.NotRunnable)
                    invalid++;
            }

            Assert.That(invalid, Is.EqualTo(2), "Invalid count");
        }

        [TestCase(5, 2, "ABC")]
        [TestCase(5.0, 2.0, "ABC")]
        [TestCase(5, 2.0, "ABC")]
        [TestCase(5.0, 2L, "ABC")]
        public void TestCase_TwoTypeParameters<T1, T2>(T1 x, T2 y, string label)
        {
            Assert.Multiple(() =>
            {
                Assert.That(x, Is.EqualTo(5));
                Assert.That(y, Is.EqualTo(2));
                Assert.That(label, Is.EqualTo("ABC"));
            });
        }

        [TestCase(5, 2, "ABC")]
        [TestCase(5.0, 2.0, "ABC")]
        [TestCase(5, 2.0, "ABC")]
        [TestCase(5.0, 2L, "ABC")]
        public void TestCase_TwoTypeParameters_Reversed<T1, T2>(T2 x, T1 y, string label)
        {
            Assert.Multiple(() =>
            {
                Assert.That(x, Is.EqualTo(5));
                Assert.That(y, Is.EqualTo(2));
                Assert.That(label, Is.EqualTo("ABC"));
            });
        }

        [TestCaseSource(nameof(Source))]
        public void TestCaseSource_OneTypeParameterOnTwoArgs<T>(T x, T y, string label)
        {
            Assert.Multiple(() =>
            {
                Assert.That(x, Is.EqualTo(5));
                Assert.That(y, Is.EqualTo(2));
                Assert.That(label, Is.EqualTo("ABC"));
            });
        }

        [Test]
        public void TestCaseSource_IncompatibleArgsAreNotRunnable()
        {
            var result = TestBuilder.RunTestFixture(typeof(IncompatibleGenericTestCaseSourceData));
            Assert.Multiple(() =>
            {
                Assert.That(result.PassCount, Is.EqualTo(2), "PassCount");
                Assert.That(result.FailCount, Is.EqualTo(2), "FailCount");
            });

            int invalid = 0;
            // Examine grandchildren - child is parameterized method suite
            var suiteResult = result.Children.ToArray()[0];
            foreach (var childResult in suiteResult.Children)
            {
                if (childResult.ResultState == ResultState.NotRunnable)
                    invalid++;
            }

            Assert.That(invalid, Is.EqualTo(2), "Invalid count");
        }

        [TestCaseSource(nameof(Source))]
        public void TestCaseSource_TwoTypeParameters<T1, T2>(T1 x, T2 y, string label)
        {
            Assert.Multiple(() =>
            {
                Assert.That(x, Is.EqualTo(5));
                Assert.That(y, Is.EqualTo(2));
                Assert.That(label, Is.EqualTo("ABC"));
            });
        }

        // TOOD: Remove when https://github.com/nunit/nunit.analyzers/issues/632 is fixed and released
#pragma warning disable NUnit1031 // The individual arguments provided by a ValuesAttribute must match the type of the corresponding parameter of the method

        [Test]
        public void Combinatorial_OneTypeParameterOnTwoArgs<T>(
            [Values(5, 5.0)] T x,
            [Values(2.0, 2)] T y)
        {
            Assert.Multiple(() =>
            {
                Assert.That(x, Is.EqualTo(5));
                Assert.That(y, Is.EqualTo(2));
            });
        }

        [Test]
        public void Combinatorial_TwoTypeParameters<T1, T2>(
            [Values(5, 5.0)] T1 x,
            [Values(2.0, 2)] T2 y)
        {
            Assert.Multiple(() =>
            {
                Assert.That(x, Is.EqualTo(5));
                Assert.That(y, Is.EqualTo(2));
            });
        }
#pragma warning restore NUnit1031 // The individual arguments provided by a ValuesAttribute must match the type of the corresponding parameter of the method

        [Test]
        public void Combinatorial_IncompatibleArgsAreNotRunnable()
        {
            var result = TestBuilder.RunTestFixture(typeof(IncompatibleGenericCombinatorialData));
            Assert.That(result.PassCount, Is.EqualTo(2), "PassCount");
            Assert.That(result.FailCount, Is.EqualTo(2), "FailCount");

            int invalid = 0;
            // Examine grandchildren - child is parameterized method suite
            var suiteResult = result.Children.ToArray()[0];
            foreach (var childResult in suiteResult.Children)
            {
                if (childResult.ResultState == ResultState.NotRunnable)
                    invalid++;
            }

            Assert.That(invalid, Is.EqualTo(2), "Invalid count");
        }

        [Test]
        public void TestCase_MissingGenericArgumentAreNotRunnable()
        {
            //Need to use full fixture, since targeting one test at the time crashed during buildup and not in the framework
            ITestResult result = TestBuilder.RunTestFixture(typeof(NotRunnableGenericData));
            Assert.That(result.PassCount, Is.EqualTo(1), "PassCount");
            Assert.That(result.TotalCount, Is.EqualTo(3), "TotalCount");

            var voidTest = result.Children.Single(c => c.Name == nameof(NotRunnableGenericData.TestWithGeneric_ReturningVoid_ThatIsUnRunnable) + "<T>");
            Assert.That(voidTest.ResultState, Is.EqualTo(ResultState.NotRunnable));

            var returnTest = result.Children.Single(c => c.Name == nameof(NotRunnableGenericData.TestWithGeneric_ReturningGenericType_ThatIsUnRunnable));
            Assert.That(returnTest.Children.First().ResultState, Is.EqualTo(ResultState.NotRunnable));

            var validTest = result.Children.Single(c => c.Name == nameof(NotRunnableGenericData.TestWithGeneric_PassingInGenericParameter_ThatIsRunnable));
            Assert.That(validTest.ResultState, Is.EqualTo(ResultState.Success));
        }

        private static readonly object[] Source = new object[]
        {
            new object[] { 5, 2, "ABC" },
            new object[] { 5.0, 2.0, "ABC" },
            new object[] { 5, 2.0, "ABC" },
            new object[] { 5.0, 2L, "ABC" }
        };

        //[TestCaseSource("SequenceCases")]
        //public void SequenceEquality<TSource>(IEnumerable<TSource> left, IEnumerable<TSource> right)
        //{
        //    Assert.That(left, Is.EqualTo(right));
        //}

        //static ITestCaseData[] SequenceCases = {
        //    new TestCaseData(new List<int> { 1, 2 }, new List<int> { 1, 2 }) };
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
    }
}
