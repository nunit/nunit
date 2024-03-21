// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// QueuingEventListener uses an EventQueue to store any
    /// events received on its EventListener interface.
    /// </summary>
    public class QueuingEventListener : ITestListener
    {
        /// <summary>
        /// The EventQueue created and filled by this listener
        /// </summary>
        public EventQueue Events { get; }

        /// <summary>
        /// Construct a QueuingEventListener
        /// </summary>
        public QueuingEventListener()
        {
            Events = new EventQueue();
        }

        #region EventListener Methods
        /// <summary>
        /// A test has started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        public void TestStarted(ITest test)
        {
            Events.Enqueue(new TestStartedEvent(test));
        }

        /// <summary>
        /// A test case finished
        /// </summary>
        /// <param name="result">Result of the test case</param>
        public void TestFinished(ITestResult result)
        {
            Events.Enqueue(new TestFinishedEvent(result));
        }

        /// <summary>
        /// Called when a OneTimeSetUp has started
        /// </summary>
        /// <param name="test">Information about the OneTimeSetUp method that has started. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeSetUpStarted(ITest test)
        {
            Events.Enqueue(new OneTimeSetUpStartedEvent(test));
        }

        /// <summary>
        /// Called when a OneTimeSetUp has finished
        /// </summary>
        /// <param name="test">Information about the OneTimeSetUp method that has finished. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeSetUpFinished(ITest test)
        {
            Events.Enqueue(new OneTimeSetUpFinishedEvent(test));
        }

        /// <summary>
        /// Called when a OneTimeTearDown has started
        /// </summary>
        /// <param name="test">Information about the OneTimeTearDown method that has started. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeTearDownStarted(ITest test)
        {
            Events.Enqueue(new OneTimeTearDownStartedEvent(test));
        }

        /// <summary>
        /// Called when a OneTimeTearDown has finished
        /// </summary>
        /// <param name="test">Information about the OneTimeTearDown method that has finished. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeTearDownFinished(ITest test)
        {
            Events.Enqueue(new OneTimeTearDownFinishedEvent(test));
        }

        /// <summary>
        /// Called when a test produces output for immediate display
        /// </summary>
        /// <param name="output">A TestOutput object containing the text to display</param>
        public void TestOutput(TestOutput output)
        {
            Events.Enqueue(new TestOutputEvent(output));
        }

        /// <summary>
        /// Called when a test produces a message to be sent to listeners
        /// </summary>
        /// <param name="message">A <see cref="TestMessage"/> object containing the text to send</param>
        public void SendMessage(TestMessage message)
        {
            Events.Enqueue(new TestMessageEvent(message));
        }

        #endregion
    }
}
