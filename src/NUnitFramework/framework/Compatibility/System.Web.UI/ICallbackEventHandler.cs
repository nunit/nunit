// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if !NETFRAMEWORK
namespace System.Web.UI
{
    /// <summary>
    /// A shim of the .NET interface for platforms that do not support it.
    /// Used to indicate that a control can be the target of a callback event on the server.
    /// </summary>
    public interface ICallbackEventHandler
    {
        /// <summary>
        /// Processes a callback event that targets a control.
        /// </summary>
        /// <param name="report"></param>
        void RaiseCallbackEvent(string report);

        /// <summary>
        /// Returns the results of a callback event that targets a control.
        /// </summary>
        /// <returns></returns>
        string GetCallbackResult();
    }
}
#endif
