// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class EventListenerSendMessageTests
    {
        TestListenerIntercepter ListenerResult;

        [SetUp]
        public void SetUp()
        {
            // Wrap the current listener, listening to events, and forwarding the original event
            ListenerResult = new TestListenerIntercepter(TestExecutionContext.CurrentContext.Listener);
            TestExecutionContext.CurrentContext.Listener = ListenerResult;
        }

        [TearDown]
        public void TearDown()
        {
            // Restore the original listener
            TestExecutionContext.CurrentContext.Listener = ListenerResult.DefaultListener;
        }

        [Test]
        public void TestSendsMessage()
        {
            TestExecutionContext.CurrentContext.SendMessage("destination", "message");

            Assert.That(ListenerResult.Messages.Count, Is.EqualTo(1));
            Assert.That(ListenerResult.Messages[0].Destination, Is.EqualTo("destination"));
            Assert.That(ListenerResult.Messages[0].Message, Is.EqualTo("message"));
            Assert.That(ListenerResult.Messages[0].TestId, Is.EqualTo(TestContext.CurrentContext.Test.ID));
        }

#region ITestListener implementation

        private class TestListenerIntercepter : ITestListener
        {
            public IList<TestMessage> Messages { get; }
            public ITestListener DefaultListener { get; }

            public TestListenerIntercepter(ITestListener defaultListener)
            {
                DefaultListener = defaultListener;
                Messages = new List<TestMessage>();
            }

            void ITestListener.TestStarted(ITest test)
            {
                if (DefaultListener != null)
                    DefaultListener.TestStarted(test);
            }

            void ITestListener.TestFinished(ITestResult result)
            {
                if (DefaultListener != null)
                    DefaultListener.TestFinished(result);
            }

            void ITestListener.TestOutput(TestOutput output)
            {

            }

            void ITestListener.SendMessage(TestMessage message)
            {
                Assert.NotNull(message);

                Messages.Add(message);

                if (DefaultListener != null)
                    DefaultListener.SendMessage(message);
            }
        }

#endregion
    }
}
