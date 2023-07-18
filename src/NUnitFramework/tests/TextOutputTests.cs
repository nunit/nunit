// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests
{
    public class TextOutputTests : ITestListener
    {
        private const string SOME_TEXT = "Should go to the result";
        private const string ERROR_TEXT = "Written directly to console";
        private static readonly string NL = Environment.NewLine;

        private static string CapturedOutput => TestExecutionContext.CurrentContext.CurrentResult.Output;

        [Test]
        public void ConsoleWrite_WritesToResult()
        {
            Console.Write(SOME_TEXT);
            Assert.That(TextOutputTests.CapturedOutput, Is.EqualTo(SOME_TEXT));
        }

        [Test]
        public void ConsoleWriteLine_WritesToResult()
        {
            Console.WriteLine(SOME_TEXT);
            Assert.That(TextOutputTests.CapturedOutput, Is.EqualTo(SOME_TEXT + NL));
        }

        [Test]
        public void ConsoleErrorWrite_WritesToListener()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TextOutputFixture), nameof(TextOutputFixture.ConsoleErrorWrite));
            var work = TestBuilder.CreateWorkItem(test, new TextOutputFixture());
            work.Context.Listener = this;
            var result = TestBuilder.ExecuteWorkItem(work);

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(result.Output, Is.EqualTo(string.Empty));
            });

            Assert.That(_testOutput, Is.Not.Null, "No output received");
            Assert.Multiple(() =>
            {
                Assert.That(_testOutput.Text, Is.EqualTo(ERROR_TEXT));
                Assert.That(_testOutput.Stream, Is.EqualTo("Error"));
            });
        }

        [Test]
        public void ConsoleErrorWriteLine_WritesToListener()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TextOutputFixture), nameof(TextOutputFixture.ConsoleErrorWriteLine));
            var work = TestBuilder.CreateWorkItem(test, new TextOutputFixture());
            work.Context.Listener = this;
            var result = TestBuilder.ExecuteWorkItem(work);

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(result.Output, Is.EqualTo(string.Empty));
            });

            Assert.That(_testOutput, Is.Not.Null, "No output received");
            Assert.Multiple(() =>
            {
                Assert.That(_testOutput.Text, Is.EqualTo(ERROR_TEXT + Environment.NewLine));
                Assert.That(_testOutput.Stream, Is.EqualTo("Error"));
            });
        }

        [Test]
        public void MultipleWrites()
        {
            // Test purely for display purposes
            TestContext.Progress.WriteLine("TestContext.Progress displays immediately");
            TestContext.Error.WriteLine("TestContext.Error displays immediately as well");
            Console.Error.WriteLine("Console.Error also displays immediately");
            Console.WriteLine("This line is added to the result and displayed when test ends");
            Console.WriteLine("As is this line");
        }

        [Test]
        public void TestContextError_WritesToListener()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TextOutputFixture), nameof(TextOutputFixture.TestContextErrorWriteLine));
            var work = TestBuilder.CreateWorkItem(test, new TextOutputFixture());
            work.Context.Listener = this;
            var result = TestBuilder.ExecuteWorkItem(work);

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(result.Output, Is.EqualTo(string.Empty));
            });

            Assert.That(_testOutput, Is.Not.Null, "No output received");
            Assert.Multiple(() =>
            {
                Assert.That(_testOutput.Text, Is.EqualTo(ERROR_TEXT + Environment.NewLine));
                Assert.That(_testOutput.Stream, Is.EqualTo("Error"));
            });
        }

        [Test]
        public void TestContextProgress_WritesToListener()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TextOutputFixture), nameof(TextOutputFixture.TestContextProgressWriteLine));
            var work = TestBuilder.CreateWorkItem(test, new TextOutputFixture());
            work.Context.Listener = this;
            var result = TestBuilder.ExecuteWorkItem(work);

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(result.Output, Is.EqualTo(string.Empty));
            });

            Assert.That(_testOutput, Is.Not.Null, "No output received");
            Assert.Multiple(() =>
            {
                Assert.That(_testOutput.Text, Is.EqualTo(ERROR_TEXT + Environment.NewLine));
                Assert.That(_testOutput.Stream, Is.EqualTo("Progress"));
            });
        }

        [Test]
        public void TestContextOut_WritesToResult()
        {
            TestContext.Out.WriteLine(SOME_TEXT);
            Assert.That(TextOutputTests.CapturedOutput, Is.EqualTo(SOME_TEXT + NL));
        }

        [Test]
        public void TestContextWrite_WritesToResult()
        {
            TestContext.Write(SOME_TEXT);
            Assert.That(TextOutputTests.CapturedOutput, Is.EqualTo(SOME_TEXT));
        }

        [Test]
        public void TestContextWriteLine_WritesToResult()
        {
            TestContext.WriteLine(SOME_TEXT);
            Assert.That(Framework.Internal.TestExecutionContext.CurrentContext.CurrentResult.Output, Is.EqualTo(SOME_TEXT + NL));
        }

        #region ITestListener Implementation

        void ITestListener.TestStarted(ITest test)
        {
        }

        void ITestListener.TestFinished(ITestResult result)
        {
        }

        private TestOutput? _testOutput;

        void ITestListener.TestOutput(TestOutput output)
        {
            _testOutput = output;
        }

        void ITestListener.SendMessage(TestMessage message)
        {
        }

        #endregion
    }
}
