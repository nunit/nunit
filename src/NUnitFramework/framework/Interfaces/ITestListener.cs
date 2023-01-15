// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ITestListener interface is used internally to receive
    /// notifications of significant events while a test is being
    /// run. The events are propagated to clients by means of an
    /// AsyncCallback. NUnit extensions may also monitor these events.
    /// </summary>
    public interface ITestListener
    {
        /// <summary>
        /// Called when a test has just started
        /// </summary>
        /// <param name="test">The test that is starting</param>
        void TestStarted(ITest test);

        /// <summary>
        /// Called when a test has finished
        /// </summary>
        /// <param name="result">The result of the test</param>
        void TestFinished(ITestResult result);

        /// <summary>
        /// Called when a test produces output for immediate display
        /// </summary>
        /// <param name="output">A TestOutput object containing the text to display</param>
        void TestOutput(TestOutput output);

        /// <summary>
        /// Called when a test produces a message to be sent to listeners
        /// </summary>
        /// <param name="message">A <see cref="TestMessage"/> object containing the text to send</param>
        void SendMessage(TestMessage message);
    }
}
