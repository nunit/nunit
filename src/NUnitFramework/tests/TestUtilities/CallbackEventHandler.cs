// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestUtilities
{
    public class CallbackEventHandler : System.Web.UI.ICallbackEventHandler
    {
        private string _result;

        public string GetCallbackResult()
        {
            return _result;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            _result = eventArgument;
        }
    }
}
