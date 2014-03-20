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

#if !NUNITLITE
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NUnit.Framework.Interfaces;

using ThreadState = System.Threading.ThreadState;

namespace NUnit.Framework.Internal.Execution
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
                Event e = q.Dequeue(false);
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
            [Timeout(1000)]
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
                {
                    Thread.Sleep(30);
                }

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

        //[TestFixture]
        //public class SetWaitHandle_Enqueue_SynchronousTest : ProducerConsumerTest
        //{
        //    private EventQueue q;
        //    private AutoResetEvent waitHandle;
        //    private volatile bool afterEnqueue;

        //    [Test]
        //    [Timeout(1000)]
        //    public void SetWaitHandle_Enqueue_Synchronous()
        //    {
        //        using (this.waitHandle = new AutoResetEvent(false))
        //        {
        //            this.q = new EventQueue();
        //            this.q.SetWaitHandleForSynchronizedEvents(this.waitHandle);
        //            this.afterEnqueue = false;
        //            this.RunProducerConsumer();
        //        }
        //    }

        //    protected override void Producer()
        //    {
        //        Event synchronousEvent = new RunStartedEvent(string.Empty, 0);
        //        Assert.IsTrue(synchronousEvent.IsSynchronous);
        //        this.q.Enqueue(synchronousEvent);
        //        this.afterEnqueue = true;
        //        Thread.MemoryBarrier();
        //    }

        //    protected override void Consumer()
        //    {
        //        this.q.Dequeue(true);
        //        Thread.Sleep(30);
        //        Assert.IsFalse(this.afterEnqueue);
        //        this.waitHandle.Set();
        //        Thread.Sleep(30);
        //        Assert.IsTrue(this.afterEnqueue);
        //    }
        //}

        [TestFixture]
        public class SetWaitHandle_Enqueue_AsynchronousTest : ProducerConsumerTest
        {
            private EventQueue q;
            private volatile bool afterEnqueue;

            [Test]
            [Timeout(1000)]
            public void SetWaitHandle_Enqueue_Asynchronous()
            {
                using (AutoResetEvent waitHandle = new AutoResetEvent(false))
                {
                    this.q = new EventQueue();
                    this.q.SetWaitHandleForSynchronizedEvents(waitHandle);
                    this.afterEnqueue = false;
                    this.RunProducerConsumer();
                }
            }

            protected override void Producer()
            {
                Event asynchronousEvent = new OutputEvent(new TestOutput(string.Empty, TestOutputType.Trace));
                Assert.IsFalse(asynchronousEvent.IsSynchronous);
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
            EventQueue q = new EventQueue();
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
        [Timeout(3000)]
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
        [Timeout(1000)]
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
                        if (e.IsSynchronous)
                        {
                            Assert.That(q.Count, Is.EqualTo(0));
                        }
                        else
                        {
                            sumOfAsynchronousQueueLength += q.Count;
                            numberOfAsynchronousEvents++;
                        }
                    }
                }

                Console.WriteLine("Average queue length: {0}", (float)sumOfAsynchronousQueueLength / numberOfAsynchronousEvents);
            }
        }

        /// <summary>
        /// Verifies that when
        /// (1) Traces are captured and fed into the EventListeners, and
        /// (2) an EventListener writes Traces,
        /// the Trace / EventPump / EventListener do not deadlock.
        /// </summary>
        /// <remarks>
        /// This mainly simulates the object structure created by RemoteTestRunner.Run.
        /// </remarks>
        [Test]
        [Timeout(1000)]
        public void TracingEventListenerDoesNotDeadlock()
        {
            QueuingEventListener upstreamListener = new QueuingEventListener();
            EventQueue upstreamListenerQueue = upstreamListener.Events;

            // Install a TraceListener sending TestOutput events to the upstreamListener.
            // This simulates RemoteTestRunner.StartTextCapture, where TestContext installs such a TraceListener.
            TextWriter traceWriter = new EventListenerTextWriter(upstreamListener, TestOutputType.Trace);
            const string TraceListenerName = "TracingEventListenerDoesNotDeadlock";
            TraceListener feedingTraceToUpstreamListener = new TextWriterTraceListener(traceWriter, TraceListenerName);

            try
            {
                Trace.Listeners.Add(feedingTraceToUpstreamListener);

                // downstreamListenerToTrace simulates an EventListener installed e.g. by an Addin, 
                // which may call Trace within the EventListener methods:
                TracingEventListener downstreamListenerToTrace = new TracingEventListener();
                using (EventPump pump = new EventPump(downstreamListenerToTrace, upstreamListenerQueue))
                {
                    pump.Name = "TracingEventListenerDoesNotDeadlock";
                    pump.Start();

                    const int Repetitions = 10;
                    for (int i = 0; i < Repetitions; i++)
                    {
                        foreach (Event e in events)
                        {
                            Trace.WriteLine("Before sending {0} event.", e.GetType().Name);
                            e.Send(upstreamListener);
                            Trace.WriteLine("After sending {0} event.", e.GetType().Name);
                        }
                    }
                }
            }
            finally
            {
                Trace.Listeners.Remove(TraceListenerName);
                feedingTraceToUpstreamListener.Dispose();
            }
        }

        /// <summary> 
        /// Floods the queue of an EventPump with multiple concurrent event producers.
        /// Prints the maximum queue length to Console, but does not implement an
        /// oracle on what the maximum queue length should be.
        /// </summary>
        /// <param name="numberOfProducers">The number of concurrent producer threads.</param>
        /// <param name="producerDelay">
        /// If <c>true</c>, the producer threads slow down by adding a short delay time.
        /// </param>
        [TestCase(1, false)]
        [TestCase(5, true)]
        [TestCase(5, false)]
        [Explicit("Takes several seconds. Just prints the queue length of the EventPump to Console, but has no oracle regarding this.")]
        public void EventPumpQueueLength(int numberOfProducers, bool producerDelay)
        {
            EventQueue q = new EventQueue();
            EventProducer[] producers = new EventProducer[numberOfProducers];
            for (int i = 0; i < numberOfProducers; i++)
            {
                producers[i] = new EventProducer(q, i, producerDelay);
            }

            using (EventPump pump = new EventPump(TestListener.NULL, q))
            {
                pump.Name = "EventPumpQueueLength";
                pump.Start();

                foreach (EventProducer p in producers)
                {
                    p.ProducerThread.Start();
                }
                foreach (EventProducer p in producers)
                {
                    p.ProducerThread.Join();
                }
                pump.Stop();
            }
            Assert.That(q.Count, Is.EqualTo(0));

            foreach (EventProducer p in producers)
            {
                Console.WriteLine(
                    "#Events: {0}, MaxQueueLength: {1}", p.SentEventsCount, p.MaxQueueLength);
                Assert.IsNull(p.Exception, "{0}", p.Exception);
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
                    ThreadUtility.Kill(consumerThread);
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
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                }
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
                    Event e = new OutputEvent(new TestOutput(this.ProducerThread.Name, TestOutputType.Log));
                    DateTime start = DateTime.Now;
                    while (DateTime.Now - start <= TimeSpan.FromSeconds(3))
                    {
                        this.queue.Enqueue(e);
                        this.SentEventsCount++;
                        this.MaxQueueLength = Math.Max(this.queue.Count, this.MaxQueueLength);

                        // without Sleep or with just a Sleep(0), the EventPump thread does not keep up and the queue gets very long
                        if (this.delay)
                        {
                            Thread.Sleep(1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Exception = ex;
                }
            }
        }

        private class TracingEventListener : ITestListener
        {
            #region EventListener Members
            //public void RunStarted(string name, int testCount)
            //{
            //    WriteTrace("RunStarted({0},{1})", name, testCount);
            //}

            //public void RunFinished(TestResult result)
            //{
            //    WriteTrace("RunFinished({0})", result);
            //}

            //public void RunFinished(Exception exception)
            //{
            //    WriteTrace("RunFinished({0})", exception);
            //}

            public void TestStarted(ITest test)
            {
                //WriteTrace("TestStarted({0})", test.Name);
                WriteTrace("TestStarted");
            }

            public void TestFinished(ITestResult result)
            {
                WriteTrace("TestFinished({0})", result);
            }

            //public void SuiteStarted(TestName testName)
            //{
            //    WriteTrace("SuiteStarted({0})", testName);
            //}

            //public void SuiteFinished(TestResult result)
            //{
            //    WriteTrace("SuiteFinished({0})", result);
            //}

            public void UnhandledException(Exception exception)
            {
                WriteTrace("UnhandledException({0})", exception);
            }

            public void TestOutput(TestOutput testOutput)
            {
                if (testOutput.Type != TestOutputType.Trace)
                {
                    WriteTrace("TestOutput {0}: '{1}'", testOutput.Type, testOutput.Text);
                }
            }
            #endregion

            private static void WriteTrace(string message, params object[] args)
            {
                Trace.TraceInformation(message, args);
            }
        }
    }
}
#endif
