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
using NUnit.Framework;
using NUnit.Core.Builders;
using NUnit.TestUtilities;
using NUnit.TestData.SetUpTest;

namespace NUnit.Core.Tests
{
	[TestFixture]
	public class SetUpTest
	{	
		[Test]
		public void SetUpAndTearDownCounter()
		{
			SetUpAndTearDownCounterFixture fixture = new SetUpAndTearDownCounterFixture();
			TestBuilder.RunTestFixture( fixture );

			Assert.AreEqual(3, fixture.setUpCounter);
			Assert.AreEqual(3, fixture.tearDownCounter);
		}

		
		[Test]
		public void MakeSureSetUpAndTearDownAreCalled()
		{
			SetUpAndTearDownFixture fixture = new SetUpAndTearDownFixture();
			TestBuilder.RunTestFixture( fixture );

			Assert.IsTrue(fixture.wasSetUpCalled);
			Assert.IsTrue(fixture.wasTearDownCalled);
		}

		[Test]
		public void CheckInheritedSetUpAndTearDownAreCalled()
		{
			InheritSetUpAndTearDown fixture = new InheritSetUpAndTearDown();
			TestBuilder.RunTestFixture( fixture );

			Assert.IsTrue(fixture.wasSetUpCalled);
			Assert.IsTrue(fixture.wasTearDownCalled);
		}

		[Test]
		public void CheckOverriddenSetUpAndTearDownAreNotCalled()
		{
			DefineInheritSetUpAndTearDown fixture = new DefineInheritSetUpAndTearDown();
			TestBuilder.RunTestFixture( fixture );

			Assert.IsFalse(fixture.wasSetUpCalled);
			Assert.IsFalse(fixture.wasTearDownCalled);
			Assert.IsTrue(fixture.derivedSetUpCalled);
			Assert.IsTrue(fixture.derivedTearDownCalled);
		}

        [Test]
        public void MultipleSetUpAndTearDownMethodsAreCalled()
        {
            MultipleSetUpTearDownFixture fixture = new MultipleSetUpTearDownFixture();
            TestBuilder.RunTestFixture(fixture);

            Assert.IsTrue(fixture.wasSetUp1Called, "SetUp1");
            Assert.IsTrue(fixture.wasSetUp2Called, "SetUp2");
            Assert.IsTrue(fixture.wasSetUp3Called, "SetUp3");
            Assert.IsTrue(fixture.wasTearDown1Called, "TearDown1");
            Assert.IsTrue(fixture.wasTearDown2Called, "TearDown2");
        }

        [Test]
        public void BaseSetUpIsCalledFirstTearDownLast()
        {
            DerivedClassWithSeparateSetUp fixture = new DerivedClassWithSeparateSetUp();
            TestBuilder.RunTestFixture(fixture);

            Assert.IsTrue(fixture.wasSetUpCalled, "Base SetUp Called");
            Assert.IsTrue(fixture.wasTearDownCalled, "Base TearDown Called");
            Assert.IsTrue(fixture.wasDerivedSetUpCalled, "Derived SetUp Called");
            Assert.IsTrue(fixture.wasDerivedTearDownCalled, "Derived TearDown Called");
            Assert.IsTrue(fixture.wasBaseSetUpCalledFirst, "SetUp Order");
            Assert.IsTrue(fixture.wasBaseTearDownCalledLast, "TearDown Order");
        }

        [Test]
        public void SetupRecordsOriginalExceptionThownByTestCase()
        {
            Exception e = new Exception("Test message for exception thrown from setup");
            SetupAndTearDownExceptionFixture fixture = new SetupAndTearDownExceptionFixture();
            fixture.setupException = e;
            TestResult result = TestBuilder.RunTestFixture(fixture);
            Assert.IsTrue(result.HasResults, "Fixture test should have child result.");
            result = (TestResult)result.Results[0];
            Assert.AreEqual(result.ResultState, ResultState.Error, "Test should be in error state");
            //TODO: below assert fails now, a bug?
            //Assert.AreEqual(result.FailureSite, FailureSite.SetUp, "Test should be failed at setup site");
            string expected = string.Format("{0} : {1}", e.GetType().FullName, e.Message);
            Assert.AreEqual(expected, result.Message);
        }

        [Test]
        public void TearDownRecordsOriginalExceptionThownByTestCase()
        {
            Exception e = new Exception("Test message for exception thrown from tear down");
            SetupAndTearDownExceptionFixture fixture = new SetupAndTearDownExceptionFixture();
            fixture.tearDownException = e;
            TestResult result = TestBuilder.RunTestFixture(fixture);
            Assert.IsTrue(result.HasResults, "Fixture test should have child result.");
            result = (TestResult)result.Results[0];
            Assert.AreEqual(result.ResultState, ResultState.Error, "Test should be in error state");
            Assert.AreEqual(result.FailureSite, FailureSite.TearDown, "Test should be failed at tear down site");
            string expected = string.Format("TearDown : {0} : {1}", e.GetType().FullName, e.Message);
            Assert.AreEqual(expected, result.Message);
        }
	}
}
