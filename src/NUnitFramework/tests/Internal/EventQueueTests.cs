// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// Summary description for EventQueueTests.
    /// </summary>
    [TestFixture]
    public class EventQueueTests
    {
        private static readonly Event[] events =
        {
            new TestStartedEvent(null),
            new TestOutputEvent(null),
            new TestStartedEvent(null),
            new TestFinishedEvent(null),
            new TestStartedEvent(null),
            new TestOutputEvent(null),
            new TestMessageEvent(null),
            new TestFinishedEvent(null),
            new TestFinishedEvent(null),
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
                Event e = q.Dequeue(false);
                Assert.AreEqual(events[index].GetType(), e.GetType(), $"Event {index}");
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

        #region EventQueue Tests

        [Test]
        public void QueueEvents()
        {
            EventQueue q = new EventQueue();
            EnqueueEvents(q);
            VerifyQueue(q);
        }

        [Test]
        public void DequeueEmpty()
        {
            EventQueue q = new EventQueue();
            Assert.IsNull(q.Dequeue(false));
        }

        [TestFixture]
        public class DequeueBlocking_StopTest : ProducerConsumerTest
        {
            private EventQueue q;
            private volatile int receivedEvents;

            [Test]
#if THREAD_ABORT
            [Timeout(1000)]
#endif
            public void DequeueBlocking_Stop()
            {
                this.q = new EventQueue();
                this.receivedEvents = 0;
                this.RunProducerConsumer();
                Assert.AreEqual(events.Length + 1, this.receivedEvents);
            }

            protected override void Producer()
            {
                EnqueueEvents(this.q);
                while (this.receivedEvents < events.Length)
                    Thread.Sleep(30);

                this.q.Stop();
            }

            protected override void Consumer()
            {
                Event e;
                do
                {
                    e = this.q.Dequeue(true);
                    this.receivedEvents++;
                    Thread.MemoryBarrier();
                }
                while (e != null);
            }
        }

        [TestFixture]
        public class SetWaitHandle_Enqueue_AsynchronousTest : ProducerConsumerTest
        {
            private EventQueue q;
            private volatile bool afterEnqueue;

            [Test]
#if THREAD_ABORT
            [Timeout(1000)]
#endif
            public void SetWaitHandle_Enqueue_Asynchronous()
            {
                using (new AutoResetEvent(false))
                {
                    this.q = new EventQueue();
                    this.afterEnqueue = false;
                    this.RunProducerConsumer();
                }
            }

            protected override void Producer()
            {
                Event asynchronousEvent = new TestStartedEvent(new TestSuite("Dummy"));
                this.q.Enqueue(asynchronousEvent);
                this.afterEnqueue = true;
                Thread.MemoryBarrier();
            }

            protected override void Consumer()
            {
                this.q.Dequeue(true);
                Thread.Sleep(30);
                Assert.IsTrue(this.afterEnqueue);
            }
        }

        #endregion

        #region QueuingEventListener Tests

        [Test]
        public void SendEvents()
        {
            QueuingEventListener el = new QueuingEventListener();
            SendEvents(el);
            VerifyQueue(el.Events);
        }

        #endregion

        #region EventPump Tests

        [Test]
        public void StartAndStopPumpOnEmptyQueue()
        {
            using (EventPump pump = new EventPump(TestListener.NULL, new EventQueue()))
            {
                pump.Name = "StartAndStopPumpOnEmptyQueue";
                StartPump(pump, 1000);
                Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Pumping));
                StopPump(pump, 1000);
                Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Stopped));
            }
        }

        [Test]
#if THREAD_ABORT
        [Timeout(3000)]
#endif
        public void PumpEvents()
        {
            EventQueue q = new EventQueue();
            QueuingEventListener el = new QueuingEventListener();
            using (EventPump pump = new EventPump(el, q))
            {
                pump.Name = "PumpEvents";
                Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Stopped));
                StartPump(pump, 1000);
                Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Pumping));
                EnqueueEvents(q);
                StopPump(pump, 1000);
                Assert.That(pump.PumpState, Is.EqualTo(EventPumpState.Stopped));
            }
            VerifyQueue(el.Events);
        }

        [Test]
#if THREAD_ABORT
        [Timeout(3000)]
#endif
        public void PumpSynchronousAndAsynchronousEvents()
        {
            EventQueue q = new EventQueue();
            using (EventPump pump = new EventPump(TestListener.NULL, q))
            {
                pump.Name = "PumpSynchronousAndAsynchronousEvents";
                pump.Start();

                int numberOfAsynchronousEvents = 0;
                int sumOfAsynchronousQueueLength = 0;
                const int Repetitions = 2;
                for (int i = 0; i < Repetitions; i++)
                {
                    foreach (Event e in events)
                    {
                        q.Enqueue(e);

                        sumOfAsynchronousQueueLength += q.Count;
                        numberOfAsynchronousEvents++;
                    }
                }

                Console.WriteLine("Average queue length: {0}", (float)sumOfAsynchronousQueueLength / numberOfAsynchronousEvents);
            }
        }

#endregion

        public abstract class ProducerConsumerTest
        {
            private volatile Exception myConsumerException;

            protected void RunProducerConsumer()
            {
                this.myConsumerException = null;
                Thread consumerThread = new Thread(new ThreadStart(this.ConsumerThreadWrapper));
                try
                {
                    consumerThread.Start();
                    this.Producer();
                    bool consumerStopped = consumerThread.Join(1000);
                    Assert.IsTrue(consumerStopped);
                }
                finally
                {
#if THREAD_ABORT
                    ThreadUtility.Kill(consumerThread);
#endif
                }

                Assert.IsNull(this.myConsumerException);
            }

            protected abstract void Producer();

            protected abstract void Consumer();

            private void ConsumerThreadWrapper()
            {
                try
                {
                    this.Consumer();
                }
#if THREAD_ABORT
                catch (System.Threading.ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
#endif
                catch (Exception ex)
                {
                    this.myConsumerException = ex;
                }
            }
        }

        private class EventProducer
        {
            public readonly Thread ProducerThread;
            public int SentEventsCount;
            public int MaxQueueLength;
            public Exception Exception;
            private readonly EventQueue queue;
            private readonly bool delay;

            public EventProducer(EventQueue q, int id, bool delay)
            {
                this.queue = q;
                this.ProducerThread = new Thread(new ThreadStart(this.Produce));
                this.ProducerThread.Name = this.GetType().FullName + id;
                this.delay = delay;
            }

            private void Produce()
            {
                try
                {
                    Event e = new TestStartedEvent(new TestSuite("Dummy"));
                    DateTime start = DateTime.Now;
                    while (DateTime.Now - start <= TimeSpan.FromSeconds(3))
                    {
                        this.queue.Enqueue(e);
                        this.SentEventsCount++;
                        this.MaxQueueLength = Math.Max(this.queue.Count, this.MaxQueueLength);

                        // without Sleep or with just a Sleep(0), the EventPump thread does not keep up and the queue gets very long
                        if (this.delay)
                            Thread.Sleep(1);
                    }
                }
                catch (Exception ex)
                {
                    this.Exception = ex;
                }
            }
        }
    }
}
