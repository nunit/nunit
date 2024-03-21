// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestListener provides an implementation of ITestListener that
    /// does nothing. It is used only through its NULL property.
    /// </summary>
    public class TestListener : ITestListener
    {
        /// <summary>
        /// Called when a test has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
        }

        /// <summary>
        /// Called when a test case has finished
        /// </summary>
        /// <param name="result">The result of the test</param>
        public void TestFinished(ITestResult result)
        {
        }

        /// <summary>
        /// Called when a OneTimeSetUp has started
        /// </summary>
        /// <param name="test">Information about the OneTimeSetUp method that has started. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeSetUpStarted(ITest test)
        {
        }

        /// <summary>
        /// Called when a OneTimeSetUp has finished
        /// </summary>
        /// <param name="test">Information about the OneTimeSetUp method that has finished. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeSetUpFinished(ITest test)
        {
        }

        /// <summary>
        /// Called when a OneTimeTearDown has started
        /// </summary>
        /// <param name="test">Information about the OneTimeTearDown method that has started. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeTearDownStarted(ITest test)
        {
        }

        /// <summary>
        /// Called when a OneTimeTearDown has finished
        /// </summary>
        /// <param name="test">Information about the OneTimeTearDown method that has finished. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeTearDownFinished(ITest test)
        {
        }

        /// <summary>
        /// Called when a test produces output for immediate display
        /// </summary>
        /// <param name="output">A TestOutput object containing the text to display</param>
        public void TestOutput(TestOutput output)
        {
        }

        /// <summary>
        /// Called when a test produces a message to be sent to listeners
        /// </summary>
        /// <param name="message">A <see cref="TestMessage"/> object containing the message to send</param>
        public void SendMessage(TestMessage message)
        {
        }

        /// <summary>
        /// Construct a new TestListener - private so it may not be used.
        /// </summary>
        private TestListener()
        {
        }

        /// <summary>
        /// Get a listener that does nothing
        /// </summary>
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        public static ITestListener NULL => new TestListener();
    }
}
