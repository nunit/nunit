// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.AssertIgnoreData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Assertions
{
    /// <summary>
    /// Tests of IgnoreException and Assert.Ignore
    /// </summary>
    [TestFixture]
    public class AssertIgnoreTests
    {
        [Test]
        public void ThrowsIgnoreException()
        {
            Assert.That(
                () => Assert.Ignore(),
                Throws.TypeOf<IgnoreException>());
        }

        [Test]
        public void ThrowsIgnoreExceptionWithMessage()
        {
            Assert.That(
                () => Assert.Ignore("MESSAGE"),
                Throws.TypeOf<IgnoreException>().With.Message.EqualTo("MESSAGE"));
        }

        [Test]
        public void ThrowsIgnoreExceptionWithMessageAndArgs()
        {
            Assert.That(
                () => Assert.Ignore("MESSAGE: {0}+{1}={2}", 2, 2, 4),
                Throws.TypeOf<IgnoreException>().With.Message.EqualTo("MESSAGE: 2+2=4"));
        }

        [Test]
        public void IgnoreWorksForTestCase()
        {
            Type fixtureType = typeof(IgnoredTestCaseFixture);
            ITestResult result = TestBuilder.RunTestCase(fixtureType, "CallsIgnore");
            Assert.AreEqual(ResultState.Ignored, result.ResultState);
            Assert.AreEqual("Ignore me", result.Message);
        }

        [Test]
        public void IgnoreWorksForTestSuite()
        {
            TestSuite suite = new TestSuite("IgnoredTestFixture");
            suite.Add( TestBuilder.MakeFixture( typeof( IgnoredTestSuiteFixture ) ) );
            ITestResult fixtureResult = TestBuilder.RunTest(suite).Children.ToArray ()[0];

            Assert.AreEqual(ResultState.Ignored.WithSite(FailureSite.SetUp), fixtureResult.ResultState);

            foreach (ITestResult testResult in fixtureResult.Children)
                Assert.AreEqual(ResultState.Ignored.WithSite(FailureSite.Parent), testResult.ResultState);
        }

        [Test]
        public void IgnoreWorksFromSetUp()
        {
            ITestResult fixtureResult = TestBuilder.RunTestFixture( typeof( IgnoreInSetUpFixture ) );

            // TODO: Decide whether to pass Ignored state to containing fixture
            //Assert.AreEqual(ResultState.Ignored, fixtureResult.ResultState);

            foreach (var testResult in fixtureResult.Children)
                Assert.AreEqual(ResultState.Ignored, testResult.ResultState);
        }

        [Test]
        public void IgnoreWithUserMessage()
        {
            try
            {
                Assert.Ignore( "my message" );
            }
            catch( IgnoreException ex )
            {
                Assert.AreEqual( "my message", ex.Message );
            }
        }

        [Test]
        public void IgnoreWithUserMessage_OneArg()
        {
            try
            {
                Assert.Ignore( "The number is {0}", 5 );
            }
            catch( IgnoreException ex )
            {
                Assert.AreEqual( "The number is 5", ex.Message );
            }
        }

        [Test]
        public void IgnoreWithUserMessage_ThreeArgs()
        {
            try
            {
                Assert.Ignore( "The numbers are {0}, {1} and {2}", 1, 2, 3 );
            }
            catch( IgnoreException ex )
            {
                Assert.AreEqual( "The numbers are 1, 2 and 3", ex.Message );
            }
        }

        [Test]
        public void IgnoreWithUserMessage_ArrayOfArgs()
        {
            try
            {
            Assert.Ignore( "The numbers are {0}, {1} and {2}", new object[] { 1, 2, 3 } );
            }
            catch( IgnoreException ex )
            {
                Assert.AreEqual( "The numbers are 1, 2 and 3", ex.Message );
            }
        }
    }
}
