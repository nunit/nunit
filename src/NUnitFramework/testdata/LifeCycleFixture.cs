// ***********************************************************************
// Copyright (c) 2020 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************


using System;
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
        public void DummyTest1() { }
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
            Assert.AreEqual(_initialValue, _value++);
        }

        [Test]
        public void Test2()
        {
            Assert.AreEqual(_initialValue, _value++);
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
            Assert.AreEqual(0, _counter++);
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
