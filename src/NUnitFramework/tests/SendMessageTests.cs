// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.TestData;
using NUnit.TestUtilities;

namespace NUnit.Framework
{
    public class SendMessageTests : ITestListener
    {
        private const string SOME_DESTINATION = "destination";
        private const string SOME_MESSAGE = "message";

        [Test]
        public void TestMessage_SendsToListener()
        {
            var test = TestBuilder.MakeTestFromMethod(typeof(SendMessageFixture), nameof(SendMessageFixture.TestWithMessage));
            var work = TestBuilder.CreateWorkItem(test, new SendMessageFixture());
            work.Context.Listener = this;
            var result = TestBuilder.ExecuteWorkItem(work);

            Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(result.Output, Is.EqualTo(""));

            Assert.That(_testMessage, Is.Not.Null);
            Assert.That(_testMessage.Destination, Is.EqualTo(SOME_DESTINATION));
            Assert.That(_testMessage.Message, Is.EqualTo(SOME_MESSAGE));
            Assert.That(_testMessage.TestId, Is.EqualTo(result.Test.Id));
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

        private TestMessage? _testMessage;

        void ITestListener.SendMessage(TestMessage message)
        {
            _testMessage = message;
        }

        #endregion
    }
}
