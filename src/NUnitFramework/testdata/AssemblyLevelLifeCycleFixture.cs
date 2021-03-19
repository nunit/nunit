// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestData.LifeCycleTests
{
    public static class AssemblyLevelFixtureLifeCycleTest
    {
        public const string Code = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            [TestFixture]
            public class FixtureUnderTest
            {
                private int _value;

                [Test]
                public void Test1()
                {
                    Assert.AreEqual(0, _value++);
                }

                [Test]
                public void Test2()
                {
                    Assert.AreEqual(0, _value++);
                }
            }
            ";
    }

    public static class AssemblyLevelLifeCycleTestFixtureSourceTest
    {
        public const string Code = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            [TestFixtureSource(""FixtureArgs"")]
            public class FixtureUnderTest
            {
                private readonly int _initialValue;
                private int _value;

                public FixtureUnderTest(int num)
                {
                    _initialValue = num;
                    _value = num;
                }

                public static int[] FixtureArgs()
                {
                    return new int[] { 1, 42 };
                }

                [Test]
                public void Test1()
                {
                    Assert.AreEqual(_initialValue, _value++);
                }

                [Test]
                public void Test2()
                {
                    Assert.AreEqual(_initialValue, _value++);
                }
            }
            ";
    }

    public static class AssemblyLevelLifeCycleFixtureWithTestCasesTest
    {
        public const string Code = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            public class FixtureUnderTest
            {
                private int _counter;

                [TestCase(0)]
                [TestCase(1)]
                [TestCase(2)]
                [TestCase(3)]
                public void Test(int _)
                {
                    Assert.AreEqual(0, _counter++);
                }
            }
            ";
    }

    public static class AssemblyLevelLifeCycleFixtureWithTestCaseSourceTest
    {
        public const string Code = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            public class FixtureUnderTest
            {
                private int _counter;

                public static int[] Args()
                {
                    return new int[] { 1, 42 };
                }

                [TestCaseSource(""Args"")]
                public void Test(int _)
                {
                    Assert.AreEqual(0, _counter++);
                }
            }
            ";
    }

    public static class AssemblyLevelLifeCycleFixtureWithValuesTest
    {
        public const string Code = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            public class FixtureUnderTest
            {
                private int _counter;

                [Test]
                public void Test([Values] bool? _)
                {
                    Assert.AreEqual(0, _counter++);
                }
            }
            ";
    }

    public static class AssemblyLevelLifeCycleFixtureWithTheoryTest
    {
        public const string Code = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            public class FixtureUnderTest
            {
                private int _counter;

                [Theory]
                public void Test(bool? _)
                {
                    Assert.AreEqual(0, _counter++);
                }
            }
            ";
    }
}
