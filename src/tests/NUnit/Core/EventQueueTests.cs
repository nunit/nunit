// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Collections;
using System.Threading;
using NUnit.Framework;

namespace NUnit.Core.Tests
{
	/// <summary>
	/// Summary description for EventQueueTests.
	/// </summary>
    [TestFixture]
    public class EventQueueTests
    {
        static readonly Event[] events = {
				new RunStartedEvent( new TestName(), 0 ),
				new SuiteStartedEvent( null ),
				new TestStartedEvent( null ),
				new TestFinishedEvent( null ),
				new SuiteFinishedEvent( null ),
				new RunFinishedEvent( (TestResult)null )
			};

        private static void EnqueueEvents(EventQueue q)
        {
            foreach (Event e in events)
                q.Enqueue(e);
        }

        private static void SendEvents(ITestListener el)
        {
            foreach (Event e in events)
                e.Send(el);
        }

        private static void VerifyQueue(EventQueue q)
        {
            for (int index = 0; index < events.Length; index++)
            {
                Event e = q.Dequeue();
                Assert.AreEqual(events[index].GetType(), e.GetType(),
                    string.Format("Event {0}", index));
            }
        }

        private static void StartPump(EventPump pump, int waitTime)
        {
            pump.Start();
            WaitForPumpToStart(pump, waitTime);
        }

        private static void StopPump(EventPump pump, int waitTime)
        {
            pump.Stop();
            WaitForPumpToStop(pump, waitTime);
        }

        private static void WaitForPumpToStart(EventPump pump, int waitTime)
        {
            while (waitTime > 0 && pump.PumpState != EventPumpState.Pumping)
            {
                Thread.Sleep(100);
                waitTime -= 100;
            }
        }

        private static void WaitForPumpToStop(EventPump pump, int waitTime)
        {
            while (waitTime > 0 && pump.PumpState != EventPumpState.Stopped)
            {
                Thread.Sleep(100);
                waitTime -= 100;
            }
        }

        [Test]
        public void QueueEvents()
        {
            EventQueue q = new EventQueue();
            EnqueueEvents(q);
            VerifyQueue(q);
        }

        [Test]
        public void SendEvents()
        {
            QueuingEventListener el = new QueuingEventListener();
            SendEvents(el);
            VerifyQueue(el.Events);
        }

        [Test]
        public void StartAndStopPumpOnEmptyQueue()
        {
            EventPump pump = new EventPump(TestListener.NULL, new EventQueue(), false);
            StartPump(pump, 1000);
            Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Pumping));
            StopPump(pump, 1000);
            Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Stopped));
        }

        [Test]
        public void PumpAutoStopsOnRunFinished()
        {
            EventQueue q = new EventQueue();
            EventPump pump = new EventPump(TestListener.NULL, q, true);
            Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Stopped));
            StartPump(pump, 1000);
            Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Pumping));
            q.Enqueue(new RunFinishedEvent(new Exception()));
            WaitForPumpToStop(pump, 1000);
            Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Stopped));
        }

        [Test]
        public void PumpEvents()
        {
            EventQueue q = new EventQueue();
            EnqueueEvents(q);
            QueuingEventListener el = new QueuingEventListener();
            EventPump pump = new EventPump(el, q, false);
            Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Stopped));
            StartPump(pump, 1000);
            Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Pumping));
            StopPump(pump, 1000);
            Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Stopped));
            VerifyQueue(el.Events);
        }

        [Test]
        public void PumpEventsWithAutoStop()
        {
            EventQueue q = new EventQueue();
            EnqueueEvents(q);
            Assert.AreEqual(6, q.Count);
            QueuingEventListener el = new QueuingEventListener();
            EventPump pump = new EventPump(el, q, true);
            pump.Start();
            int tries = 10;
            while (--tries > 0 && q.Count > 0)
            {
                Thread.Sleep(100);
            }
            Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Stopped));
        }
    }
}
