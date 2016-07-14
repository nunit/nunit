// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// InternalTraceLevel is an enumeration controlling the
    /// level of detailed presented in the internal log.
    /// </summary>
    public enum InternalTraceLevel
    {
        /// <summary>
        /// Use the default settings as specified by the user.
        /// </summary>
        Default,

        /// <summary>
        /// Do not display any trace messages
        /// </summary>
        Off,

        /// <summary>
        /// Display Error messages only
        /// </summary>
        Error,

        /// <summary>
        /// Display Warning level and higher messages
        /// </summary>
        Warning,

        /// <summary>
        /// Display informational and higher messages
        /// </summary>
        Info,

        /// <summary>
        /// Display debug messages and higher - i.e. all messages
        /// </summary>
        Debug,

        /// <summary>
        /// Display debug messages and higher - i.e. all messages
        /// </summary>
        Verbose = Debug
    }
}
