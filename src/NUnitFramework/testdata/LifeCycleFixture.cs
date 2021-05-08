// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt


using System;
using NUnit.Framework;

namespace NUnit.TestData.LifeCycleTests
{
    #region Basic Lifecycle
    [TestFixture]
    public class CountingLifeCycleTestFixture
    {
        public int Count { get; set; }

        [Test]
        public void CountIsAlwaysOne()
        {
            Count++;
            Assert.AreEqual(1, Count);
        }

        [Test]
        public void CountIsAlwaysOne_2()
        {
            Count++;
            Assert.AreEqual(1, Count);
        }
    }

    [TestFixture]
    public class FullLifecycleTestCase : IDisposable
    {
        public static int ConstructCount;
        public static int DisposeCount;
        public static int OneTimeSetUpCount;
        public static int OneTimeTearDownCount;

        public static int SetUpCountTotal;
        public static int TearDownCountTotal;

        public int InstanceSetUpCount;
        public int InstanceTearDownCount;

        public static void Reset()
        {
            ConstructCount = 0;
            DisposeCount = 0;
            OneTimeSetUpCount = 0;
            OneTimeTearDownCount = 0;
            SetUpCountTotal = 0;
            TearDownCountTotal = 0;
        }

        public FullLifecycleTestCase()
        {
            ConstructCount++;
        }

        public void Dispose()
        {
            DisposeCount++;
        }

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            OneTimeSetUpCount++;
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            OneTimeTearDownCount++;
        }

        [SetUp]
        public void SetUp()
        {
            SetUpCountTotal++;
            InstanceSetUpCount++;
        }

        [TearDown]
        public void TearDown()
        {
            TearDownCountTotal++;
            InstanceTearDownCount++;
        }

        [Test]
        public void DummyTest1()
        {
            Assert.That(ConstructCount - DisposeCount, Is.EqualTo(1));

            Assert.That(OneTimeSetUpCount, Is.EqualTo(1));
            Assert.That(OneTimeTearDownCount, Is.EqualTo(0));

            Assert.That(InstanceSetUpCount, Is.EqualTo(1));
            Assert.That(InstanceTearDownCount, Is.EqualTo(0));
        }

        [Test]
        public void DummyTest2()
        {
            Assert.That(ConstructCount - DisposeCount, Is.EqualTo(1));

            Assert.That(OneTimeSetUpCount, Is.EqualTo(1));
            Assert.That(OneTimeTearDownCount, Is.EqualTo(0));

            Assert.That(InstanceSetUpCount, Is.EqualTo(1));
            Assert.That(InstanceTearDownCount, Is.EqualTo(0));
        }

        [Test]
        public void DummyTest3()
        {
            Assert.That(ConstructCount - DisposeCount, Is.EqualTo(1));

            Assert.That(OneTimeSetUpCount, Is.EqualTo(1));
            Assert.That(OneTimeTearDownCount, Is.EqualTo(0));

            Assert.That(InstanceSetUpCount, Is.EqualTo(1));
            Assert.That(InstanceTearDownCount, Is.EqualTo(0));
        }
    }

    #endregion

    #region Validation

    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class InstanceOneTimeSetupAndTearDownFixtureInstancePerTestCase
    {
        public int TotalOneTimeSetupCount = 0;
        public int TotalOneTimeTearDownCount = 0;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            TotalOneTimeSetupCount++;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TotalOneTimeTearDownCount++;
        }

        [Test]
        public void DummyTest1() { }
    }
    #endregion

    #region Test Annotations
    [TestFixtureSource(nameof(FixtureArgs))]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class LifeCycleWithTestFixtureSourceFixture : IDisposable
    {
        private readonly int _initialValue;
        private int _value;

        public static int DisposeCalls { get; set; }

        public LifeCycleWithTestFixtureSourceFixture(int num)
        {
            _initialValue = num;
            _value = num;
        }

        public static int[] FixtureArgs() => new[] { 1, 42 };

        public void Dispose() => DisposeCalls++;

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

    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class FixtureWithTestCases : IDisposable
    {
        private int _counter;

        public static int DisposeCalls { get; set; }

        public void Dispose() => DisposeCalls++;

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Test(int _)
        {
            Assert.AreEqual(0, _counter++);
        }
    }

    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class FixtureWithTestCaseSource : IDisposable
    {
        private int _counter;

        public static int DisposeCalls { get; set; }
        public static int[] Args() => new[] { 1, 42 };

        public void Dispose() => DisposeCalls++;

        [TestCaseSource(nameof(Args))]
        public void Test(int _)
        {
            Assert.AreEqual(0, _counter++);
        }
    }

    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class FixtureWithValuesAttributeTest : IDisposable
    {
        private int _counter;

        public static int DisposeCalls { get; set; }

        public void Dispose() => DisposeCalls++;

        [Test]
        public void Test([Values] bool? _)
        {
            Assert.AreEqual(0, _counter++);
        }
    }

    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class FixtureWithTheoryTest : IDisposable
    {
        private int _counter;

        public static int DisposeCalls { get; set; }

        public void Dispose() => DisposeCalls++;

        [Theory]
        public void Test(bool? _)
        {
            Assert.AreEqual(0, _counter++);
        }
    }

    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class RepeatingLifeCycleFixtureInstancePerTestCase
    {
        public int Counter { get; set; }
        public static int RepeatCounter { get; set; }

        [Test]
        [Repeat(3)]
        public void CounterShouldAlwaysStartAsZero()
        {
            Counter++;
            RepeatCounter++;
            Assert.That(Counter, Is.EqualTo(1));
        }
    }

    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.All)]
    public class ParallelLifeCycleFixtureInstancePerTestCase
    {
        int _setupCount = 0;

        [SetUp]
        public void SetUp()
        {
            _setupCount++;
        }

        [Test]
        public void DummyTest1()
        {
            Assert.That(_setupCount, Is.EqualTo(1));
        }

        [Test]
        public void DummyTest2()
        {
            Assert.That(_setupCount, Is.EqualTo(1));
        }

        [Test]
        public void DummyTest3()
        {
            Assert.That(_setupCount, Is.EqualTo(1));
        }
    }
    #endregion

    #region Nesting and inheritance
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class LifeCycleWithNestedFixture
    {
        public class NestedFixture
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
    }

    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class LifeCycleWithNestedOverridingFixture
    {
        [FixtureLifeCycle(LifeCycle.SingleInstance)]
        public class NestedFixture
        {
            private int _value;

            [Test]
            [Order(0)]
            public void Test1()
            {
                Assert.AreEqual(0, _value++);
            }

            [Test]
            [Order(1)]
            public void Test2()
            {
                Assert.AreEqual(1, _value++);
            }
        }
    }

    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class LifeCycleInheritanceBase
    {
    }

    [TestFixture]
    public class LifeCycleInheritedFixture : LifeCycleInheritanceBase
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

    [TestFixture]
    [FixtureLifeCycle(LifeCycle.SingleInstance)]
    public class LifeCycleInheritanceOverriddenFixture : LifeCycleInheritanceBase
    {
        private int _value;

        [Test]
        [Order(0)]
        public void Test1()
        {
            Assert.AreEqual(0, _value++);
        }

        [Test]
        [Order(1)]
        public void Test2()
        {
            Assert.AreEqual(1, _value++);
        }
    }
    #endregion









}
