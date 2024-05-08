// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics;
#if !NET6_0_OR_GREATER
using System.Security.Permissions;
#endif

namespace NUnit.Framework.Diagnostics
{
    /// <summary><para>
    /// The ProgressTraceListener class allows directing tracing or
    /// debugging output to <see cref="TestContext.Progress"/>.
    /// </para><para>
    /// To activate, place the following snippet into the one-time
    /// set-up method of either a test's fixture or the set-up
    /// fixture of a project:
    /// <c>
    /// System.Trace.Listeners.Add(new ProgressTraceListener());
    /// </c>
    /// </para><para>
    /// Make sure to only add a listener once, e.g.:
    /// <c>
    /// if (!System.Trace.Listeners.OfType&lt;ProgressTraceListener&gt;().Any())
    ///     System.Trace.Listeners.Add(new ProgressTraceListener());
    /// </c>
    /// </para><para>
    /// Or again remove in the one-time tear-down, e.g.:
    /// <c>
    /// System.Trace.Listeners.Remove(_progressTraceListener);
    /// </c>
    /// </para></summary>
    /// <remarks><para>
    /// Although named "Trace", <see cref="TextWriterTraceListener"/>
    /// "directs tracing or debugging output".
    /// </para><para>
    /// This listener is provided by NUnit (i.e. the origin of
    /// <see cref="TestContext.Progress"/>) same as the
    /// <see cref="ConsoleTraceListener"/> is provided by .NET
    /// (the origin of <see cref="Console"/>).
    /// </para></remarks>
    // Implemented following the ConsoleTraceListener implementation.
#if !NET6_0_OR_GREATER
    [HostProtection(Synchronization = true)]
#endif
    public class ProgressTraceListener : TextWriterTraceListener
    {
        #region Constructors

        /// <summary>
        /// Construct a ProgressTraceListener with trace
        /// output written to <see cref="TestContext.Progress"/>
        /// </summary>
        public ProgressTraceListener() : base(TestContext.Progress)
        {
        }

        #endregion
    }
}
