﻿// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;
using System.IO;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Addins
{
    [Extension]
    [ExtensionProperty("Format", "nunit2")]
    public class NUnit2XmlResultWriter : IResultWriter
    {
        private XmlWriter xmlWriter;

        /// <summary>
        /// Checks if the output is writable. If the output is not
        /// writable, this method should throw an exception.
        /// </summary>
        /// <param name="outputPath"></param>
        public void CheckWritability(string outputPath)
        {
            using ( new StreamWriter( outputPath, false, Encoding.UTF8 ) ) { }
        }

        public void WriteResultFile(XmlNode result, string outputPath)
        {
            using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                WriteResultFile(result, writer);
            }
        }

        public void WriteResultFile(XmlNode result, TextWriter writer)
        {
            using (var xmlWriter = new XmlTextWriter(writer))
            {
                xmlWriter.Formatting = Formatting.Indented;
                WriteXmlOutput(result, xmlWriter);
            }
        }

        private void WriteXmlOutput(XmlNode result, XmlWriter xmlWriter)
        {
            this.xmlWriter = xmlWriter;

            InitializeXmlFile(result);

            foreach (XmlNode child in result.ChildNodes)
                if (child.Name.StartsWith("test-"))
                    WriteResultElement(child);

            TerminateXmlFile();
        }

        private void InitializeXmlFile(XmlNode result)
        {
            NUnit2ResultSummary summary = new NUnit2ResultSummary(result);

            xmlWriter.WriteStartDocument(false);
            xmlWriter.WriteComment("This file represents the results of running a test suite");

            xmlWriter.WriteStartElement("test-results");

            xmlWriter.WriteAttributeString("name", GetPathOfFirstTestFile(result));
            xmlWriter.WriteAttributeString("total", summary.ResultCount.ToString());
            xmlWriter.WriteAttributeString("errors", summary.Errors.ToString());
            xmlWriter.WriteAttributeString("failures", summary.Failures.ToString());
            xmlWriter.WriteAttributeString("not-run", summary.TestsNotRun.ToString());
            xmlWriter.WriteAttributeString("inconclusive", summary.Inconclusive.ToString());
            xmlWriter.WriteAttributeString("ignored", summary.Ignored.ToString());
            xmlWriter.WriteAttributeString("skipped", summary.Skipped.ToString());
            xmlWriter.WriteAttributeString("invalid", summary.NotRunnable.ToString());
            
            DateTime start = result.GetAttribute("start-time", DateTime.UtcNow);
            xmlWriter.WriteAttributeString("date", start.ToString("yyyy-MM-dd"));
            xmlWriter.WriteAttributeString("time", start.ToString("HH:mm:ss"));
            WriteEnvironment();
            WriteCultureInfo();
        }

        private string GetPathOfFirstTestFile(XmlNode resultNode)
        {
            foreach (XmlNode child in resultNode.ChildNodes)
                if (child.Name == "test-suite")
                    return child.GetAttribute("fullname");

            return "UNKNOWN";
        }

        private void WriteCultureInfo()
        {
            xmlWriter.WriteStartElement("culture-info");
            xmlWriter.WriteAttributeString("current-culture",
                                           CultureInfo.CurrentCulture.ToString());
            xmlWriter.WriteAttributeString("current-uiculture",
                                           CultureInfo.CurrentUICulture.ToString());
            xmlWriter.WriteEndElement();
        }

        private void WriteEnvironment()
        {
            xmlWriter.WriteStartElement("environment");
            xmlWriter.WriteAttributeString("nunit-version",
                                           Assembly.GetExecutingAssembly().GetName().Version.ToString());
            xmlWriter.WriteAttributeString("clr-version",
                                           Environment.Version.ToString());
            xmlWriter.WriteAttributeString("os-version",
                                           Environment.OSVersion.ToString());
            xmlWriter.WriteAttributeString("platform",
                Environment.OSVersion.Platform.ToString());
            xmlWriter.WriteAttributeString("cwd",
                                           Environment.CurrentDirectory);
            xmlWriter.WriteAttributeString("machine-name",
                                           Environment.MachineName);
            xmlWriter.WriteAttributeString("user",
                                           Environment.UserName);
            xmlWriter.WriteAttributeString("user-domain",
                                           Environment.UserDomainName);
            xmlWriter.WriteEndElement();
        }

        private string TranslateResult(string resultState, string label)
        {
            switch (resultState)
            {
                default:
                case "Passed":
                    return "Success";
                case "Inconclusive":
                    return "Inconclusive";
                case "Failed":
                    switch (label)
                    {
                        case "Error":
                        case "Cancelled":
                            return label;
                        default:
                            return "Failure";
                    }
                case "Skipped":
                    switch (label)
                    {
                        case "Ignored":
                            return "Ignored";
                        case "Invalid":
                            return "NotRunnable";
                        default:
                            return "Skipped";
                    }
            }
        }

        private void WriteResultElement(XmlNode result)
        {
            StartTestElement(result);

            var properties = result.SelectSingleNode("properties");
            if (properties != null)
            {
                WriteCategoriesElement(properties);
                WritePropertiesElement(properties);
            }

            var message = result.SelectSingleNode("reason/message");
            if (message != null)
                WriteReasonElement(message.InnerText);

            message = result.SelectSingleNode("failure/message");
            var stackTrace = result.SelectSingleNode("failure/stack-trace");
            if (message != null)
                WriteFailureElement(message.InnerText, stackTrace == null ? null : stackTrace.InnerText);

            if (result.Name != "test-case")
                WriteChildResults(result);

            xmlWriter.WriteEndElement(); // test element
        }

        private void TerminateXmlFile()
        {
            xmlWriter.WriteEndElement(); // test-results
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
        }


        #region Element Creation Helpers

        private void StartTestElement(XmlNode result)
        {
            if (result.Name == "test-case")
            {
                xmlWriter.WriteStartElement("test-case");
                xmlWriter.WriteAttributeString("name", result.GetAttribute("fullname"));
            }
            else
            {
                var suiteType = result.GetAttribute("type");
                xmlWriter.WriteStartElement("test-suite");
                xmlWriter.WriteAttributeString("type", suiteType == "ParameterizedMethod" ? "ParameterizedTest" : suiteType);
                string nameAttr = suiteType == "Assembly" || suiteType == "Project" ? "fullname" : "name";
                xmlWriter.WriteAttributeString("name", result.GetAttribute(nameAttr));
            }

            var descNode = result.SelectSingleNode("properties/property[@name='Description']");
            if (descNode != null)
            {
                string description = descNode.GetAttribute("value");
                if (description != null)
                    xmlWriter.WriteAttributeString("description", description);
            }

            string resultState = result.GetAttribute("result");
            string label = result.GetAttribute("label");
            string executed = resultState == "Skipped" ? "False" : "True";
            string success = resultState == "Passed" ? "True" : "False";

            double duration = result.GetAttribute("duration", 0.0);
            string asserts = result.GetAttribute("asserts");

            xmlWriter.WriteAttributeString("executed", executed);
            xmlWriter.WriteAttributeString("result", TranslateResult(resultState, label));

            if (executed == "True")
            {
                xmlWriter.WriteAttributeString("success", success);
                xmlWriter.WriteAttributeString("time", duration.ToString("#####0.000", NumberFormatInfo.InvariantInfo));
                xmlWriter.WriteAttributeString("asserts", asserts);
            }
        }

        private void WriteCategoriesElement(XmlNode properties)
        {
            var items = properties.SelectNodes("property[@name='Category']");
            if (items.Count == 0)
                return; // No category properties found

            xmlWriter.WriteStartElement("categories");
            foreach (XmlNode item in items)
            {
                xmlWriter.WriteStartElement("category");
                xmlWriter.WriteAttributeString("name", item.GetAttribute("value"));
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        private void WritePropertiesElement(XmlNode properties)
        {
            var items = properties.SelectNodes("property");
            var categories = properties.SelectNodes("property[@name='Category']");
            if (items.Count == categories.Count)
                return; // No non-category properties found

            xmlWriter.WriteStartElement("properties");
            foreach (XmlNode item in items)
            {
                if (item.GetAttribute("name") == "Category")
                    continue;

                xmlWriter.WriteStartElement("property");
                xmlWriter.WriteAttributeString("name", item.GetAttribute("name"));
                xmlWriter.WriteAttributeString("value", item.GetAttribute("value"));
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }

        private void WriteReasonElement(string message)
        {
            xmlWriter.WriteStartElement("reason");
            xmlWriter.WriteStartElement("message");
            WriteCData(message);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        private void WriteFailureElement(string message, string stackTrace)
        {
            xmlWriter.WriteStartElement("failure");
            xmlWriter.WriteStartElement("message");
            WriteCData(message);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("stack-trace");
            if (stackTrace != null)
                WriteCData(stackTrace);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        private void WriteChildResults(XmlNode result)
        {
            xmlWriter.WriteStartElement("results");

            foreach (XmlNode childResult in result.ChildNodes)
                if (childResult.Name.StartsWith("test-"))
                    WriteResultElement(childResult);

            xmlWriter.WriteEndElement();
        }
        #endregion

        #region Output Helpers
        /// <summary>
        /// Makes string safe for xml parsing, replacing control chars with '?'
        /// </summary>
        /// <param name="encodedString">string to make safe</param>
        /// <returns>xml safe string</returns>
        private static string CharacterSafeString(string encodedString)
        {
            /*The default code page for the system will be used.
            Since all code pages use the same lower 128 bytes, this should be sufficient
            for finding uprintable control characters that make the xslt processor error.
            We use characters encoded by the default code page to avoid mistaking bytes as
            individual characters on non-latin code pages.*/
            char[] encodedChars = System.Text.Encoding.Default.GetChars(System.Text.Encoding.Default.GetBytes(encodedString));

            System.Collections.ArrayList pos = new System.Collections.ArrayList();
            for (int x = 0; x < encodedChars.Length; x++)
            {
                char currentChar = encodedChars[x];
                //unprintable characters are below 0x20 in Unicode tables
                //some control characters are acceptable. (carriage return 0x0D, line feed 0x0A, horizontal tab 0x09)
                if (currentChar < 32 && (currentChar != 9 && currentChar != 10 && currentChar != 13))
                {
                    //save the array index for later replacement.
                    pos.Add(x);
                }
            }
            foreach (int index in pos)
            {
                encodedChars[index] = '?';//replace unprintable control characters with ?(3F)
            }
            return System.Text.Encoding.Default.GetString(System.Text.Encoding.Default.GetBytes(encodedChars));
        }

        private void WriteCData(string text)
        {
            int start = 0;
            while (true)
            {
                int illegal = text.IndexOf("]]>", start);
                if (illegal < 0)
                    break;
                xmlWriter.WriteCData(text.Substring(start, illegal - start + 2));
                start = illegal + 2;
                if (start >= text.Length)
                    return;
            }

            if (start > 0)
                xmlWriter.WriteCData(text.Substring(start));
            else
                xmlWriter.WriteCData(text);
        }

        #endregion
    }
}
