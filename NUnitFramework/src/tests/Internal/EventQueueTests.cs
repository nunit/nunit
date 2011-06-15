// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************
using System;
using System.Collections;
using System.Threading;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// Summary description for EventQueueTests.
	/// </summary>
    [TestFixture]
    public class EventQueueTests
    {
        static readonly Event[] events = {
				new TestStartedEvent( null ),
				new TestStartedEvent( null ),
				new TestFinishedEvent( null ),
				new TestStartedEvent( null ),
				new TestFinishedEvent( null ),
				new TestFinishedEvent( null ),
			};

        private static void EnqueueEvents(EventQueue q)
        {
            foreach (Event e in events)
                q.Enqueue(e);
        }

        private static void SendEvents(ITestListener listener)
        {
            foreach (Event e in events)
                e.Send(listener);
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
