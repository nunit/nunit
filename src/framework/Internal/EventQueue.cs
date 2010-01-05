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
		abstract public void Send( ITestListener listener );
	}

	public class TestStartedEvent : Event
	{
		ITest test;

		public TestStartedEvent( ITest test )
		{
			this.test = test;
		}

		public override void Send( ITestListener listener )
		{
			listener.TestStarted( this.test );
		}
	}
			
	public class TestFinishedEvent : Event
	{
		TestResult result;

		public TestFinishedEvent( TestResult result )
		{
			this.result = result;
		}

		public override void Send( ITestListener listener )
		{
			listener.TestFinished( this.result );
		}
	}

	public class OutputEvent : Event
	{
		TestOutput output;

		public OutputEvent( TestOutput output )
		{
			this.output = output;
		}

		public override void Send( ITestListener listener )
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

		public void Enqueue( Event e )
		{
			lock( this )
			{
				this.queue.Enqueue( e );
				Monitor.Pulse( this );
			}
		}

		public Event Dequeue()
		{
			lock( this )
			{
				return (Event)this.queue.Dequeue();
			}
		}
	}
}
