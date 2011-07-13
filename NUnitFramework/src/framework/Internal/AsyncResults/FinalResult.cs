// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

using System;
using System.Xml;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// FinalResult is an AsyncResult returned after the action has been completed.
    /// The object state is represented by a string containing XML.
    /// </summary>
    [Serializable]
    public class FinalResult : AsyncResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinalResult"/> class
        /// based on a string containing XML.
        /// </summary>
        /// <param name="xmlResult">The result of an operation represented by an XML fragment.</param>
        /// <param name="synchronous">if set to <c>true</c> [synchronous].</param>
        public FinalResult(string xmlResult, bool synchronous) : base(xmlResult, true, synchronous) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FinalResult"/> class
        /// based on an XmlNode.
        /// </summary>
        /// <param name="xmlResult">The result of an operation represented by an XML fragment.</param>
        /// <param name="synchronous">if set to <c>true</c> [synchronous].</param>
        public FinalResult(XmlNode xmlResult, bool synchronous) : base(xmlResult.OuterXml, true, synchronous) { }

        /// <summary>
        /// Protected constructor for use by derived classes.
        /// </summary>
        protected FinalResult() : base(null, true, true) { }
    }
}