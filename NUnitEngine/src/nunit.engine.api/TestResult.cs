using System;
using System.Xml;

namespace NUnit.Engine
{
    [Serializable]
    public class TestResult
    {
        private string xml;

        public TestResult(XmlNode xml)
        {
            this.xml = xml.OuterXml;
        }

        public TestResult(string xml)
        {
            this.xml = xml;
        }

        public XmlNode GetXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.FirstChild;
        }
    }
}
