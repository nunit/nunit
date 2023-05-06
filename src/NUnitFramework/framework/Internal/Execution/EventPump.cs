// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        private static readonly Logger log = InternalTrace.GetLogger("EventPump");

        #region Instance Variables

        /// <summary>
        /// The downstream listener to which we send events
        /// </summary>
        private readonly ITestListener _eventListener;

        /// <summary>
        /// The queue that holds our events
        /// </summary>
        private readonly EventQueue _events;

        /// <summary>
        /// Thread to do the pumping
        /// </summary>
        private Thread? _pumpThread;

        /// <summary>
        /// The current state of the event pump
        /// </summary>
        private int _pumpState = (int)EventPumpState.Stopped;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventListener">The EventListener to receive events</param>
        /// <param name="events">The event queue to pull events from</param>
        public EventPump(ITestListener eventListener, EventQueue events)
        {
            _eventListener = eventListener;
            _events = events;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current state of the pump
        /// </summary>
        public EventPumpState PumpState => (EventPumpState)_pumpState;

        /// <summary>
        /// Gets or sets the name of this EventPump
        /// (used only internally and for testing).
        /// </summary>
        public string? Name { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Dispose stops the pump
        /// Disposes the used WaitHandle, too.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        /// Start the pump
        /// </summary>
        public void Start()
        {
            if (Interlocked.CompareExchange(ref _pumpState, (int)EventPumpState.Pumping, (int)EventPumpState.Stopped) == (int)EventPumpState.Stopped)  // Ignore if already started
            {
                _pumpThread = new Thread(PumpThreadProc)
                {
                    Name = "EventPumpThread" + Name,
                    Priority = ThreadPriority.Highest
                };

                _pumpThread.Start();
            }
        }

        /// <summary>
        /// Tell the pump to stop after emptying the queue.
        /// </summary>
        public void Stop()
        {
            if (Interlocked.CompareExchange (ref _pumpState, (int)EventPumpState.Stopping, (int)EventPumpState.Pumping) == (int)EventPumpState.Pumping)
            {
                _events.Stop();
                _pumpThread?.Join();
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
            log.Debug("Starting EventPump");

            //ITestListener hostListeners = CoreExtensions.Host.Listeners;
            try
            {
                while (true)
                {
                    Event? e = _events.Dequeue( PumpState == EventPumpState.Pumping );
                    if ( e is null )
                        break;
                    try
                    {
                        e.Send(_eventListener);
                        //e.Send(hostListeners);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Exception in event handler {0}", ExceptionHelper.BuildStackTrace(ex));
                    }
                }

                log.Debug("EventPump Terminating");
            }
            catch (Exception ex)
            {
                log.Error("Exception in pump thread {0}", ExceptionHelper.BuildStackTrace(ex));
            }
            finally
            {
                _pumpState = (int)EventPumpState.Stopped;
                //pumpThread = null;
                if (_events.Count > 0)
                    log.Error("Event pump thread exiting with {0} events remaining");
            }
        }
        #endregion
    }
}
