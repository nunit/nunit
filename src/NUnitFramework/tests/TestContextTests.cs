// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.TestData.TestContextData;
using NUnit.TestUtilities;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    [TestFixture]
    public class TestContextTests
    {
        private TestContext _setupContext;

        private readonly string _name = TestContext.CurrentContext.Test.Name;

        private readonly string _testDirectory = TestContext.CurrentContext.TestDirectory;
        private readonly string _workDirectory = TestContext.CurrentContext.WorkDirectory;

        private string _tempFilePath;

        private const string TempFileName = "TestContextTests.tmp";

        [OneTimeSetUp]
        public void CreateTempFile()
        {
            _tempFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, TempFileName);
            File.Create(_tempFilePath).Dispose();
        }

        [OneTimeTearDown]
        public void RemoveTempFile()
        {
            File.Delete(_tempFilePath);
        }

        [SetUp]
        public void SaveSetUpContext()
        {
            _setupContext = TestContext.CurrentContext;
        }

        #region TestDirectory

        [Test]
        public void ConstructorCanAccessTestDirectory()
        {
            Assert.That(_testDirectory, Is.Not.Null);
        }

        [TestCaseSource(nameof(TestDirectorySource))]
        public void TestCaseSourceCanAccessTestDirectory(string testDirectory)
        {
            Assert.That(testDirectory, Is.EqualTo(_testDirectory));
        }

        static IEnumerable<string> TestDirectorySource()
        {
            yield return TestContext.CurrentContext.TestDirectory;
        }

        #endregion

        #region WorkDirectory

        [Test]
        public void ConstructorAccessWorkDirectory()
        {
            Assert.That(_workDirectory, Is.Not.Null);
        }

        [Test]
        public void TestCanAccessWorkDirectory()
        {
            string workDirectory = TestContext.CurrentContext.WorkDirectory;
            Assert.NotNull(workDirectory);
            Assert.That(Directory.Exists(workDirectory), $"Directory {workDirectory} does not exist");
        }

        [TestCaseSource(nameof(WorkDirectorySource))]
        public void TestCaseSourceCanAccessWorkDirectory(string workDirectory)
        {
            Assert.That(workDirectory, Is.EqualTo(_workDirectory));
        }

        static IEnumerable<string> WorkDirectorySource()
        {
            yield return TestContext.CurrentContext.WorkDirectory;
        }

    #endregion

        #region Test

        #region Name

        [Test]
            public void ConstructorCanAccessFixtureName()
            {
                Assert.That(_name, Is.EqualTo("TestContextTests"));
            }

            [Test]
            public void TestCanAccessItsOwnName()
            {
                Assert.That(TestContext.CurrentContext.Test.Name, Is.EqualTo("TestCanAccessItsOwnName"));
            }

            [Test]
            public void SetUpCanAccessTestName()
            {
                Assert.That(_setupContext.Test.Name, Is.EqualTo(TestContext.CurrentContext.Test.Name));
            }

            [TestCase(5)]
            public void TestCaseCanAccessItsOwnName(int x)
            {
                Assert.That(TestContext.CurrentContext.Test.Name, Is.EqualTo("TestCaseCanAccessItsOwnName(5)"));
            }

            #endregion

        #region FullName

        [Test]
        public void SetUpCanAccessTestFullName()
        {
            Assert.That(_setupContext.Test.FullName, Is.EqualTo(TestContext.CurrentContext.Test.FullName));
        }

        [Test]
        public void TestCanAccessItsOwnFullName()
        {
            Assert.That(TestContext.CurrentContext.Test.FullName,
                Is.EqualTo("NUnit.Framework.TestContextTests.TestCanAccessItsOwnFullName"));
        }

        [TestCase(42)]
        public void TestCaseCanAccessItsOwnFullName(int x)
        {
            Assert.That(TestContext.CurrentContext.Test.FullName,
                Is.EqualTo("NUnit.Framework.TestContextTests.TestCaseCanAccessItsOwnFullName(42)"));
        }

        #endregion

        #region MethodName

        [Test]
        public void SetUpCanAccessTestMethodName()
        {
            Assert.That(_setupContext.Test.MethodName, Is.EqualTo(TestContext.CurrentContext.Test.MethodName));
        }

        [Test]
        public void TestCanAccessItsOwnMethodName()
        {
            Assert.That(TestContext.CurrentContext.Test.MethodName, Is.EqualTo("TestCanAccessItsOwnMethodName"));
        }

        [TestCase(5)]
        public void TestCaseCanAccessItsOwnMethodName(int x)
        {
            Assert.That(TestContext.CurrentContext.Test.MethodName, Is.EqualTo("TestCaseCanAccessItsOwnMethodName"));
        }

        #endregion

        #region Id

        [Test]
        public void TestCanAccessItsOwnId()
        {
            Assert.That(TestContext.CurrentContext.Test.ID, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void SetUpCanAccessTestId()
        {
            Assert.That(_setupContext.Test.ID, Is.EqualTo(TestContext.CurrentContext.Test.ID));
        }

        #endregion

        #region Properties

        [Test]
        [Property("Answer", "42")]
        public void TestCanAccessItsOwnProperties()
        {
            Assert.That(TestContext.CurrentContext.Test.Properties.Get("Answer"), Is.EqualTo("42"));
        }

        #endregion

        #region Arguments

        [TestCase(24, "abc")]
        public void TestCanAccessItsOwnArguments(int x, string s)
        {
            Assert.That(TestContext.CurrentContext.Test.Arguments, Is.EqualTo(new object[] { 24, "abc" }));
        }

        [Test]
        public void TestCanAccessEmptyArgumentsArrayWhenDoesNotHaveArguments()
        {
            Assert.That(TestContext.CurrentContext.Test.Arguments, Is.EqualTo(Array.Empty<object>()));
        }

        #endregion

        #endregion

        #region Result

        [Test]
        public void TestCanAccessAssertCount()
        {
            var context = TestExecutionContext.CurrentContext;

            // These are counted as asserts
            Assert.That(context.AssertCount, Is.EqualTo(0));
            Assert.AreEqual(4, 2 + 2);
            Warn.Unless(2 + 2, Is.EqualTo(4));

            // This one is counted below
            Assert.That(context.AssertCount, Is.EqualTo(3));

            // Assumptions are not counted are not counted
            Assume.That(2 + 2, Is.EqualTo(4));

            Assert.That(TestContext.CurrentContext.AssertCount, Is.EqualTo(4));
        }

        [TestCase("ThreeAsserts_TwoFailed", AssertionStatus.Failed, AssertionStatus.Failed)]
        [TestCase("WarningPlusFailedAssert", AssertionStatus.Warning, AssertionStatus.Failed)]
        public void TestCanAccessAssertionResults(string testName, params AssertionStatus[] expectedStatus)
        {
            AssertionResultFixture fixture = new AssertionResultFixture();
            TestBuilder.RunTestCase(fixture, testName);
            var assertions = fixture.Assertions;

            Assert.That(assertions.Select((o) => o.Status),
                Is.EqualTo(expectedStatus));
            Assert.That(assertions.Select((o) => o.Message),
                Has.All.Contains("Expected: 5"));
            Assert.That(assertions.Select((o) => o.StackTrace),
                Has.All.Contains(testName));
        }

        [Test]
        public void TestCanAccessTestState_PassingTest()
        {
            TestStateRecordingFixture fixture = new TestStateRecordingFixture();
            TestBuilder.RunTestFixture(fixture);
            Assert.That(fixture.StateList, Is.EqualTo("Inconclusive=>Inconclusive=>Passed"));
        }

        [Test]
        public void TestCanAccessTestState_FailureInSetUp()
        {
            TestStateRecordingFixture fixture = new TestStateRecordingFixture();
            fixture.SetUpFailure = true;
            TestBuilder.RunTestFixture(fixture);
            Assert.That(fixture.StateList, Is.EqualTo("Inconclusive=>=>Failed"));
        }

        [Test]
        public void TestCanAccessTestState_FailingTest()
        {
            TestStateRecordingFixture fixture = new TestStateRecordingFixture();
            fixture.TestFailure = true;
            TestBuilder.RunTestFixture(fixture);
            Assert.That(fixture.StateList, Is.EqualTo("Inconclusive=>Inconclusive=>Failed"));
        }

        [Test]
        public void TestCanAccessTestState_IgnoredInSetUp()
        {
            TestStateRecordingFixture fixture = new TestStateRecordingFixture();
            fixture.SetUpIgnore = true;
            TestBuilder.RunTestFixture(fixture);
            Assert.That(fixture.StateList, Is.EqualTo("Inconclusive=>=>Skipped:Ignored"));
        }

        [Test]
        public void TestContextStoresFailureInfoForTearDown()
        {
            var fixture = new TestTestContextInTearDown();
            TestBuilder.RunTestFixture(fixture);
            Assert.That(fixture.FailCount, Is.EqualTo(1));
            Assert.That(fixture.Message, Is.EqualTo("Deliberate failure"));

            PlatformInconsistency.MonoMethodInfoInvokeLosesStackTrace.SkipOnAffectedPlatform(() =>
            {
                Assert.That(fixture.StackTrace, Does.Contain("NUnit.TestData.TestContextData.TestTestContextInTearDown.FailingTest"));
            });
        }

        [Test]
        public void TestContextStoresFailureInfoForOneTimeTearDown()
        {
            var fixture = new TestTestContextInOneTimeTearDown();
            TestBuilder.RunTestFixture(fixture);
            Assert.That(fixture.PassCount, Is.EqualTo(2));
            Assert.That(fixture.FailCount, Is.EqualTo(1));
            Assert.That(fixture.WarningCount, Is.EqualTo(0));
            Assert.That(fixture.SkipCount, Is.EqualTo(3));
            Assert.That(fixture.InconclusiveCount, Is.EqualTo(4));
            Assert.That(fixture.Message, Is.EqualTo(TestResult.CHILD_ERRORS_MESSAGE));
            Assert.That(fixture.StackTrace, Is.Null);
        }

        #endregion

        #region Out

        [Test]
        public async Task TestContextOut_ShouldFlowWithAsyncExecution()
        {
            var expected = TestContext.Out;
            await YieldAsync();
            Assert.AreEqual(expected, TestContext.Out);
        }

        [Test]
        public async Task TestContextWriteLine_ShouldNotThrow_WhenExecutedFromAsyncMethod()
        {
            Assert.DoesNotThrow(TestContext.WriteLine);
            await YieldAsync();
            Assert.DoesNotThrow(TestContext.WriteLine);
        }

        [Test]
        public void TestContextOut_ShouldBeAvailableFromOtherThreads()
        {
            var isTestContextOutAvailable = false;
            Task.Factory.StartNew(() =>
            {
                isTestContextOutAvailable = TestContext.Out != null;
            }).Wait();
            Assert.True(isTestContextOutAvailable);
        }

        private async Task YieldAsync()
        {
            await Task.Yield();
        }

        #endregion

        #region Test Attachments

        [Test]
        public void FilePathOnlyDoesNotThrow()
        {
            Assert.That(() => TestContext.AddTestAttachment(_tempFilePath), Throws.Nothing);
        }

        [Test]
        public void FilePathAndDescriptionDoesNotThrow()
        {
            Assert.That(() => TestContext.AddTestAttachment(_tempFilePath, "Description"), Throws.Nothing);
        }

        [TestCase(null)]
        [TestCase("bad|path.png", IncludePlatform = "Win")]
        public void InvalidFilePathsThrowsArgumentException(string filePath)
        {
            Assert.That(() => TestContext.AddTestAttachment(filePath), Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void NoneExistentFileThrowsFileNotFoundException()
        {
            Assert.That(() => TestContext.AddTestAttachment("NotAFile.txt"), Throws.InstanceOf<FileNotFoundException>());
        }

        #endregion

        #region Retry
        [Test]
        public void TestCanAccessCurrentRepeatCount()
        {
            var context = TestExecutionContext.CurrentContext;

            Assert.That(context.CurrentRepeatCount, Is.EqualTo(0), "expected TestContext.CurrentRepeatCount to be accessible and be zero on first execution of test");
        }
        #endregion
    }

    [TestFixture]
    public class TestContextTearDownTests
    {
        private const int THE_MEANING_OF_LIFE = 42;

        [Test]
        public void TestTheMeaningOfLife()
        {
            Assert.That(THE_MEANING_OF_LIFE, Is.EqualTo(42));
        }

        [TearDown]
        public void TearDown()
        {
            TestContext context = TestContext.CurrentContext;
            Assert.That(context, Is.Not.Null);
            Assert.That(context.Test, Is.Not.Null);
            Assert.That(context.Test.Name, Is.EqualTo("TestTheMeaningOfLife"));
            Assert.That(context.Result, Is.Not.Null);
            Assert.That(context.Result.Outcome, Is.EqualTo(ResultState.Success));
            Assert.That(context.Result.PassCount, Is.EqualTo(1));
            Assert.That(context.Result.FailCount, Is.EqualTo(0));
            Assert.That(context.TestDirectory, Is.Not.Null);
            Assert.That(context.WorkDirectory, Is.Not.Null);
        }
    }

    [TestFixture]
    public class TestContextOneTimeTearDownTests
    {
        [Test]
        public void TestTruth()
        {
            Assert.That(true, Is.True);
        }

        [Test]
        public void TestFalsehood()
        {
            Assert.That(false, Is.False);
        }

        [Test, Explicit]
        public void TestExplicit()
        {
            Assert.Pass("Always passes if you run it!");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestContext context = TestContext.CurrentContext;
            Assert.That(context, Is.Not.Null);
            Assert.That(context.Test, Is.Not.Null);
            Assert.That(context.Test.Name, Is.EqualTo("TestContextOneTimeTearDownTests"));
            Assert.That(context.Result, Is.Not.Null);
            Assert.That(context.Result.Outcome, Is.EqualTo(ResultState.Success));
            Assert.That(context.Result.PassCount, Is.EqualTo(2));
            Assert.That(context.Result.FailCount, Is.EqualTo(0));
            Assert.That(context.Result.SkipCount, Is.EqualTo(1));
        }
    }
}
