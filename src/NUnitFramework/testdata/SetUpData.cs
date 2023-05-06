// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;

namespace NUnit.TestData.SetUpData
{
    [TestFixture]
    public class SetUpAndTearDownFixture
    {
        public bool WasSetUpCalled;
        public bool WasTearDownCalled;
        public bool ThrowInBaseSetUp;

        [SetUp]
        public virtual void Init()
        {
            WasSetUpCalled = true;
            if (ThrowInBaseSetUp)
                throw (new Exception("Exception in base setup"));
        }

        [TearDown]
        public virtual void Destroy()
        {
            WasTearDownCalled = true;
        }

        [Test]
        public void Success() { }
    }


    [TestFixture]
    public class SetUpAndTearDownCounterFixture
    {
        public int SetUpCounter;
        public int TearDownCounter;

        [SetUp]
        public virtual void Init()
        {
            SetUpCounter++;
        }

        [TearDown]
        public virtual void Destroy()
        {
            TearDownCounter++;
        }

        [Test]
        public void TestOne(){}

        [Test]
        public void TestTwo(){}

        [Test]
        public void TestThree(){}
    }

    [TestFixture]
    public class InheritSetUpAndTearDown : SetUpAndTearDownFixture
    {
        [Test]
        public void AnotherTest(){}
    }

    [TestFixture]
    public class DefineInheritSetUpAndTearDown : SetUpAndTearDownFixture
    {
        public bool DerivedSetUpCalled;
        public bool DerivedTearDownCalled;

        [SetUp]
        public override void Init()
        {
            DerivedSetUpCalled = true;
        }

        [TearDown]
        public override void Destroy()
        {
            DerivedTearDownCalled = true;
        }

        [Test]
        public void AnotherTest(){}
    }

    public class MultipleSetUpTearDownFixture
    {
        public bool WasSetUp1Called;
        public bool WasSetUp2Called;
        public bool WasSetUp3Called;
        public bool WasTearDown1Called;
        public bool WasTearDown2Called;

        [SetUp]
        public virtual void Init1()
        {
            WasSetUp1Called = true;
        }
        [SetUp]
        public virtual void Init2()
        {
            WasSetUp2Called = true;
        }
        [SetUp]
        public virtual void Init3()
        {
            WasSetUp3Called = true;
        }

        [TearDown]
        public virtual void TearDown1()
        {
            WasTearDown1Called = true;
        }
        [TearDown]
        public virtual void TearDown2()
        {
            WasTearDown2Called = true;
        }

        [Test]
        public void Success() { }
    }

    [TestFixture]
    public class DerivedClassWithSeparateSetUp : SetUpAndTearDownFixture
    {
        public bool WasDerivedSetUpCalled;
        public bool WasDerivedTearDownCalled;
        public bool WasBaseSetUpCalledFirst;
        public bool WasBaseTearDownCalledLast;

        [SetUp]
        public void DerivedInit()
        {
            WasDerivedSetUpCalled = true;
            WasBaseSetUpCalledFirst = WasSetUpCalled;
        }

        [TearDown]
        public void DerivedTearDown()
        {
            WasDerivedTearDownCalled = true;
            WasBaseTearDownCalledLast = !WasTearDownCalled;
        }
    }

    [TestFixture]
    public class SetupAndTearDownExceptionFixture
    {
        public Exception SetupException;
        public Exception TearDownException;

        [SetUp]
        public void SetUp()
        {
            if (SetupException is not null) throw SetupException;
        }

        [TearDown]
        public void TearDown()
        {
            if (TearDownException is not null) throw TearDownException;
        }

        [Test]
        public void TestOne() {}
    }
}
