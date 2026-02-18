// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using TestCaseSourceTestData = NUnit.TestData.NoTestsAttributeFixture.TestCaseSource;
using TestFixtureSourceTestData = NUnit.TestData.NoTestsAttributeFixture.TestFixtureSource;
using TheoryTestData = NUnit.TestData.NoTestsAttributeFixture.Theory;
using ValueSourceTestData = NUnit.TestData.NoTestsAttributeFixture.ValueSource;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    internal static class NoTestsAttributeTests
    {
        public static class TestFixtureSourceCompatibility
        {
            [Test]
            public static void EmptySource_UsesDefaultStatus()
            {
                var fixture = TestBuilder.MakeFixture(typeof(TestFixtureSourceTestData.FixtureSetsDefaultStatus));

                Assert.That(fixture.RunState, Is.EqualTo(RunState.Runnable));

                var result = TestBuilder.RunTest(fixture);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Inconclusive));
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Inconclusive));
            }

            [Test]
            public static void EmptySource_SetsResultExplicitly()
            {
                var fixture = TestBuilder.MakeFixture(typeof(TestFixtureSourceTestData.FixtureOverridesDefaultStatus));

                Assert.That(fixture.RunState, Is.EqualTo(RunState.Runnable));

                var result = TestBuilder.RunTest(fixture);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            }
        }

        public static class TestCaseSourceCompatibility
        {
            [Test]
            public static void EmptySource_OverridesResultFromParent()
            {
                var fixture = TestBuilder.MakeFixture(typeof(TestCaseSourceTestData.FixtureOverridesDefaultStatus));

                Assert.That(fixture.RunState, Is.EqualTo(RunState.Runnable));

                var test = fixture.Tests.Single(x => x.Name == nameof(TestCaseSourceTestData.FixtureOverridesDefaultStatus.WithMethodLevelOverride)) as Test;
                var result = TestBuilder.RunTest(test!);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Inconclusive));
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Inconclusive));
            }

            [Test]
            public static void EmptySource_SetsResultOnParent()
            {
                var fixture = TestBuilder.MakeFixture(typeof(TestCaseSourceTestData.FixtureOverridesDefaultStatus));

                Assert.That(fixture.RunState, Is.EqualTo(RunState.Runnable));

                var test = fixture.Tests.Single(x => x.Name == nameof(TestCaseSourceTestData.FixtureOverridesDefaultStatus.NoMethodLevelOverride)) as Test;
                var result = TestBuilder.RunTest(test!);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            }

            [Test]
            public static void EmptySource_ExplicitlySetsResultOnMethod()
            {
                var suite = TestBuilder.MakeParameterizedMethodSuite(
                    typeof(TestCaseSourceTestData.MethodSetsDefaultStatus),
                    nameof(TestCaseSourceTestData.MethodSetsDefaultStatus.MethodSetsPassed));

                Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));

                var result = TestBuilder.RunTest(suite);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            }

            [Test]
            public static void EmptySource_UsesDefaultResultOnMethod()
            {
                var suite = TestBuilder.MakeParameterizedMethodSuite(
                    typeof(TestCaseSourceTestData.MethodSetsDefaultStatus),
                    nameof(TestCaseSourceTestData.MethodSetsDefaultStatus.MethodDoesntSpecify));

                Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));

                var result = TestBuilder.RunTest(suite);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Inconclusive));
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Inconclusive));
            }
        }

        public static class ValueSourceCompatibility
        {
            [Test]
            public static void EmptySource_OverridesResultFromParent()
            {
                var fixture = TestBuilder.MakeFixture(typeof(ValueSourceTestData.FixtureOverridesDefaultStatus));

                Assert.That(fixture.RunState, Is.EqualTo(RunState.Runnable));

                var test = fixture.Tests.Single(x => x.Name == nameof(ValueSourceTestData.FixtureOverridesDefaultStatus.WithMethodLevelOverride)) as Test;
                var result = TestBuilder.RunTest(test!);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Inconclusive));
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Inconclusive));
            }

            [Test]
            public static void EmptySource_SetsResultOnParent()
            {
                var fixture = TestBuilder.MakeFixture(typeof(ValueSourceTestData.FixtureOverridesDefaultStatus));

                Assert.That(fixture.RunState, Is.EqualTo(RunState.Runnable));

                var test = fixture.Tests.Single(x => x.Name == nameof(ValueSourceTestData.FixtureOverridesDefaultStatus.NoMethodLevelOverride)) as Test;
                var result = TestBuilder.RunTest(test!);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            }

            [Test]
            public static void EmptySource_ExplicitlySetsResultOnMethod()
            {
                var suite = TestBuilder.MakeParameterizedMethodSuite(
                    typeof(ValueSourceTestData.MethodSetsDefaultStatus),
                    nameof(ValueSourceTestData.MethodSetsDefaultStatus.MethodSetsPassed));

                Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));

                var result = TestBuilder.RunTest(suite);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
            }

            [Test]
            public static void EmptySource_UsesDefaultResultOnMethod()
            {
                var suite = TestBuilder.MakeParameterizedMethodSuite(
                    typeof(ValueSourceTestData.MethodSetsDefaultStatus),
                    nameof(ValueSourceTestData.MethodSetsDefaultStatus.MethodDoesntSpecify));

                Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));

                var result = TestBuilder.RunTest(suite);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Inconclusive));
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Inconclusive));
            }
        }

        public static class TheoryIncompatibility
        {
            [Test]
            public static void EmptySource_OverridesResultFromParent()
            {
                var fixture = TestBuilder.MakeFixture(typeof(TheoryTestData.FixtureOverridesDefaultStatus));

                Assert.That(fixture.RunState, Is.EqualTo(RunState.Runnable));

                var test = fixture.Tests.Single(x => x.Name == nameof(TheoryTestData.FixtureOverridesDefaultStatus.WithMethodLevelOverride)) as Test;
                var result = TestBuilder.RunTest(test!);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            }

            [Test]
            public static void EmptySource_SetsResultOnParent()
            {
                var fixture = TestBuilder.MakeFixture(typeof(TheoryTestData.FixtureOverridesDefaultStatus));

                Assert.That(fixture.RunState, Is.EqualTo(RunState.Runnable));

                var test = fixture.Tests.Single(x => x.Name == nameof(TheoryTestData.FixtureOverridesDefaultStatus.NoMethodLevelOverride)) as Test;
                var result = TestBuilder.RunTest(test!);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            }

            [Test]
            public static void EmptySource_ExplicitlySetsResultOnMethod()
            {
                var suite = TestBuilder.MakeParameterizedMethodSuite(
                    typeof(TheoryTestData.MethodSetsDefaultStatus),
                    nameof(TheoryTestData.MethodSetsDefaultStatus.MethodSetsPassed));

                Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));

                var result = TestBuilder.RunTest(suite);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            }

            [Test]
            public static void EmptySource_UsesDefaultResultOnMethod()
            {
                var suite = TestBuilder.MakeParameterizedMethodSuite(
                    typeof(TheoryTestData.MethodSetsDefaultStatus),
                    nameof(TheoryTestData.MethodSetsDefaultStatus.MethodDoesntSpecify));

                Assert.That(suite.RunState, Is.EqualTo(RunState.Runnable));

                var result = TestBuilder.RunTest(suite);

                Assert.That(result.ResultState, Is.EqualTo(ResultState.Failure));
            }
        }
    }
}
