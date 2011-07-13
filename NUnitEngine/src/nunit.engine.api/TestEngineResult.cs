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

namespace NUnit.Engine
{
    /// <summary>
    /// Wrapper class for the xml-formatted results produced
    /// by the test engine for most operations. The XML is
    /// stored as a string in order to allow serialization.
    /// </summary>
    [Serializable]
    public class TestEngineResult
    {
        private string xmlText;

        [NonSerialized]
        private XmlNode xmlNode;

        #region Constructors

        /// <summary>
        /// Construct a TestResult from an XmlNode
        /// </summary>
        /// <param name="xml">An XmlNode representing the result</param>
        public TestEngineResult(XmlNode xml)
        {
            this.xmlNode = xml;
            this.xmlText = xml.OuterXml;
        }

        /// <summary>
        /// Construct a test from a string holding xml
        /// </summary>
        /// <param name="xml">A string containing the xml result</param>
        public TestEngineResult(string xml)
        {
            this.xmlText = xml;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a string representing the type of result, which
        /// is the same as the name of the top-level element.
        /// </summary>
        public string ResultType
        {
            get { return Xml.Name; }
        }

        /// <summary>
        /// Gets a flag indicating whether this is an error result
        /// </summary>
        public bool IsError
        {
            get { return Xml.Name == "error"; }
        }

        /// <summary>
        /// Gets the xml string representing a result
        /// </summary>
        public string Text
        {
            get { return xmlText; }
        }

        /// <summary>
        /// Gets the xml representing a test result as an XmlNode
        /// </summary>
        public XmlNode Xml
        {
            get
            {
                if (xmlNode == null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlText);
                    xmlNode = doc.FirstChild;
                }

                return xmlNode;
            }
        }

        public XmlNodeList Errors
        {
            get
            {
                return Xml.SelectNodes("error");
            }
        }

        #endregion
    }
}
