// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestUtilities
{
    public class CallbackEventHandler : System.Web.UI.ICallbackEventHandler
    {
        private string? _result;

        public string GetCallbackResult()
        {
            // Use "null" when null is passed.
            // https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.icallbackeventhandler.raisecallbackevent?view=netframework-4.8.1#remarks
            return _result ?? "null";
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            _result = eventArgument;
        }
    }
}
