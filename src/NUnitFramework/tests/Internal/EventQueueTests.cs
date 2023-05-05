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
            // These are all in violation of contract
            // However the code here doesn't use the argument.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            new TestStartedEvent(null),
            new TestOutputEvent(null),
            new TestStartedEvent(null),
            new TestFinishedEvent(null),
            new TestStartedEvent(null),
            new TestOutputEvent(null),
            new TestMessageEvent(null),
            new TestFinishedEvent(null),
            new TestFinishedEvent(null),
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
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
                Event? e = q.Dequeue(false);
                Assert.That(e, Is.Not.Null);
                Assert.That(e.GetType(), Is.EqualTo(events[index].GetType()), $"Event {index}");
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
            Assert.That(q.Dequeue(false), Is.Null);
        }

        [TestFixture]
        public class DequeueBlocking_StopTest : ProducerConsumerTest
        {
            private volatile int receivedEvents;

            [Test]
#if THREAD_ABORT
            [Timeout(1000)]
#endif
            public void DequeueBlocking_Stop()
            {
                var q = new EventQueue();
                this.receivedEvents = 0;
                this.RunProducerConsumer(q);
                Assert.That(this.receivedEvents, Is.EqualTo(events.Length + 1));
            }

            protected override void Producer(object? parameter)
            {
                if (parameter is not EventQueue q)
                    throw new ArgumentException("Expected an EventQueue", nameof(parameter));
                EnqueueEvents(q);
                while (this.receivedEvents < events.Length)
                    Thread.Sleep(30);

                q.Stop();
            }

            protected override void Consumer(object? parameter)
            {
                if (parameter is not EventQueue q)
                    throw new ArgumentException("Expected an EventQueue", nameof(parameter));
                Event? e;
                do
                {
                    e = q.Dequeue(true);
                    this.receivedEvents++;
                    Thread.MemoryBarrier();
                }
                while (e != null);
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
            private volatile Exception? myConsumerException;

            protected void RunProducerConsumer(object? parameter)
            {
                this.myConsumerException = null;
                Thread consumerThread = new Thread(this.ConsumerThreadWrapper);
                try
                {
                    consumerThread.Start(parameter);
                    this.Producer(parameter);
                    bool consumerStopped = consumerThread.Join(1000);
                    Assert.That(consumerStopped, Is.True);
                }
                finally
                {
#if THREAD_ABORT
                    ThreadUtility.Kill(consumerThread);
#endif
                }

                Assert.That(this.myConsumerException, Is.Null);
            }

            protected abstract void Producer(object? parameter);

            protected abstract void Consumer(object? parameter);

            private void ConsumerThreadWrapper(object? parameter)
            {
                try
                {
                    this.Consumer(parameter);
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
            public Exception? Exception;
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
