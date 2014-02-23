// ***********************************************************************
// Copyright (c) 2006 Charlie Poole
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
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// The EventPumpState enum represents the state of an
    /// EventPump.
    /// </summary>
    public enum EventPumpState
    {
        /// <summary>
        /// The pump is stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// The pump is pumping events with no stop requested
        /// </summary>
        Pumping,

        /// <summary>
        /// The pump is pumping events but a stop has been requested
        /// </summary>
        Stopping
    }

    /// <summary>
    /// EventPump pulls events out of an EventQueue and sends
    /// them to a listener. It is used to send events back to
    /// the client without using the CallContext of the test
    /// runner thread.
    /// </summary>
    public class EventPump : IDisposable
    {
        static Logger log = InternalTrace.GetLogger("EventPump");

        #region Instance Variables

        /// <summary>
        /// The handle on which a thread enqueuing an event with <see cref="Event.IsSynchronous"/> == <c>true</c>
        /// waits, until the EventPump has sent the event to its listeners.
        /// </summary>
        private readonly AutoResetEvent synchronousEventSent = new AutoResetEvent(false);

        /// <summary>
        /// The downstream listener to which we send events
        /// </summary>
        private ITestListener eventListener;
        
        /// <summary>
        /// The queue that holds our events
        /// </summary>
        EventQueue events;
        
        /// <summary>
        /// Thread to do the pumping
        /// </summary>
        Thread pumpThread;

        /// <summary>
        /// The current state of the eventpump
        /// </summary>
        private volatile EventPumpState pumpState = EventPumpState.Stopped;

        private string name;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventListener">The EventListener to receive events</param>
        /// <param name="events">The event queue to pull events from</param>
        public EventPump( ITestListener eventListener, EventQueue events)
        {
            this.eventListener = eventListener;
            this.events = events;
            this.events.SetWaitHandleForSynchronizedEvents(this.synchronousEventSent);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current state of the pump
        /// </summary>
        /// <remarks>
        /// On <c>volatile</c> and <see cref="Thread.MemoryBarrier"/>, see
        /// "http://www.albahari.com/threading/part4.aspx".
        /// </remarks>
        public EventPumpState PumpState
        {
            get 
            {
                Thread.MemoryBarrier();
                return pumpState; 
            }

            set
            {
                this.pumpState = value;
                Thread.MemoryBarrier();
            }
        }

        /// <summary>
        /// Gets or sets the name of this EventPump
        /// (used only internally and for testing).
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Dispose stops the pump
        /// Disposes the used WaitHandle, too.
        /// </summary>
        public void Dispose()
        {
            Stop();
            ((IDisposable)this.synchronousEventSent).Dispose();
        }

        /// <summary>
        /// Start the pump
        /// </summary>
        public void Start()
        {
            if ( this.PumpState == EventPumpState.Stopped )  // Ignore if already started
            {
                this.pumpThread = new Thread( new ThreadStart( PumpThreadProc ) );
                this.pumpThread.Name = "EventPumpThread" + this.Name;
                this.pumpThread.Priority = ThreadPriority.Highest;
                pumpState = EventPumpState.Pumping;
                this.pumpThread.Start();
            }
        }

        /// <summary>
        /// Tell the pump to stop after emptying the queue.
        /// </summary>
        public void Stop()
        {
            if ( pumpState == EventPumpState.Pumping ) // Ignore extra calls
            {
                this.PumpState = EventPumpState.Stopping;
                this.events.Stop();
                this.pumpThread.Join();
            }
        }
        #endregion

        #region PumpThreadProc

        /// <summary>
        /// Our thread proc for removing items from the event
        /// queue and sending them on. Note that this would
        /// need to do more locking if any other thread were
        /// removing events from the queue.
        /// </summary>
        private void PumpThreadProc()
        {
            //ITestListener hostListeners = CoreExtensions.Host.Listeners;
            try
            {
                while (true)
                {
                    Event e = this.events.Dequeue( this.PumpState == EventPumpState.Pumping );
                    if ( e == null )
                        break;
                    try 
                    {
                        e.Send(this.eventListener);
                        //e.Send(hostListeners);
                    }
                    catch (Exception ex)
                    {
                        log.Error( "Exception in event handler\r\n {0}", ex );
                    }
                    finally
                    {
                        if ( e.IsSynchronous )
                            this.synchronousEventSent.Set();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error( "Exception in pump thread", ex );
            }
            finally
            {
                this.PumpState = EventPumpState.Stopped;
                //pumpThread = null;
            }
        }
        #endregion
    }
}
#endif
