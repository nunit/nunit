// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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

#if !(NET20 || NET35 || NET40 || NET45)
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
