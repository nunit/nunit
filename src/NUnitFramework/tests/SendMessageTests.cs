// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.TestData;

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

            Assert.That(_testMessage.Destination, Is.EqualTo(SOME_DESTINATION));
            Assert.That(_testMessage.Message, Is.EqualTo(SOME_MESSAGE));
            Assert.That(_testMessage.TestId, Is.EqualTo(result.Test.Id));
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

        TestMessage _testMessage;

        public void SendMessage(TestMessage message)
        {
            _testMessage = message;
        }

        #endregion
    }
}
