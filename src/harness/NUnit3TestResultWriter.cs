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
using System.Reflection;
using System.Text;
using System.Xml;
using System.IO;
using NUnit.Framework.Internal;

namespace NUnit.Framework.TestHarness
{
    /// <summary>
    /// NUnit3xmlOutputWriter is responsible for writing the results
    /// of a test to a file in NUnit 3.0 format.
    /// </summary>
    public class NUnit3TestResultWriter
    {
        private XmlWriter xmlWriter;

        public void WriteResultFile(XmlNode resultNode, string outputPath)
        {
            using (StreamWriter writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                WriteResultFile(resultNode, writer);
            }
        }

        public void WriteResultFile(XmlNode resultNode, TextWriter writer)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
            {
                WriteXmlOutput(resultNode, xmlWriter);
            }
        }

        private void WriteXmlOutput(XmlNode resultNode, XmlWriter xmlWriter)
        {
            this.xmlWriter = xmlWriter;

            InitializeXmlFile(resultNode);

            resultNode.WriteTo(xmlWriter);

            TerminateXmlFile();
        }

        private void InitializeXmlFile(XmlNode resultNode)
        {
            xmlWriter.WriteStartDocument(false);

            // In order to match the format used by NUnit 3.0, we
            // wrap the entire result from the framework in a 
            // <test-run> element.
            xmlWriter.WriteStartElement("test-run");
            xmlWriter.WriteAttributeString("id", "2"); // TODO: Should not be hard-coded
            xmlWriter.WriteAttributeString("name", XmlHelper.GetAttribute(resultNode, "name"));
            xmlWriter.WriteAttributeString("fullname", XmlHelper.GetAttribute(resultNode, "fullname"));
            xmlWriter.WriteAttributeString("testcasecount", XmlHelper.GetAttribute(resultNode, "testcasecount"));

            xmlWriter.WriteAttributeString("result", XmlHelper.GetAttribute(resultNode, "result"));
            var label = XmlHelper.GetAttribute(resultNode, "label");
            if (label != null)
                xmlWriter.WriteAttributeString("label", label);

            xmlWriter.WriteAttributeString("start-time", XmlHelper.GetAttribute(resultNode, "start-time"));
            xmlWriter.WriteAttributeString("end-time", XmlHelper.GetAttribute(resultNode, "end-time"));
            xmlWriter.WriteAttributeString("duration", XmlHelper.GetAttribute(resultNode, "duration"));

            xmlWriter.WriteAttributeString("total", XmlHelper.GetAttribute(resultNode, "total"));
            xmlWriter.WriteAttributeString("passed", XmlHelper.GetAttribute(resultNode, "passed"));
            xmlWriter.WriteAttributeString("failed", XmlHelper.GetAttribute(resultNode, "failed"));
            xmlWriter.WriteAttributeString("inconclusive", XmlHelper.GetAttribute(resultNode, "inconclusive"));
            xmlWriter.WriteAttributeString("skipped", XmlHelper.GetAttribute(resultNode, "skipped"));
            xmlWriter.WriteAttributeString("asserts", XmlHelper.GetAttribute(resultNode, "asserts"));


            xmlWriter.WriteAttributeString("random-seed", Randomizer.InitialSeed.ToString());

            WriteEnvironmentElement();
        }

        private void WriteEnvironmentElement()
        {
            xmlWriter.WriteStartElement("environment");

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = AssemblyHelper.GetAssemblyName(assembly);
            xmlWriter.WriteAttributeString("nunit-version", assemblyName.Version.ToString());

            xmlWriter.WriteAttributeString("clr-version", Environment.Version.ToString());
            xmlWriter.WriteAttributeString("os-version", Environment.OSVersion.ToString());
            xmlWriter.WriteAttributeString("platform", Environment.OSVersion.Platform.ToString());
            xmlWriter.WriteAttributeString("cwd", Environment.CurrentDirectory);
            xmlWriter.WriteAttributeString("machine-name", Environment.MachineName);
            xmlWriter.WriteAttributeString("user", Environment.UserName);
            xmlWriter.WriteAttributeString("user-domain", Environment.UserDomainName);
            xmlWriter.WriteAttributeString("culture", System.Globalization.CultureInfo.CurrentCulture.ToString());
            xmlWriter.WriteAttributeString("uiculture", System.Globalization.CultureInfo.CurrentUICulture.ToString());

            xmlWriter.WriteEndElement();
        }

        private void TerminateXmlFile()
        {
            xmlWriter.WriteEndElement(); // test-run
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
        }
    }
}
