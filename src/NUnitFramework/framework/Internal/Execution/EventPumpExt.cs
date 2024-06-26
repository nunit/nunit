// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// EventPump pulls extended Event instances out of an EventQueue and sends
    /// them to a ITestListenerExt. It is used to send these events back to
    /// the client without using the CallContext of the test
    /// runner thread.
    /// </summary>
    public sealed class EventPumpExt : EventPump<ExtendedEvent, ITestListenerExt>, IDisposable
    {
        /// <summary>
        /// Constructor for extended EventPump
        /// </summary>
        /// <param name="eventListener">The EventListener to receive events</param>
        /// <param name="events">The event queue to pull events from</param>
        public EventPumpExt(ITestListenerExt eventListener, EventQueue<ExtendedEvent> events)
            : base(eventListener, events, "ExtendedEventPump")
        {
        }
    }
}
