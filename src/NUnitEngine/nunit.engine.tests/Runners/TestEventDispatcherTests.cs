// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

using NUnit.Engine.Runners;
using NUnit.Framework;

namespace NUnit.Engine.Tests.Runners
{
    using System.Collections.Generic;

    [TestFixture]
    public class TestEventDispatcherTests
    {
        private TestScheduler _testScheduler;
        private TestEventListener _testEventListener1;
        private TestEventListener _testEventListener2;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            _testEventListener1 = new TestEventListener();
            _testEventListener2 = new TestEventListener();
        }

        [Test]
        public void ShouldNotSendEventsDirectrlyToListeners()
        {
            // Given
            var dispatcher = CreateInstance();
            dispatcher.Listeners.Add(_testEventListener1);
            dispatcher.Listeners.Add(_testEventListener2);

            // When       
            dispatcher.OnTestEvent("abc");

            // Then
            Assert.AreEqual(1, _testScheduler.QueueSize);
            Assert.AreEqual(0, _testEventListener1.Repots.Count);
            Assert.AreEqual(0, _testEventListener2.Repots.Count);
        }

        [Test]
        public void ShouldSendEventsViaScheduler()
        {
            // Given
            var dispatcher = CreateInstance();
            dispatcher.Listeners.Add(_testEventListener1);
            dispatcher.Listeners.Add(_testEventListener2);

            // When       
            dispatcher.OnTestEvent("abc");
            _testScheduler.Run();

            // Then
            Assert.AreEqual(0, _testScheduler.QueueSize);
            Assert.AreEqual(1, _testEventListener1.Repots.Count);
            Assert.AreEqual("abc", _testEventListener1.Repots[0]);
            Assert.AreEqual(1, _testEventListener2.Repots.Count);
            Assert.AreEqual("abc", _testEventListener2.Repots[0]);
        }

        private TestEventDispatcher CreateInstance()
        {
            return new TestEventDispatcher(_testScheduler);
        }

        private class TestEventListener : ITestEventListener
        {
            public IList<string> Repots { get; set; }

            public TestEventListener()
            {
                Repots = new List<string>();
            }

            public void OnTestEvent(string report)
            {
                Repots.Add(report);
            }
        }
    }
}
