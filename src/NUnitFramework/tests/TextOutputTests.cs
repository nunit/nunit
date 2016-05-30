// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework.Tests
{
    public class TextOutputTests : ITestListener
    {
        private const string SOME_TEXT = "Should go to the result";
        private const string ERROR_TEXT = "Written directly to console";
        private static readonly string NL = NUnit.Env.NewLine;

        private string CapturedOutput
        {
            get { return TestExecutionContext.CurrentContext.CurrentResult.Output; }
        }

#if !SILVERLIGHT && !PORTABLE && !NETCF
        [Test]
        public void ConsoleWrite_WritesToResult()
        {
            Console.Write(SOME_TEXT);
            Assert.That(CapturedOutput, Is.EqualTo(SOME_TEXT));
        }

        [Test]
        public void ConsoleWriteLine_WritesToResult()
        {
            Console.WriteLine(SOME_TEXT);
            Assert.That(CapturedOutput, Is.EqualTo(SOME_TEXT + NL));
        }

        [Test]
        public void ConsoleErrorWrite_WritesToListener()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TextOutputFixture), "ConsoleErrorWrite");
            var work = TestBuilder.PrepareWorkItem(test, new TextOutputFixture());
            work.Context.Listener = this;
            var result = TestBuilder.ExecuteWorkItem(work);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(result.Output, Is.EqualTo(""));

            Assert.NotNull(_testOutput, "No output received");
            Assert.That(_testOutput.Text, Is.EqualTo(ERROR_TEXT));
            Assert.That(_testOutput.Stream, Is.EqualTo("Error"));
        }

        [Test]
        public void ConsoleErrorWriteLine_WritesToListener()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TextOutputFixture), "ConsoleErrorWriteLine");
            var work = TestBuilder.PrepareWorkItem(test, new TextOutputFixture());
            work.Context.Listener = this;
            var result = TestBuilder.ExecuteWorkItem(work);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(result.Output, Is.EqualTo(""));

            Assert.NotNull(_testOutput, "No output received");
            Assert.That(_testOutput.Text, Is.EqualTo(ERROR_TEXT + Environment.NewLine));
            Assert.That(_testOutput.Stream, Is.EqualTo("Error"));
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
            var test = TestBuilder.MakeTestFromMethod(typeof(TextOutputFixture), "TestContextErrorWriteLine");
            var work = TestBuilder.PrepareWorkItem(test, new TextOutputFixture());
            work.Context.Listener = this;
            var result = TestBuilder.ExecuteWorkItem(work);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(result.Output, Is.EqualTo(""));

            Assert.NotNull(_testOutput, "No output received");
            Assert.That(_testOutput.Text, Is.EqualTo(ERROR_TEXT + Environment.NewLine));
            Assert.That(_testOutput.Stream, Is.EqualTo("Error"));
        }

        [Test]
        public void TestContextProgress_WritesToListener()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(TextOutputFixture), "TestContextProgressWriteLine");
            var work = TestBuilder.PrepareWorkItem(test, new TextOutputFixture());
            work.Context.Listener = this;
            var result = TestBuilder.ExecuteWorkItem(work);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(result.Output, Is.EqualTo(""));

            Assert.NotNull(_testOutput, "No output received");
            Assert.That(_testOutput.Text, Is.EqualTo(ERROR_TEXT + Environment.NewLine));
            Assert.That(_testOutput.Stream, Is.EqualTo("Progress"));
        }
#endif

        [Test]
        public void TestContextOut_WritesToResult()
        {
            TestContext.Out.WriteLine(SOME_TEXT);
            Assert.That(CapturedOutput, Is.EqualTo(SOME_TEXT + NL));
        }

        [Test]
        public void TestContextWrite_WritesToResult()
        {
            TestContext.Write(SOME_TEXT);
            Assert.That(CapturedOutput, Is.EqualTo(SOME_TEXT));
        }

        [Test]
        public void TestContextWriteLine_WritesToResult()
        {
            TestContext.WriteLine(SOME_TEXT);
            Assert.That(Internal.TestExecutionContext.CurrentContext.CurrentResult.Output, Is.EqualTo(SOME_TEXT + NL));
        }

        #region ITestListener Implementation

        public void TestStarted(ITest test)
        {
        }

        public void TestFinished(ITestResult result)
        {
        }

        TestOutput _testOutput;

        public void TestOutput(TestOutput output)
        {
            _testOutput = output;
        }

        #endregion
    }
}
