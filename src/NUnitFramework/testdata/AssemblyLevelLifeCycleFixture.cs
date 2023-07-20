// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestData.LifeCycleTests
{
    public static class AssemblyLevelFixtureLifeCycleTest
    {
        public const string Code = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            [TestFixture]
            public class FixtureUnderTest : NUnit.TestData.LifeCycleTests.BaseLifeCycle
            {
                private int _value;

                [Test]
                public void Test1()
                {
                    Assert.That(_value++,Is.EqualTo(0));
                }

                [Test]
                public void Test2()
                {
                    Assert.That(_value++,Is.EqualTo(0));
                }
            }
            ";
    }

    public static class OverrideAssemblyLevelFixtureLifeCycleTest
    {
        public const string Code = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            [TestFixture]
            [FixtureLifeCycle(LifeCycle.SingleInstance)]
            public class FixtureUnderTest : NUnit.TestData.LifeCycleTests.BaseLifeCycle
            {
                private int _value;

                [Test]
                [Order(0)]
                public void Test1()
                {
                    Assert.That(_value++,Is.EqualTo(0));
                }

                [Test]
                [Order(1)]
                public void Test2()
                {
                    Assert.That(_value++,Is.EqualTo(1));
                }
            }
            ";
    }

    public static class NestedOverrideAssemblyLevelFixtureLifeCycleTest
    {
        public const string OuterClass = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            [TestFixture]
            [FixtureLifeCycle(LifeCycle.SingleInstance)]
            public class FixtureUnderTest
            {
                public class NestedFixture : NUnit.TestData.LifeCycleTests.BaseLifeCycle
                {
                    private int _value;

                    [Test]
                    [Order(0)]
                    public void Test1()
                    {
                        Assert.That(_value++,Is.EqualTo(0));
                    }

                    [Test]
                    [Order(1)]
                    public void Test2()
                    {
                        Assert.That(_value++,Is.EqualTo(1));
                    }
                }
            }
            ";

        public const string InnerClass = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            [TestFixture]
            public class FixtureUnderTest
            {
                [FixtureLifeCycle(LifeCycle.SingleInstance)]
                public class NestedFixture : NUnit.TestData.LifeCycleTests.BaseLifeCycle
                {
                    private int _value;

                    [Test]
                    [Order(0)]
                    public void Test1()
                    {
                        Assert.That(_value++,Is.EqualTo(0));
                    }

                    [Test]
                    [Order(1)]
                    public void Test2()
                    {
                        Assert.That(_value++,Is.EqualTo(1));
                    }
                }
            }
            ";
    }

    public static class InheritedOverrideTest
    {
        public const string InheritClassWithOtherLifecycle = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            [FixtureLifeCycle(LifeCycle.SingleInstance)]
            public class Base : NUnit.TestData.LifeCycleTests.BaseLifeCycle
            {
            }

            [TestFixture]
            public class FixtureUnderTest : Base
            {
                private int _value;

                [Test]
                [Order(0)]
                public void Test1()
                {
                    Assert.That(_value++,Is.EqualTo(0));
                }

                [Test]
                [Order(1)]
                public void Test2()
                {
                    Assert.That(_value++,Is.EqualTo(1));
                }
            }
            ";

        public const string InheritClassWithOtherLifecycleFromOtherAssembly = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            [TestFixture]
            public class FixtureUnderTest : NUnit.TestData.LifeCycleTests.LifeCycleInheritanceBaseSingleInstance
            {
                private int _value;

                [Test]
                [Order(0)]
                public void Test1()
                {
                    Assert.That(_value++,Is.EqualTo(0));
                }

                [Test]
                [Order(1)]
                public void Test2()
                {
                    Assert.That(_value++,Is.EqualTo(1));
                }
            }
            ";
    }

    public static class AssemblyLevelLifeCycleTestFixtureSourceTest
    {
        public const string Code = @"
            using NUnit.Framework;

            [assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]

            [TestFixtureSource(""FixtureUnderTest"")]
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
                    Assert.That(_value++,Is.EqualTo(_initialValue));
                }

                [Test]
                public void Test2()
                {
                    Assert.That(_value++,Is.EqualTo(_initialValue));
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
                    Assert.That(_counter++,Is.EqualTo(0));
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
                     Assert.That(_counter++,Is.EqualTo(0));
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
                     Assert.That(_counter++,Is.EqualTo(0));
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
                     Assert.That(_counter++,Is.EqualTo(0));
                }
            }
            ";
    }
}
