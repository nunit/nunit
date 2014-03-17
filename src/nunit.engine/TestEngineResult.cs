// ***********************************************************************
// Copyright (c) 2011-2014 Charlie Poole
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
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using NUnit.Engine.Internal;

namespace NUnit.Engine
{
    /// <summary>
    /// Wrapper class for the xml-formatted results produced
    /// by the test engine for most operations. The XML is
    /// stored as a string in order to allow serialization
    /// and actual XmlNodes are created on demand.
    /// 
    /// In principal, there should only be one XmlNode in
    /// a result. However, as work progresses, there may
    /// temporarily be multiple nodes, which have not yet
    /// been aggregated under a higher level suite. For
    /// that reason, TestEngineResult maintains a list
    /// of XmlNodes and another of the corresponding text.
    /// 
    /// Static methods are provided for aggregating the
    /// internal XmlNodes into a single node as well as
    /// for combining multiple TestEngineResults into one.
    /// 
    /// </summary>
    [Serializable]
    public class TestEngineResult
    {
        private List<string> _xmlText = new List<string>();

        [NonSerialized]
        private List<XmlNode> _xmlNodes = new List<XmlNode>();

        #region Constructors

        /// <summary>
        /// Construct a TestResult from an XmlNode
        /// </summary>
        /// <param name="xml">An XmlNode representing the result</param>
        public TestEngineResult(XmlNode xml)
        {
            this._xmlNodes.Add(xml);
            this._xmlText.Add(xml.OuterXml);
        }

        /// <summary>
        /// Construct a test from a string holding xml
        /// </summary>
        /// <param name="xml">A string containing the xml result</param>
        public TestEngineResult(string xml)
        {
            this._xmlText.Add(xml);
        }

        /// <summary>
        /// Default constructor used when adding multiple results
        /// </summary>
        public TestEngineResult()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a flag indicating whether this is a single result
        /// having only one XmlNode associated with it.
        /// </summary>
        public bool IsSingle 
        {
            get { return _xmlText.Count == 1; }
        }

        /// <summary>
        /// Gets the xml representing a test result as an XmlNode
        /// </summary>
        public IList<XmlNode> XmlNodes
        {
            get
            {
                // xmlNodes might be null after deserialization
                if (_xmlNodes == null)
                    _xmlNodes = new List<XmlNode>();

                for (int i = _xmlNodes.Count; i < _xmlText.Count; i++)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_xmlText[i]);
                    _xmlNodes.Add(doc.FirstChild);
                }

                return _xmlNodes;
            }
        }

        /// <summary>
        /// Gets the XML representing a single test result.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If the result is empty or has multiple XML nodes.
        /// </exception>
        public XmlNode Xml
        {
            get 
            {
                // TODO: Fix this in some way and also allow for a count of zero
                if (!IsSingle)
                    throw new InvalidOperationException("May not use 'Xml' property on a result with multiple XmlNodes");
                    
                return XmlNodes[0]; 
            }
        }

        public void Add(string xml)
        {
            this._xmlText.Add(xml);
        }

        public void Add(XmlNode xml)
        {
            this._xmlText.Add(xml.OuterXml);
            this._xmlNodes.Add(xml);
        }

        #endregion
    }
}
