// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The ITestListenerExt interface is used internally to receive
    /// notifications of significant events while a test is being
    /// run. The events are propagated to clients by means of an
    /// AsyncCallback. NUnit extensions may also monitor these events.
    ///
    /// It is an extension to <see cref="ITestListener"/> that
    /// covers events for <see cref="OneTimeSetUpAttribute"/>
    /// and <see cref="OneTimeTearDownAttribute"/> methods.
    /// </summary>
    public interface ITestListenerExt
    {
        /// <summary>
        /// Called when a OneTimeSetUp has just started.
        /// </summary>
        /// <param name="test">The test to which this OneTimeSetUp belongs.</param>
        void OneTimeSetUpStarted(ITest test);

        /// <summary>
        /// Called when a OneTimeSetUp has just finished.
        /// </summary>
        /// <param name="test">The test to which this OneTimeSetUp belongs.</param>
        void OneTimeSetUpFinished(ITest test);

        /// <summary>
        /// Called when a OneTimeTearDown has just started.
        /// </summary>
        /// <param name="test">The test to which this OneTimeTearDown belongs.</param>
        void OneTimeTearDownStarted(ITest test);

        /// <summary>
        /// Called when a OneTimeTearDown has just finished.
        /// </summary>
        /// <param name="test">The test to which this OneTimeTearDown belongs.</param>
        void OneTimeTearDownFinished(ITest test);
    }
}
