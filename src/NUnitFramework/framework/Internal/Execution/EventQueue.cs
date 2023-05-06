// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Concurrent;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{

    #region Individual Event Classes

    /// <summary>
    /// NUnit.Core.Event is the abstract base for all stored events.
    /// An Event is the stored representation of a call to the
    /// ITestListener interface and is used to record such calls
    /// or to queue them for forwarding on another thread or at
    /// a later time.
    /// </summary>
    public abstract class Event
    {
        /// <summary>
        /// The Send method is implemented by derived classes to send the event to the specified listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public abstract void Send(ITestListener listener);
    }

    /// <summary>
    /// TestStartedEvent holds information needed to call the TestStarted method.
    /// </summary>
    public class TestStartedEvent : Event
    {
        private readonly ITest _test;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStartedEvent"/> class.
        /// </summary>
        /// <param name="test">The test.</param>
        public TestStartedEvent(ITest test)
        {
            _test = test;
        }

        /// <summary>
        /// Calls TestStarted on the specified listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public override void Send(ITestListener listener)
        {
            listener.TestStarted(_test);
        }
    }

    /// <summary>
    /// TestFinishedEvent holds information needed to call the TestFinished method.
    /// </summary>
    public class TestFinishedEvent : Event
    {
        private readonly ITestResult _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFinishedEvent"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public TestFinishedEvent(ITestResult result)
        {
            _result = result;
        }

        /// <summary>
        /// Calls TestFinished on the specified listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public override void Send(ITestListener listener)
        {
            listener.TestFinished(_result);
        }
    }

    /// <summary>
    /// TestOutputEvent holds information needed to call the TestOutput method.
    /// </summary>
    public class TestOutputEvent : Event
    {
        private readonly TestOutput _output;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestOutputEvent"/> class.
        /// </summary>
        /// <param name="output">The output object.</param>
        public TestOutputEvent(TestOutput output)
        {
            _output = output;
        }

        /// <summary>
        /// Calls TestOutput on the specified listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public override void Send(ITestListener listener)
        {
            listener.TestOutput(_output);
        }
    }

    /// <summary>
    /// TestMessageEvent holds information needed to call the SendMessage method.
    /// </summary>
    public class TestMessageEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestMessageEvent"/> class.
        /// </summary>
        /// <param name="testMessage">The test message object.</param>
        public TestMessageEvent(TestMessage testMessage)
        {
            TestMessage = testMessage;
        }

        /// <summary>
        /// Calls <see cref="Send(ITestListener)"/> on the specified listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public override void Send(ITestListener listener)
        {
            listener.SendMessage(TestMessage);
        }

        /// <summary>
        /// Holds <see cref="TestMessage"/> object for sending to all listeners
        /// </summary>
        public TestMessage TestMessage { get; }
    }

    #endregion

    /// <summary>
    /// Implements a queue of work items each of which
    /// is queued as a WaitCallback.
    /// </summary>
    public class EventQueue
    {
        private const int SpinCount = 5;

//        static readonly Logger log = InternalTrace.GetLogger("EventQueue");

        private readonly ConcurrentQueue<Event> _queue = new ConcurrentQueue<Event>();

        /* This event is used solely for the purpose of having an optimized sleep cycle when
         * we have to wait on an external event (Add or Remove for instance)
         */
        private readonly ManualResetEventSlim _mreAdd = new ManualResetEventSlim();

        /* The whole idea is to use these two values in a transactional
         * way to track and manage the actual data inside the underlying lock-free collection
         * instead of directly working with it or using external locking.
         *
         * They are manipulated with CAS and are guaranteed to increase over time and use
         * of the instance thus preventing ABA problems.
         */
        private int _addId = int.MinValue;
        private int _removeId = int.MinValue;

        private int _stopped;

        /// <summary>
        /// Gets the count of items in the queue.
        /// </summary>
        public int Count => _queue.Count;

        /// <summary>
        /// Enqueues the specified event
        /// </summary>
        /// <param name="e">The event to enqueue.</param>
        public void Enqueue(Event e)
        {
            do
            {
                int cachedAddId = _addId;

                // Validate that we have are the current enqueuer
                if (Interlocked.CompareExchange(ref _addId, cachedAddId + 1, cachedAddId) != cachedAddId)
                    continue;

                // Add to the collection
                _queue.Enqueue(e);

                // Wake up threads that may have been sleeping
                _mreAdd.Set();

                break;
            } while (true);

            // Setting this to anything other than 0 causes NUnit to be sensitive
            // to the windows timer resolution - see issue #2217
            Thread.Sleep(0);  // give EventPump thread a chance to process the event
        }

        /// <summary>
        /// Removes the first element from the queue and returns it (or <see langword="null"/>).
        /// </summary>
        /// <param name="blockWhenEmpty">
        /// If <see langword="true"/> and the queue is empty, the calling thread is blocked until
        /// either an element is enqueued, or <see cref="Stop"/> is called.
        /// </param>
        /// <returns>
        /// <list type="bullet">
        ///   <item>
        ///     <term>If the queue not empty</term>
        ///     <description>the first element.</description>
        ///   </item>
        ///   <item>
        ///     <term>otherwise, if <paramref name="blockWhenEmpty"/>==<see langword="false"/>
        ///       or <see cref="Stop"/> has been called</term>
        ///     <description><see langword="null"/>.</description>
        ///   </item>
        /// </list>
        /// </returns>
        public Event? Dequeue(bool blockWhenEmpty)
        {
            SpinWait sw = new SpinWait();

            do
            {
                int cachedRemoveId = _removeId;
                int cachedAddId = _addId;

                // Empty case
                if (cachedRemoveId == cachedAddId)
                {
                    if (!blockWhenEmpty || _stopped != 0)
                        return null;

                    // Spin a few times to see if something changes
                    if (sw.Count <= SpinCount)
                    {
                        sw.SpinOnce();
                    }
                    else
                    {
                        // Reset to wait for an enqueue
                        _mreAdd.Reset();

                        // Recheck for an enqueue to avoid a Wait
                        if (cachedRemoveId != _removeId || cachedAddId != _addId)
                        {
                            // Queue is not empty, set the event
                            _mreAdd.Set();
                            continue;
                        }

                        // Wait for something to happen
                        _mreAdd.Wait(500);
                    }

                    continue;
                }

                // Validate that we are the current dequeuer
                if (Interlocked.CompareExchange(ref _removeId, cachedRemoveId + 1, cachedRemoveId) != cachedRemoveId)
                    continue;


                // Dequeue our work item
                Event? e;
                while (!_queue.TryDequeue(out e))
                {
                    if (!blockWhenEmpty || _stopped != 0)
                        return null;
                }

                return e;
            } while (true);
        }

        /// <summary>
        /// Stop processing of the queue
        /// </summary>
        public void Stop()
        {
            if (Interlocked.CompareExchange(ref _stopped, 1, 0) == 0)
                _mreAdd.Set();
        }
    }
}
