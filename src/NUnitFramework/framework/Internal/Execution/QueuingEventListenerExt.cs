// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// QueuingEventListenerExt uses an EventQueue to store any
    /// extended events received on its EventListener interface.
    /// </summary>
    public class QueuingEventListenerExt : ITestListenerExt
    {
        /// <summary>
        /// The EventQueue for extended events
        /// created and filled by the extended events listener
        /// </summary>
        public EventQueueExt Events { get; }

        /// <summary>
        /// Construct a QueuingEventListener
        /// </summary>
        public QueuingEventListenerExt()
        {
            Events = new EventQueueExt();
        }

        #region EventListener Methods

        /// <summary>
        /// Called when a OneTimeSetUp has started
        /// </summary>
        /// <param name="test">Information about the OneTimeSetUp method that has started. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeSetUpStarted(ITest test)
        {
            Events.Enqueue(new OneTimeSetUpStartedEvent(test));
        }

        /// <summary>
        /// Called when a OneTimeSetUp has finished
        /// </summary>
        /// <param name="test">Information about the OneTimeSetUp method that has finished. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeSetUpFinished(ITest test)
        {
            Events.Enqueue(new OneTimeSetUpFinishedEvent(test));
        }

        /// <summary>
        /// Called when a OneTimeTearDown has started
        /// </summary>
        /// <param name="test">Information about the OneTimeTearDown method that has started. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeTearDownStarted(ITest test)
        {
            Events.Enqueue(new OneTimeTearDownStartedEvent(test));
        }

        /// <summary>
        /// Called when a OneTimeTearDown has finished
        /// </summary>
        /// <param name="test">Information about the OneTimeTearDown method that has finished. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeTearDownFinished(ITest test)
        {
            Events.Enqueue(new OneTimeTearDownFinishedEvent(test));
        }

        #endregion
    }
}
