// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

namespace NUnit.Framework.Interfaces
{
    using System;

    /// <summary>
    /// The TestOutput class holds a unit of output from 
    /// a test to stdOut, stdErr, Trace or a logger.
    /// </summary>
    public class TestOutput : IXmlNodeBuilder
    {
        string text;
        TestOutputType type;

        /// <summary>
        /// Construct with text and an ouput destination type
        /// </summary>
        /// <param name="text">Text to be output</param>
        /// <param name="type">Destination of output</param>
        public TestOutput(string text, TestOutputType type)
        {
            this.text = text;
            this.type = type;
        }

        /// <summary>
        /// Return string representation of the object for debugging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return type + ": " + text;
        }

        /// <summary>
        /// Get the text 
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
        }

        /// <summary>
        /// Get the output type
        /// </summary>
        public TestOutputType Type
        {
            get
            {
                return this.type;
            }
        }

        #region IXmlNodeBuilder Members

        /// <summary>
        /// Returns an XmlNode representing the current object.
        /// </summary>
        /// <param name="recursive">If true, children are included where applicable</param>
        /// <returns>
        /// An XmlNode representing the result
        /// </returns>
        public XmlNode ToXml(bool recursive)
        {
            XmlNode topNode = XmlNode.CreateTopLevelElement("dummy");

            XmlNode thisNode = AddToXml(topNode, recursive);

            return thisNode;
        }

        /// <summary>
        /// Returns an XmlNode representing the current object after
        /// adding it as a child of the supplied parent node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">If true, children are included, where applicable</param>
        /// <returns></returns>
        public XmlNode AddToXml(XmlNode parentNode, bool recursive)
        {
            XmlNode thisNode = parentNode.AddElement("output");
            thisNode.AddAttribute("type", this.Type.ToString());
            thisNode.AddAttribute("text", this.Text);

            return thisNode;
        }

        #endregion
    }

    /// <summary>
    /// Enum representing the output destination.
    /// </summary>
    ///// TODO: Decide if we want to change this to
    ///// a combinable flag as it was in the past.
    ///// Old summary:
    ///// It uses combinable flags so that a given
    ///// output control can accept multiple types
    ///// of output. Normally, each individual
    ///// output uses a single flag value.
    public enum TestOutputType
    {
        /// <summary>
        /// Send output to stdOut
        /// </summary>
        Out, 
        
        /// <summary>
        /// Send output to stdErr
        /// </summary>
        Error,

        /// <summary>
        /// Send output to Trace
        /// </summary>
        Trace,

        /// <summary>
        /// Send output to Log
        /// </summary>
        Log
    }
}
