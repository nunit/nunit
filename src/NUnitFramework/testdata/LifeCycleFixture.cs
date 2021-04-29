// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt


using System;
using NUnit.Framework;

namespace NUnit.TestData.LifeCycleTests
{
    [TestFixture]
    public class DisposableFixture : IDisposable
    {
        public static int DisposeCount = 0;

        [Test]
        public void DummyTest1() { }

        [Test]
        public void DummyTest2() { }

        public void Dispose()
        {
            DisposeCount++;
        }
    }

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
    public class SetupAndTearDownFixtureInstancePerTestCase
    {
        int _totalSetupCount = 0;
        int _totalTearDownCount = 0;

        [SetUp]
        public void Setup()
        {
            _totalSetupCount++;
        }

        [TearDown]
        public void TearDown()
        {
            _totalTearDownCount++;
        }

        [Test]
        public void DummyTest1() 
        {
            Assert.That(_totalSetupCount, Is.EqualTo(1)); 
            Assert.That(_totalTearDownCount, Is.EqualTo(0));
        }

        [Test]
        public void DummyTest2()
        {
            Assert.That(_totalSetupCount, Is.EqualTo(1));
            Assert.That(_totalTearDownCount, Is.EqualTo(0));
        }

        [Test]
        public void DummyTest3()
        {
            Assert.That(_totalSetupCount, Is.EqualTo(1));
            Assert.That(_totalTearDownCount, Is.EqualTo(0));
        }
    }

    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class StaticOneTimeSetupAndTearDownFixtureInstancePerTestCase
    {
        public static int TotalOneTimeSetupCount = 0;
        public static int TotalOneTimeTearDownCount = 0;

        [OneTimeSetUp]
        public static void OneTimeSetup()
        {
            TotalOneTimeSetupCount++;
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            TotalOneTimeTearDownCount++;
        }

        [Test]
        public void DummyTest1() {}

        [Test]
        public void DummyTest2() {}
    }

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
        public void DummyTest1() {}
    }

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
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class DisposableLifeCycleFixtureInstancePerTestCase : IDisposable
    {
        public static int DisposeCalls { get; set; }
        public static int ConstructCalls { get; set; }
        
        public DisposableLifeCycleFixtureInstancePerTestCase()
        {
            ConstructCalls++;
        }
        
        [Test]
        [Order(1)]
        public void TestCase1()
        {
            Assert.That(DisposeCalls, Is.EqualTo(0));
        }

        [Test]
        [Order(2)]
        public void TestCase2()
        {
            Assert.That(DisposeCalls, Is.EqualTo(1));
        }

        [Test]
        [Order(3)]
        public void TestCase3()
        {
            Assert.That(DisposeCalls, Is.EqualTo(2));
        }

        public void Dispose()
        {
            DisposeCalls++;
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
}
