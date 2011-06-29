using System;
using System.Xml;

namespace NUnit.Engine
{
    /// <summary>
    /// Wrapper class for the xml test result produced
    /// when running a test.
    /// </summary>
    [Serializable]
    public class TestResult
    {
        private string xml;

        /// <summary>
        /// Construct a TestResult from an XmlNode
        /// </summary>
        /// <param name="xml">An XmlNode representing the result</param>
        public TestResult(XmlNode xml)
        {
            this.xml = xml.OuterXml;
        }

        /// <summary>
        /// Construct a test from a string holding xml
        /// </summary>
        /// <param name="xml">A string containing the xml result</param>
        public TestResult(string xml)
        {
            this.xml = xml;
        }

        /// <summary>
        /// Return the xml representing a test result as an XmlNode
        /// </summary>
        /// <returns>An XmlNode representing the result</returns>
        public XmlNode GetXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.FirstChild;
        }
    }
}
