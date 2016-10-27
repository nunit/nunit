// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Collections.Generic;
using NUnit.Framework;

namespace NUnit.TestData.OneTimeSetUpTearDownData
{
    [TestFixture]
    public class SetUpAndTearDownFixture
    {
        public int setUpCount = 0;
        public int tearDownCount = 0;
        public bool throwInBaseSetUp = false;

        [OneTimeSetUp]
        public virtual void Init()
        {
            setUpCount++;
            if (throwInBaseSetUp)
                throw new Exception("Error in base OneTimeSetUp");
        }

        [OneTimeTearDown]
        public virtual void Destroy()
        {
            tearDownCount++;
        }

        [Test]
        public void Success(){}

        [Test]
        public void EvenMoreSuccess(){}
    }

    [TestFixture]
    public class SetUpAndTearDownFixtureWithTestCases
    {
        public int setUpCount = 0;
        public int tearDownCount = 0;

        [OneTimeSetUp]
        public virtual void Init()
        {
            setUpCount++;
        }

        [OneTimeTearDown]
        public virtual void Destroy()
        {
            tearDownCount++;
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
        public int setUpCount = 0;
        public int tearDownCount = 0;

        [OneTimeSetUp]
        public virtual void Init()
        {
            setUpCount++;
        }

        [OneTimeTearDown]
        public virtual void Destroy()
        {
            tearDownCount++;
        }

        public struct Data
        {
            public int Id { get; set; }
        }

        [DatapointSource]
        public IEnumerable<Data> fetchAllRows()
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

    [TestFixture,Explicit]
    public class ExplicitSetUpAndTearDownFixture
    {
        public int setUpCount = 0;
        public int tearDownCount = 0;

        [OneTimeSetUp]
        public virtual void Init()
        {
            setUpCount++;
        }

        [OneTimeTearDown]
        public virtual void Destroy()
        {
            tearDownCount++;
        }

        [Test]
        public void Success(){}

        [Test]
        public void EvenMoreSuccess(){}
    }

    [TestFixture]
    public class InheritSetUpAndTearDown : SetUpAndTearDownFixture
    {
        [Test]
        public void AnotherTest(){}

        [Test]
        public void YetAnotherTest(){}
    }

    [TestFixture]
    public class OverrideSetUpAndTearDown : SetUpAndTearDownFixture
    {
        public int derivedSetUpCount;
        public int derivedTearDownCount;

        [OneTimeSetUp]
        public override void Init()
        {
            derivedSetUpCount++;
        }

        [OneTimeTearDown]
        public override void Destroy()
        {
            derivedTearDownCount++;
        }

        [Test]
        public void AnotherTest() { }

        [Test]
        public void YetAnotherTest() { }
    }

    [TestFixture]
    public class DerivedSetUpAndTearDownFixture : SetUpAndTearDownFixture
    {
        public int derivedSetUpCount;
        public int derivedTearDownCount;

        public bool baseSetUpCalledFirst;
        public bool baseTearDownCalledLast;

        [OneTimeSetUp]
        public void Init2()
        {
            derivedSetUpCount++;
            baseSetUpCalledFirst = this.setUpCount > 0;
        }

        [OneTimeTearDown]
        public void Destroy2()
        {
            derivedTearDownCount++;
            baseTearDownCalledLast = this.tearDownCount == 0;
        }

        [Test]
        public void AnotherTest() { }

        [Test]
        public void YetAnotherTest() { }
    }

    [TestFixture]
    public class StaticSetUpAndTearDownFixture
    {
        public static int setUpCount = 0;
        public static int tearDownCount = 0;

        [OneTimeSetUp]
        public static void Init()
        {
            setUpCount++;
        }

        [OneTimeTearDown]
        public static void Destroy()
        {
            tearDownCount++;
        }

        [Test]
        public static void MyTest() { }
    }

    [TestFixture]
    public class DerivedStaticSetUpAndTearDownFixture : StaticSetUpAndTearDownFixture
    {
        public static int derivedSetUpCount;
        public static int derivedTearDownCount;

        public static bool baseSetUpCalledFirst;
        public static bool baseTearDownCalledLast;


        [OneTimeSetUp]
        public static void Init2()
        {
            derivedSetUpCount++;
            baseSetUpCalledFirst = setUpCount > 0;
        }

        [OneTimeTearDown]
        public static void Destroy2()
        {
            derivedTearDownCount++;
            baseTearDownCalledLast = tearDownCount == 0;
        }

        [Test]
        public static void SomeTest() { }
    }

    [TestFixture]
    public static class StaticClassSetUpAndTearDownFixture
    {
        public static int setUpCount = 0;
        public static int tearDownCount = 0;

        [OneTimeSetUp]
        public static void Init()
        {
            setUpCount++;
        }

        [OneTimeTearDown]
        public static void Destroy()
        {
            tearDownCount++;
        }

        [Test]
        public static void MyTest() { }
    }

    [TestFixture]
    public class MisbehavingFixture 
    {
        public bool blowUpInSetUp = false;
        public bool blowUpInTest = false;
        public bool blowUpInTearDown = false;

        public int setUpCount = 0;
        public int tearDownCount = 0;

        public void Reinitialize()
        {
            setUpCount = 0;
            tearDownCount = 0;

            blowUpInSetUp = false;
            blowUpInTearDown = false;
        }

        [OneTimeSetUp]
        public void BlowUpInSetUp() 
        {
            setUpCount++;
            if (blowUpInSetUp)
                throw new Exception("This was thrown from fixture setup");
        }

        [OneTimeTearDown]
        public void BlowUpInTearDown()
        {
            tearDownCount++;
            if ( blowUpInTearDown )
                throw new Exception("This was thrown from fixture teardown");
        }

        [Test]
        public void BlowUpInTest() 
        {
            if (blowUpInTest)
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
        public void nothingToTest()
        {
        }
    }

    [TestFixture]
    public class IgnoreInFixtureSetUp
    {
        [OneTimeSetUp]
        public void SetUpCallsIgnore() 
        {
            Assert.Ignore( "TestFixtureSetUp called Ignore" );
        }

        [Test]
        public void nothingToTest() 
        {
        }
    }

    [TestFixture]
    public class SetUpAndTearDownWithTestInName
    {
        public int setUpCount = 0;
        public int tearDownCount = 0;

        [OneTimeSetUp]
        public virtual void TestFixtureSetUp()
        {
            setUpCount++;
        }

        [OneTimeTearDown]
        public virtual void TestFixtureTearDown()
        {
            tearDownCount++;
        }

        [Test]
        public void Success(){}

        [Test]
        public void EvenMoreSuccess(){}
    }

    [TestFixture, Ignore( "Do Not Run This" )]
    public class IgnoredFixture
    {
        public bool setupCalled = false;
        public bool teardownCalled = false;

        [OneTimeSetUp]
        public virtual void ShouldNotRun()
        {
            setupCalled = true;
        }

        [OneTimeTearDown]
        public virtual void NeitherShouldThis()
        {
            teardownCalled = true;
        }

        [Test]
        public void Success(){}

        [Test]
        public void EvenMoreSuccess(){}
    }

    [TestFixture]
    public class FixtureWithNoTests
    {
        public bool setupCalled = false;
        public bool teardownCalled = false;

        [OneTimeSetUp]
        public virtual void Init()
        {
            setupCalled = true;
        }

        [OneTimeTearDown]
        public virtual void Destroy()
        {
            teardownCalled = true;
        }
    }

    [TestFixture]
    public class DisposableFixture : IDisposable
    {
        public int disposeCalled = 0;

        [Test]
        public void OneTest() { }

        public void Dispose()
        {
            disposeCalled++;
        }
    }

    [TestFixture]
    public class DisposableFixtureWithTestCases : IDisposable
    {
        public int disposeCalled = 0;

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void TestCaseTest(int data) { }
        
        public void Dispose()
        {
            disposeCalled++;
        }
    }
}
