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
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
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
		private Queue queue = new Queue();

        /// <summary>
        /// Gets the count of items in the queue.
        /// </summary>
		public int Count
		{
			get 
			{
				lock( this )
				{
					return this.queue.Count; 
				}
			}
		}

        /// <summary>
        /// Enqueues the specified event
        /// </summary>
        /// <param name="e">The event to enqueue.</param>
		public void Enqueue( Event e )
		{
			lock( this )
			{
				this.queue.Enqueue( e );
				Monitor.Pulse( this );
			}
		}

        /// <summary>
        /// Removes the next event in the queue.
        /// </summary>
        /// <returns>The next event.</returns>
		public Event Dequeue()
		{
			lock( this )
			{
				return (Event)this.queue.Dequeue();
			}
		}
	}
}
