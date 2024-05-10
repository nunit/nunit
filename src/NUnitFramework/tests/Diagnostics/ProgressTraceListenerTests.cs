// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework.Diagnostics;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Diagnostics
{
    public abstract class ProgressTraceListenerTestsBase// : ITestListener
    {
        protected const string SOME_TEXT = "Should go to the output";
        protected static readonly string NL = Environment.NewLine;

        protected TestListenerInterceptor TestResultListener { get; private set; }

        [SetUp]
        public void AddTestResultListener()
        {
            // Wrap the current listener, listening to events, and forwarding the original event
            TestResultListener = new TestListenerInterceptor(TestExecutionContext.CurrentContext.Listener);
            TestExecutionContext.CurrentContext.Listener = TestResultListener;
        }

        [TearDown]
        public void RemoveTestResultListener()
        {
            // Restore the original listener
            TestExecutionContext.CurrentContext.Listener = TestResultListener.DefaultListener;
        }

        [Test]
        public void TestProgressIsOutput()
        {
            Assert.That(TestResultListener.Outputs, Has.Count.EqualTo(0));

            TestContext.Progress.WriteLine(SOME_TEXT);
            Assert.That(TestResultListener.Outputs, Has.Count.EqualTo(1));
            Assert.That(TestResultListener.Outputs[0], Is.EqualTo(SOME_TEXT + NL));
        }

        #region ITestListener implementation

        protected class TestListenerInterceptor : ITestListener
        {
            public IList<string> Outputs { get; }
            public ITestListener DefaultListener { get; }

            public TestListenerInterceptor(ITestListener defaultListener)
            {
                DefaultListener = defaultListener;
                Outputs = new List<string>();
            }

            void ITestListener.TestStarted(ITest test)
            {
                DefaultListener?.TestStarted(test);
            }

            void ITestListener.TestFinished(ITestResult result)
            {
                DefaultListener?.TestFinished(result);
            }

            void ITestListener.TestOutput(TestOutput output)
            {
                Assert.That(output, Is.Not.Null);
                Outputs.Add(output.Text);

                DefaultListener?.TestOutput(output);
            }

            void ITestListener.SendMessage(TestMessage message)
            {
            }
        }

        #endregion
    }

    [TestFixture]
    public class ProgressTraceListenerTests : ProgressTraceListenerTestsBase
    {
        private ProgressTraceListener _progressTraceListener;

        [OneTimeSetUp]
        public void AddProgressTraceListener()
        {
            _progressTraceListener = new ProgressTraceListener();
            Trace.Listeners.Add(_progressTraceListener);
        }

        [OneTimeTearDown]
        public void RemoveProgressTraceListener()
        {
            Trace.Listeners.Remove(_progressTraceListener);
            _progressTraceListener.Dispose();
        }

        [Test]
        public void TestDebugIsDirectedToOutput()
        {
            Assert.That(TestResultListener.Outputs, Has.Count.EqualTo(0));

            Debug.WriteLine(SOME_TEXT);
            Assert.That(TestResultListener.Outputs, Has.Count.EqualTo(1));
            Assert.That(TestResultListener.Outputs[0], Is.EqualTo(SOME_TEXT + NL));
        }

        [Test]
        public void TestTraceIsDirectedToOutput()
        {
            Assert.That(TestResultListener.Outputs, Has.Count.EqualTo(0));

            Trace.WriteLine(SOME_TEXT);
            Assert.That(TestResultListener.Outputs, Has.Count.EqualTo(1));
            Assert.That(TestResultListener.Outputs[0], Is.EqualTo(SOME_TEXT + NL));
        }
    }

    [TestFixture]
    public class NoProgressTraceListenerTests : ProgressTraceListenerTestsBase
    {
        [Test]
        public void TestDebugIsNotDirectedToOutput()
        {
            Assert.That(TestResultListener.Outputs, Has.Count.EqualTo(0));

            Debug.WriteLine(SOME_TEXT);
            Assert.That(TestResultListener.Outputs, Has.Count.EqualTo(0));
        }

        [Test]
        public void TestTraceIsNotDirectedToOutput()
        {
            Assert.That(TestResultListener.Outputs, Has.Count.EqualTo(0));

            Trace.WriteLine(SOME_TEXT);
            // For some yet not understood reason, behavior of this test differs from what is
            // being seeing in https://github.com/nunit/nunit3-vs-adapter/issues/718 as well as
            // https://github.com/TestCentric/testcentric-gui/issues/1043 and also documented at
            // https://docs.nunit.org/articles/vs-test-adapter/Trace-and-Debug.html:
            // "Trace and Debug output is by default not sent to the console output, or anywhere else."
            // Thus, no output is expected here:
        ////Assert.That(TestResultListener.Outputs, Has.Count.EqualTo(0));
            // But actually there is:
            Assert.That(TestResultListener.Outputs, Has.Count.EqualTo(1));
            Assert.That(TestResultListener.Outputs[0], Is.EqualTo(SOME_TEXT + NL));
        }
    }
}
