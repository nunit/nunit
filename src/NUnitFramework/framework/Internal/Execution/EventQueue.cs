// ***********************************************************************
// Copyright (c) 2007-2016 Charlie Poole, Rob Prouse
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

#if PARALLEL
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
#if NET_2_0 || NET_3_5
using ManualResetEventSlim = System.Threading.ManualResetEvent;
#endif
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

    #endregion

    /// <summary>
    /// Implements a queue of work items each of which
    /// is queued as a WaitCallback.
    /// </summary>
    public class EventQueue
    {
//        static readonly Logger log = InternalTrace.GetLogger("EventQueue");

        private readonly ConcurrentQueue<Event> _queue = new ConcurrentQueue<Event>();

        /* This event is used solely for the purpose of having an optimized sleep cycle when
         * we have to wait on an external event (Add or Remove for instance)
         */
        private readonly AutoResetEvent _mreAdd = new AutoResetEvent(false);

        private int _stopped;

        /// <summary>
        /// Gets the count of items in the queue.
        /// </summary>
        public int Count
        {
            get
            {
                return _queue.Count;
            }
        }

        /// <summary>
        /// Enqueues the specified event
        /// </summary>
        /// <param name="e">The event to enqueue.</param>
        public void Enqueue(Event e)
        {
            // Add to the collection
            _queue.Enqueue(e);

            // Wake up threads that may have been sleeping
            _mreAdd.Set();
        }

        /// <summary>
        /// Removes the first element from the queue and returns it (or <c>null</c>).
        /// </summary>
        /// <param name="blockWhenEmpty">
        /// If <c>true</c> and the queue is empty, the calling thread is blocked until
        /// either an element is enqueued, or <see cref="Stop"/> is called.
        /// </param>
        /// <returns>
        /// <list type="bullet">
        ///   <item>
        ///     <term>If the queue not empty</term>
        ///     <description>the first element.</description>
        ///   </item>
        ///   <item>
        ///     <term>otherwise, if <paramref name="blockWhenEmpty"/>==<c>false</c>
        ///       or <see cref="Stop"/> has been called</term>
        ///     <description><c>null</c>.</description>
        ///   </item>
        /// </list>
        /// </returns>
        public Event Dequeue(bool blockWhenEmpty)
        {
            do
            {
                // Dequeue our work item
                Event e;
                if (_queue.TryDequeue (out e))
                {
                    return e;
                }

                if (!blockWhenEmpty || Interlocked.CompareExchange(ref _stopped, 0, 0) != 0)
                    return null;

                _mreAdd.WaitOne();
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

#endif
