// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NUnit.TestData.LifeCycleTests
{
    /// <summary>
    /// Base class for all LifeCycle tests: provides default implementation for counting
    /// the total number of calls to constructor and setup methods.
    /// </summary>
    public class BaseLifeCycle : IDisposable
    {
        protected static int ConstructCount;
        protected static int DisposeCount;
        protected static int OneTimeSetUpCount;
        protected static int OneTimeTearDownCount;

        protected static int SetUpCountTotal;
        protected static int TearDownCountTotal;

        protected int InstanceSetUpCount;
        protected int InstanceTearDownCount;

        public static void Reset()
        {
            ConstructCount = 0;
            DisposeCount = 0;
            OneTimeSetUpCount = 0;
            OneTimeTearDownCount = 0;
            SetUpCountTotal = 0;
            TearDownCountTotal = 0;
        }

        public static void VerifySingleInstance(int numberOfTests, int numberOfOneTimeSetUps = 1)
        {
            Assert.That(ConstructCount, Is.EqualTo(1), nameof(ConstructCount));
            Assert.That(DisposeCount, Is.EqualTo(1), nameof(DisposeCount));
            Assert.That(OneTimeSetUpCount, Is.EqualTo(numberOfOneTimeSetUps), nameof(OneTimeSetUpCount));
            Assert.That(OneTimeTearDownCount, Is.EqualTo(numberOfOneTimeSetUps), nameof(OneTimeTearDownCount));
            Assert.That(SetUpCountTotal, Is.EqualTo(numberOfTests), nameof(SetUpCountTotal));
            Assert.That(TearDownCountTotal, Is.EqualTo(numberOfTests), nameof(SetUpCountTotal));
        }

        public static void VerifyInstancePerTestCase(int numberOfTests, int numberOfOneTimeSetUps = 1)
        {
            Assert.That(ConstructCount, Is.EqualTo(numberOfTests), nameof(ConstructCount));
            Assert.That(DisposeCount, Is.EqualTo(numberOfTests), nameof(DisposeCount));
            Assert.That(OneTimeSetUpCount, Is.EqualTo(numberOfOneTimeSetUps), nameof(OneTimeSetUpCount));
            Assert.That(OneTimeTearDownCount, Is.EqualTo(numberOfOneTimeSetUps), nameof(OneTimeTearDownCount));
            Assert.That(SetUpCountTotal, Is.EqualTo(numberOfTests), nameof(SetUpCountTotal));
            Assert.That(TearDownCountTotal, Is.EqualTo(numberOfTests), nameof(SetUpCountTotal));
        }

        public BaseLifeCycle()
        {
            Interlocked.Increment(ref ConstructCount);
        }

        public void Dispose()
        {
            Interlocked.Increment(ref DisposeCount);
        }

        [OneTimeSetUp]
        public static void BaseOneTimeSetup()
        {
            Interlocked.Increment(ref OneTimeSetUpCount);
        }

        [OneTimeTearDown]
        public static void BaseOneTimeTearDown()
        {
            Interlocked.Increment(ref OneTimeTearDownCount);
        }

        [SetUp]
        public void BaseSetUp()
        {
            Interlocked.Increment(ref SetUpCountTotal);
            Interlocked.Increment(ref InstanceSetUpCount);
        }

        [TearDown]
        public void BaseTearDown()
        {
            Interlocked.Increment(ref TearDownCountTotal);
            Interlocked.Increment(ref InstanceTearDownCount);
        }
    }

    #region Basic Lifecycle
    [TestFixture]
    public class CountingLifeCycleTestFixture
    {
        public int Count { get; set; }

        [Test]
        public void CountIsAlwaysOne()
        {
            Count++;
            Assert.That(Count, Is.EqualTo(1));
        }

        [Test]
        public void CountIsAlwaysOne_2()
        {
            Count++;
            Assert.That(Count, Is.EqualTo(1));
        }
    }

    [TestFixture]
    public class FullLifecycleTestCase : BaseLifeCycle
    {
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
        public void DummyTest1()
        {
        }
    }
    #endregion

    #region Test Annotations
    [TestFixtureSource(nameof(FixtureArgs))]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class LifeCycleWithTestFixtureSourceFixture : BaseLifeCycle
    {
        private readonly int _initialValue;
        private int _value;

        public LifeCycleWithTestFixtureSourceFixture(int num)
        {
            _initialValue = num;
            _value = num;
        }

        public static int[] FixtureArgs() => new[] { 1, 42 };

        [Test]
        public void Test1()
        {
            Assert.That(_value++, Is.EqualTo(_initialValue));
        }

        [Test]
        public void Test2()
        {
            Assert.That(_value++, Is.EqualTo(_initialValue));
        }
    }

    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class FixtureWithTestCases : BaseLifeCycle
    {
        private int _counter;

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Test(int _)
        {
            Assert.That(_counter++, Is.EqualTo(0));
        }
    }

    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class FixtureWithTestCaseSource : BaseLifeCycle
    {
        private int _counter;

        public static int[] Args() => new[] { 1, 42 };

        [TestCaseSource(nameof(Args))]
        public void Test(int _)
        {
            Assert.That(_counter++, Is.EqualTo(0));
        }
    }

    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class FixtureWithValuesAttributeTest : BaseLifeCycle
    {
        private int _counter;

        [Test]
        public void Test([Values] bool? _)
        {
            Assert.That(_counter++, Is.EqualTo(0));
        }
    }

    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class FixtureWithTheoryTest : BaseLifeCycle
    {
        private int _counter;

        [Theory]
        public void Test(bool? _)
        {
            Assert.That(_counter++, Is.EqualTo(0));
        }
    }

    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class RepeatingLifeCycleFixtureInstancePerTestCase : BaseLifeCycle
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
    public class ParallelLifeCycleFixtureInstancePerTestCase : BaseLifeCycle
    {
        private int _setupCount = 0;

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
        public class NestedFixture : BaseLifeCycle
        {
            private int _value;

            [Test]
            public void Test1()
            {
                Assert.That(_value++, Is.EqualTo(0));
            }

            [Test]
            public void Test2()
            {
                Assert.That(_value++, Is.EqualTo(0));
            }
        }
    }

    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class LifeCycleWithNestedOverridingFixture
    {
        [FixtureLifeCycle(LifeCycle.SingleInstance)]
        public class NestedFixture : BaseLifeCycle
        {
            private int _value;

            [Test]
            [Order(0)]
            public void Test1()
            {
                Assert.That(_value++, Is.EqualTo(0));
            }

            [Test]
            [Order(1)]
            public void Test2()
            {
                Assert.That(_value++, Is.EqualTo(1));
            }
        }
    }

    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class LifeCycleInheritanceBaseInstancePerTestCase : BaseLifeCycle
    {
    }

    /// <remarks>Used implicitly in <see cref="AssemblyLevelFixtureLifeCycleTest"/></remarks>
    [FixtureLifeCycle(LifeCycle.SingleInstance)]
    public class LifeCycleInheritanceBaseSingleInstance : BaseLifeCycle
    {
    }

    [TestFixture]
    public class LifeCycleInheritedFixture : LifeCycleInheritanceBaseInstancePerTestCase
    {
        private int _value;

        [Test]
        public void Test1()
        {
            Assert.That(_value++, Is.EqualTo(0));
        }

        [Test]
        public void Test2()
        {
            Assert.That(_value++, Is.EqualTo(0));
        }
    }

    [TestFixture]
    [FixtureLifeCycle(LifeCycle.SingleInstance)]
    public class LifeCycleInheritanceOverriddenFixture : LifeCycleInheritanceBaseInstancePerTestCase
    {
        private int _value;

        [Test]
        [Order(0)]
        public void Test1()
        {
            Assert.That(_value++, Is.EqualTo(0));
        }

        [Test]
        [Order(1)]
        public void Test2()
        {
            Assert.That(_value++, Is.EqualTo(1));
        }
    }
    #endregion

    #region DisposeOnly

    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class InstancePerTestCaseWithDisposeTestCase : IDisposable
    {
        public static int DisposeCount;

        public void Dispose()
        {
            Interlocked.Increment(ref DisposeCount);
        }

        [Test]
        [Order(1)]
        public void Test() => Assert.Pass();

        [Test]
        [Order(2)]
        public void VerifyDisposed() => Assert.That(DisposeCount, Is.EqualTo(1));
    }

    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class InstancePerTestCaseWithAsyncDisposeTestCase : IAsyncDisposable
    {
        public static int DisposeCount;

        public ValueTask DisposeAsync()
        {
            Interlocked.Increment(ref DisposeCount);
            return new ValueTask(Task.CompletedTask);
        }

        [Test]
        [Order(1)]
        public void Test() => Assert.Pass();

        [Test]
        [Order(2)]
        public void VerifyDisposed() => Assert.That(DisposeCount, Is.EqualTo(1));
    }

    #endregion
}
