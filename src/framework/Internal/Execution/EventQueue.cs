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
using System.Globalization;
using System.Runtime.Serialization;
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
        abstract public void Send( ITestListener listener );
    
        /// <summary>
        /// Gets a value indicating whether this event is delivered synchronously by the NUnit <see cref="EventPump"/>.
        /// <para>
        /// If <c>true</c>, and if <see cref="EventQueue.SetWaitHandleForSynchronizedEvents"/> has been used to 
        /// set a WaitHandle, <see cref="EventQueue.Enqueue"/> blocks its calling thread until the <see cref="EventPump"/>
        /// thread has delivered the event and sets the WaitHandle.
        /// </para>
        /// </summary>
        public virtual bool IsSynchronous
        {
            get
            {
                return false;
            }
        }

        //protected static Exception WrapUnserializableException(Exception ex)
        //{
        //    string message = string.Format(
        //        CultureInfo.InvariantCulture,
        //        "(failed to serialize original Exception - original Exception follows){0}{1}",
        //        Environment.NewLine,
        //        ex);
        //    return new Exception(message);
        //}
    }

    /// <summary>
    /// TestStartedEvent holds information needed to call the TestStarted method.
    /// </summary>
    public class TestStartedEvent : Event
    {
        ITest test;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestStartedEvent"/> class.
        /// </summary>
        /// <param name="test">The test.</param>
        public TestStartedEvent( ITest test )
        {
            this.test = test;
        }

        ///// <summary>
        ///// Gets a value indicating whether this event is delivered synchronously by the NUnit <see cref="EventPump"/>.
        ///// <para>
        ///// If <c>true</c>, and if <see cref="EventQueue.SetWaitHandleForSynchronizedEvents"/> has been used to 
        ///// set a WaitHandle, <see cref="EventQueue.Enqueue"/> blocks its calling thread until the <see cref="EventPump"/>
        ///// thread has delivered the event and sets the WaitHandle.
        ///// </para>
        ///// </summary>
        // Keeping this as a synchronous until we rewrite using multiple autoresetevents
        //public override bool IsSynchronous
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        /// <summary>
        /// Calls TestStarted on the specified listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public override void Send(ITestListener listener)
        {
            listener.TestStarted( this.test );
        }
    }

    /// <summary>
    /// TestFinishedEvent holds information needed to call the TestFinished method.
    /// </summary>
    public class TestFinishedEvent : Event
    {
        ITestResult result;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFinishedEvent"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public TestFinishedEvent( ITestResult result )
        {
            this.result = result;
        }

        /// <summary>
        /// Calls TestFinished on the specified listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public override void Send(ITestListener listener)
        {
            listener.TestFinished( this.result );
        }
    }

    /// <summary>
    /// OutputEvent holds information needed to call the TestOutput method.
    /// </summary>
    public class OutputEvent : Event
    {
        TestOutput output;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputEvent"/> class.
        /// </summary>
        /// <param name="output">The output.</param>
        public OutputEvent( TestOutput output )
        {
            this.output = output;
        }

        /// <summary>
        /// Calls TestOutput on the specified listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public override void Send(ITestListener listener)
        {
            listener.TestOutput( this.output );
        }
    }

    #endregion

    /// <summary>
    /// Implements a queue of work items each of which
    /// is queued as a WaitCallback.
    /// </summary>
    public class EventQueue
    {
        private readonly Queue queue = new Queue();
        private readonly object syncRoot;
        private bool stopped;

        /// <summary>
        /// Construct a new EventQueue
        /// </summary>
        public EventQueue()
        {
            this.syncRoot = queue.SyncRoot;
        }

        /// <summary>
        /// WaitHandle for synchronous event delivery in <see cref="Enqueue"/>.
        /// <para>
        /// Having just one handle for the whole <see cref="EventQueue"/> implies that 
        /// there may be only one producer (the test thread) for synchronous events.
        /// If there can be multiple producers for synchronous events, one would have
        /// to introduce one WaitHandle per event.
        /// </para>
        /// </summary>
        private AutoResetEvent synchronousEventSent;

        /// <summary>
        /// Gets the count of items in the queue.
        /// </summary>
        public int Count
        {
            get 
            {
                lock( this.syncRoot )
                {
                    return this.queue.Count; 
                }
            }
        }

        /// <summary>
        /// Sets a handle on which to wait, when <see cref="Enqueue"/> is called
        /// for an <see cref="Event"/> with <see cref="Event.IsSynchronous"/> == true.
        /// </summary>
        /// <param name="synchronousEventWaitHandle">
        /// The wait handle on which to wait, when <see cref="Enqueue"/> is called
        /// for an <see cref="Event"/> with <see cref="Event.IsSynchronous"/> == true.
        /// <para>The caller is responsible for disposing this wait handle.</para>
        /// </param>
        public void SetWaitHandleForSynchronizedEvents(AutoResetEvent synchronousEventWaitHandle)
        {
            this.synchronousEventSent = synchronousEventWaitHandle;
        }

        /// <summary>
        /// Enqueues the specified event
        /// </summary>
        /// <param name="e">The event to enqueue.</param>
        public void Enqueue( Event e )
        {
            lock( this.syncRoot )
            {
                this.queue.Enqueue( e );
                Monitor.Pulse( this.syncRoot );
            }

            if (this.synchronousEventSent != null && e.IsSynchronous)
            {
                this.synchronousEventSent.WaitOne();
            }
            else
            {
                Thread.Sleep(0); // give EventPump thread a chance to process the event
            }
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
            lock (this.syncRoot)
            {
                while (this.queue.Count == 0)
                {
                    if (blockWhenEmpty && !this.stopped)
                    {
                        Monitor.Wait(this.syncRoot);
                    }
                    else
                    {
                        return null;
                    }
                }

                return (Event)this.queue.Dequeue();
            }
        }

        /// <summary>
        /// Stop processing of the queue
        /// </summary>
        public void Stop()
        {
            lock (this.syncRoot)
            {
                if (!this.stopped)
                {
                    this.stopped = true;
                    Monitor.Pulse(this.syncRoot);
                }
            }
        }
    }
}
#endif
