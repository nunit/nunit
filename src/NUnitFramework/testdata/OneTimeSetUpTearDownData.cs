// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.TestData.OneTimeSetUpTearDownData
{
    [TestFixture]
    public class SetUpAndTearDownFixture
    {
        public int SetUpCount = 0;
        public int TearDownCount = 0;
        public bool ThrowInBaseSetUp = false;

        [OneTimeSetUp]
        public virtual void Init()
        {
            SetUpCount++;
            if (ThrowInBaseSetUp)
                throw new Exception("Error in base OneTimeSetUp");
        }

        [OneTimeTearDown]
        public virtual void Destroy()
        {
            TearDownCount++;
        }

        [Test]
        public void Success() { }

        [Test]
        public void EvenMoreSuccess() { }
    }

    [TestFixture]
    public class SetUpAndTearDownFixtureWithTestCases
    {
        public int SetUpCount = 0;
        public int TearDownCount = 0;

        [OneTimeSetUp]
        public virtual void Init()
        {
            SetUpCount++;
        }

        [OneTimeTearDown]
        public virtual void Destroy()
        {
            TearDownCount++;
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Success(int i)
        {
            Assert.Pass("Passed with test case {0}", i);
        }
    }

    [TestFixture]
    public class SetUpAndTearDownFixtureWithTheories
    {
        public int SetUpCount = 0;
        public int TearDownCount = 0;

        [OneTimeSetUp]
        public virtual void Init()
        {
            SetUpCount++;
        }

        [OneTimeTearDown]
        public virtual void Destroy()
        {
            TearDownCount++;
        }

        public struct Data
        {
            public int Id { get; set; }
        }

        [DatapointSource]
        public IEnumerable<Data> FetchAllRows()
        {
            yield return new Data { Id = 1 };
            yield return new Data { Id = 2 };
            yield return new Data { Id = 3 };
            yield return new Data { Id = 4 };
        }

        [Theory]
        public void TheoryTest(Data entry)
        {
            Assert.Pass("Passed with theory id {0}", entry.Id);
        }
    }

    [TestFixture, Explicit]
    public class ExplicitSetUpAndTearDownFixture
    {
        public int SetUpCount = 0;
        public int TearDownCount = 0;

        [OneTimeSetUp]
        public virtual void Init()
        {
            SetUpCount++;
        }

        [OneTimeTearDown]
        public virtual void Destroy()
        {
            TearDownCount++;
        }

        [Test]
        public void Success() { }

        [Test]
        public void EvenMoreSuccess() { }
    }

    [TestFixture]
    public class InheritSetUpAndTearDown : SetUpAndTearDownFixture
    {
        [Test]
        public void AnotherTest() { }

        [Test]
        public void YetAnotherTest() { }
    }

    [TestFixture]
    public class OverrideSetUpAndTearDown : SetUpAndTearDownFixture
    {
        public int DerivedSetUpCount;
        public int DerivedTearDownCount;

        [OneTimeSetUp]
        public override void Init()
        {
            DerivedSetUpCount++;
        }

        [OneTimeTearDown]
        public override void Destroy()
        {
            DerivedTearDownCount++;
        }

        [Test]
        public void AnotherTest() { }

        [Test]
        public void YetAnotherTest() { }
    }

    [TestFixture]
    public class DerivedSetUpAndTearDownFixture : SetUpAndTearDownFixture
    {
        public int DerivedSetUpCount;
        public int DerivedTearDownCount;

        public bool BaseSetUpCalledFirst;
        public bool BaseTearDownCalledLast;

        [OneTimeSetUp]
        public void Init2()
        {
            DerivedSetUpCount++;
            BaseSetUpCalledFirst = this.SetUpCount > 0;
        }

        [OneTimeTearDown]
        public void Destroy2()
        {
            DerivedTearDownCount++;
            BaseTearDownCalledLast = this.TearDownCount == 0;
        }

        [Test]
        public void AnotherTest() { }

        [Test]
        public void YetAnotherTest() { }
    }

    [TestFixture]
    public class StaticSetUpAndTearDownFixture
    {
        public static int SetUpCount = 0;
        public static int TearDownCount = 0;

        [OneTimeSetUp]
        public static void Init()
        {
            SetUpCount++;
        }

        [OneTimeTearDown]
        public static void Destroy()
        {
            TearDownCount++;
        }

        [Test]
        public static void MyTest() { }
    }

    [TestFixture]
    public class DerivedStaticSetUpAndTearDownFixture : StaticSetUpAndTearDownFixture
    {
        public static int DerivedSetUpCount;
        public static int DerivedTearDownCount;

        public static bool BaseSetUpCalledFirst;
        public static bool BaseTearDownCalledLast;


        [OneTimeSetUp]
        public static void Init2()
        {
            DerivedSetUpCount++;
            BaseSetUpCalledFirst = SetUpCount > 0;
        }

        [OneTimeTearDown]
        public static void Destroy2()
        {
            DerivedTearDownCount++;
            BaseTearDownCalledLast = TearDownCount == 0;
        }

        [Test]
        public static void SomeTest() { }
    }

    [TestFixture]
    public static class StaticClassSetUpAndTearDownFixture
    {
        public static int SetUpCount = 0;
        public static int TearDownCount = 0;

        [OneTimeSetUp]
        public static void Init()
        {
            SetUpCount++;
        }

        [OneTimeTearDown]
        public static void Destroy()
        {
            TearDownCount++;
        }

        [Test]
        public static void MyTest() { }
    }

    [TestFixture]
    public class FixtureWithParallelizableOnOneTimeSetUp
    {
        [OneTimeSetUp]
        [Parallelizable]
        public void BadOneTimeSetup() { }

        [Test]
        public void Test() { }
    }

    [TestFixture]
    public class MisbehavingFixture 
    {
        public bool BlowUpInSetUp = false;
        public bool BlowUpInTest = false;
        public bool BlowUpInTearDown = false;

        public int SetUpCount = 0;
        public int TearDownCount = 0;

        public void Reinitialize()
        {
            SetUpCount = 0;
            TearDownCount = 0;

            BlowUpInSetUp = false;
            BlowUpInTearDown = false;
        }

        [OneTimeSetUp]
        public void SetUp() 
        {
            SetUpCount++;
            if (BlowUpInSetUp)
                throw new Exception("This was thrown from fixture setup");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            TearDownCount++;
            if (BlowUpInTearDown)
                throw new Exception("This was thrown from fixture teardown");
        }

        [Test]
        public void Test() 
        {
            if (BlowUpInTest)
                throw new Exception("This was thrown from a test");
        }
    }

    [TestFixture]
    public class ExceptionInConstructor
    {
        public ExceptionInConstructor()
        {
            throw new Exception( "This was thrown in constructor" );
        }

        [Test]
        public void NothingToTest()
        {
        }
    }

    [TestFixture]
    public class IgnoreInFixtureSetUp
    {
        [OneTimeSetUp]
        public void SetUpCallsIgnore() 
        {
            Assert.Ignore("OneTimeSetUp called Ignore");
        }

        [Test]
        public void NothingToTest() 
        {
        }
    }

    [TestFixture]
    public class SetUpAndTearDownWithTestInName
    {
        public int SetUpCount = 0;
        public int TearDownCount = 0;

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            SetUpCount++;
        }

        [OneTimeTearDown]
        public virtual void OneTimeTearDown()
        {
            TearDownCount++;
        }

        [Test]
        public void Success(){}

        [Test]
        public void EvenMoreSuccess(){}
    }

    [TestFixture, Ignore( "Do Not Run This" )]
    public class IgnoredFixture
    {
        public bool SetupCalled = false;
        public bool TeardownCalled = false;

        [OneTimeSetUp]
        public virtual void ShouldNotRun()
        {
            SetupCalled = true;
        }

        [OneTimeTearDown]
        public virtual void NeitherShouldThis()
        {
            TeardownCalled = true;
        }

        [Test]
        public void Success(){}

        [Test]
        public void EvenMoreSuccess(){}
    }

    [TestFixture]
    public class FixtureWithNoTests
    {
        public bool SetupCalled = false;
        public bool TeardownCalled = false;

        [OneTimeSetUp]
        public virtual void Init()
        {
            SetupCalled = true;
        }

        [OneTimeTearDown]
        public virtual void Destroy()
        {
            TeardownCalled = true;
        }
    }

    [TestFixture]
    public class DisposableFixture : IDisposable
    {
        public int DisposeCalled = 0;
        public List<String> Actions = new List<String>();
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Actions.Add("OneTimeSetUp");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Actions.Add("OneTimeTearDown");
        }

        [Test]
        public void OneTest() { }

        public void Dispose()
        {
            Actions.Add("Dispose");
            DisposeCalled++;
        }
    }

    [TestFixture]
    public class DisposableFixtureWithTestCases : IDisposable
    {
        public int DisposeCalled = 0;

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void TestCaseTest(int data) { }
        
        public void Dispose()
        {
            DisposeCalled++;
        }
    }
}
