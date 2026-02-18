// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Attributes;
using NUnit.Framework.Interfaces;

namespace NUnit.TestData
{
#pragma warning disable CS9113 // Parameter is unread.
#pragma warning disable IDE0060 // Remove unused parameter
    public static class NoTestsAttributeFixture
    {
        public static TestCaseData[] EmptyData => [];

        public static class TestCaseSource
        {
            public class MethodSetsDefaultStatus
            {
                [NoTests(TestStatus.Passed)]
                [TestCaseSource(typeof(NoTestsAttributeFixture), nameof(EmptyData))]
                public static void MethodSetsPassed(int actual, int expected)
                {
                    Assert.That(actual, Is.EqualTo(expected));
                }

                [TestCaseSource(typeof(NoTestsAttributeFixture), nameof(EmptyData))]
                public static void MethodDoesntSpecify(int actual, int expected)
                {
                    Assert.That(actual, Is.EqualTo(expected));
                }
            }

            [TestFixture]
            [NoTests(TestStatus.Passed)]
            public class FixtureOverridesDefaultStatus
            {
                [TestCaseSource(typeof(NoTestsAttributeFixture), nameof(EmptyData))]
                public static void NoMethodLevelOverride(int actual, int expected)
                {
                    Assert.That(actual, Is.EqualTo(expected));
                }

                [NoTests(TestStatus.Inconclusive)]
                [TestCaseSource(typeof(NoTestsAttributeFixture), nameof(EmptyData))]
                public static void WithMethodLevelOverride(int actual, int expected)
                {
                    Assert.That(actual, Is.EqualTo(expected));
                }
            }
        }

        public static class TestFixtureSource
        {
            [TestFixtureSource(typeof(NoTestsAttributeFixture), nameof(EmptyData))]
            public class FixtureSetsDefaultStatus(string arg)
            {
                [Test]
                public void Test()
                {
                }
            }

            [NoTests(TestStatus.Passed)]
            [TestFixtureSource(typeof(NoTestsAttributeFixture), nameof(EmptyData))]
            public class FixtureOverridesDefaultStatus(string arg)
            {
                [Test]
                public void Test()
                {
                }
            }
        }

        public static class ValueSource
        {
            public class MethodSetsDefaultStatus
            {
                [Test]
                [NoTests(TestStatus.Passed)]
                public static void MethodSetsPassed(
                    [ValueSource(typeof(NoTestsAttributeFixture), nameof(EmptyData))] int actual)
                {
                }

                [Test]
                public static void MethodDoesntSpecify(
                    [ValueSource(typeof(NoTestsAttributeFixture), nameof(EmptyData))] int actual)
                {
                }
            }

            [TestFixture]
            [NoTests(TestStatus.Passed)]
            public class FixtureOverridesDefaultStatus
            {
                [Test]
                public static void NoMethodLevelOverride(
                    [ValueSource(typeof(NoTestsAttributeFixture), nameof(EmptyData))] int actual)
                {
                }

                [Test]
                [NoTests(TestStatus.Inconclusive)]
                public static void WithMethodLevelOverride(
                    [ValueSource(typeof(NoTestsAttributeFixture), nameof(EmptyData))] int actual)
                {
                }
            }
        }

        public static class Theory
        {
            public class MethodSetsDefaultStatus
            {
                [Theory]
                [NoTests(TestStatus.Passed)]
                public static void MethodSetsPassed(int x)
                {
                }

                [Theory]
                public static void MethodDoesntSpecify(int x)
                {
                }
            }

            [TestFixture]
            [NoTests(TestStatus.Passed)]
            public class FixtureOverridesDefaultStatus
            {
                [Theory]
                public static void NoMethodLevelOverride(int x)
                {
                }

                [Theory]
                [NoTests(TestStatus.Inconclusive)]
                public static void WithMethodLevelOverride(int x)
                {
                }
            }
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CS9113 // Parameter is unread.
}
