// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestListenerExt provides an implementation of <see cred="ITestListenerExt"/> that
    /// does nothing. It is used only through its NULL property.
    /// </summary>
    public class TestListenerExt : ITestListenerExt
    {
        /// <summary>
        /// Called when a OneTimeSetUp has started
        /// </summary>
        /// <param name="test">Information about the OneTimeSetUp method that has started. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeSetUpStarted(ITest test)
        {
        }

        /// <summary>
        /// Called when a OneTimeSetUp has finished
        /// </summary>
        /// <param name="test">Information about the OneTimeSetUp method that has finished. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeSetUpFinished(ITest test)
        {
        }

        /// <summary>
        /// Called when a OneTimeTearDown has started
        /// </summary>
        /// <param name="test">Information about the OneTimeTearDown method that has started. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeTearDownStarted(ITest test)
        {
        }

        /// <summary>
        /// Called when a OneTimeTearDown has finished
        /// </summary>
        /// <param name="test">Information about the OneTimeTearDown method that has finished. Needs to be replaced with a more fitting object in the future.</param>
        public void OneTimeTearDownFinished(ITest test)
        {
        }

        /// <summary>
        /// Construct a new TestListenerExt - private so it may not be used.
        /// </summary>
        private TestListenerExt()
        {
        }

        /// <summary>
        /// Get a listener that does nothing
        /// </summary>
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        public static ITestListenerExt NULL => new TestListenerExt();
    }
}
