// ***********************************************************************
// Copyright (c) 2011 Charlie Poole, Rob Prouse
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
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.IO;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnitLite
{
    /// <summary>
    /// NUnit2XmlOutputWriter is able to create an XML file representing
    /// the result of a test run in NUnit 2.x format.
    /// </summary>
    public class NUnit2XmlOutputWriter : OutputWriter
    {
        private XmlWriter xmlWriter;

        /// <summary>
        /// Write info about a test
        /// </summary>
        /// <param name="test">The test</param>
        /// <param name="writer">A TextWriter</param>
        public override void WriteTestFile(ITest test, TextWriter writer)
        {
            throw new NotImplementedException("Explore test output is not supported by the NUnit2 format.");
        }

        /// <summary>
        /// Writes the result of a test run to a specified TextWriter.
        /// </summary>
        /// <param name="result">The test result for the run</param>
        /// <param name="writer">The TextWriter to which the xml will be written</param>
        /// <param name="runSettings"></param>
        /// <param name="filter"></param>
        public override void WriteResultFile(ITestResult result, TextWriter writer, IDictionary<string, object> runSettings, TestFilter filter)
        {
            var settings = new XmlWriterSettings { Indent = true };
            using (var xmlWriter = XmlWriter.Create(writer, settings))
            {
                WriteXmlOutput(result, xmlWriter);
            }
        }

        private void WriteXmlOutput(ITestResult result, XmlWriter xmlWriter)
        {
            this.xmlWriter = xmlWriter;

            InitializeXmlFile(result);
            WriteResultElement(result);
            TerminateXmlFile();
        }

        private void InitializeXmlFile(ITestResult result)
        {
            ResultSummary summary = new ResultSummary(result);

            xmlWriter.WriteStartDocument(false);
            xmlWriter.WriteComment("This file represents the results of running a test suite");

            xmlWriter.WriteStartElement("test-results");

            xmlWriter.WriteAttributeString("name", result.FullName);
            xmlWriter.WriteAttributeString("total", summary.TestCount.ToString());
            xmlWriter.WriteAttributeString("errors", summary.ErrorCount.ToString());
            xmlWriter.WriteAttributeString("failures", summary.FailureCount.ToString());
            var notRunTotal = summary.SkipCount + summary.FailureCount + summary.InvalidCount;
            xmlWriter.WriteAttributeString("not-run", notRunTotal.ToString());
            xmlWriter.WriteAttributeString("inconclusive", summary.InconclusiveCount.ToString());
            xmlWriter.WriteAttributeString("ignored", summary.IgnoreCount.ToString());
            xmlWriter.WriteAttributeString("skipped", summary.SkipCount.ToString());
            xmlWriter.WriteAttributeString("invalid", summary.InvalidCount.ToString());

            xmlWriter.WriteAttributeString("date", result.StartTime.ToString("yyyy-MM-dd"));
            xmlWriter.WriteAttributeString("time", result.StartTime.ToString("HH:mm:ss"));
            WriteEnvironment();
            WriteCultureInfo();
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
            var assemblyName = AssemblyHelper.GetAssemblyName(typeof(NUnit2XmlOutputWriter).GetTypeInfo().Assembly);
            xmlWriter.WriteAttributeString("nunit-version",
                                           assemblyName.Version.ToString());
#if NETSTANDARD1_6
            xmlWriter.WriteAttributeString("clr-version",
                                           System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
#else
            xmlWriter.WriteAttributeString("clr-version",
                Environment.Version.ToString());
#endif
#if !PLATFORM_DETECTION
            xmlWriter.WriteAttributeString("os-version",
                                           System.Runtime.InteropServices.RuntimeInformation.OSDescription);
#else
            xmlWriter.WriteAttributeString("os-version",
                                           OSPlatform.CurrentPlatform.ToString());
#endif
#if !NETSTANDARD1_6
            xmlWriter.WriteAttributeString("platform",
                Environment.OSVersion.Platform.ToString());
#endif
            xmlWriter.WriteAttributeString("cwd",
                                           Directory.GetCurrentDirectory());
            xmlWriter.WriteAttributeString("machine-name",
                                           Environment.MachineName);
#if !NETSTANDARD1_6
            xmlWriter.WriteAttributeString("user",
                                           Environment.UserName);
            xmlWriter.WriteAttributeString("user-domain",
                                           Environment.UserDomainName);
#endif
            xmlWriter.WriteEndElement();
        }

        private void WriteResultElement(ITestResult result)
        {
            StartTestElement(result);

            WriteCategories(result);
            WriteProperties(result);

            switch (result.ResultState.Status)
            {
                case TestStatus.Skipped:
                    if (result.Message != null)
                        WriteReasonElement(result.Message);
                    break;
                case TestStatus.Failed:
                    WriteFailureElement(result.Message, result.StackTrace);
                    break;
            }

            if (result.Test is TestSuite)
                WriteChildResults(result);

            xmlWriter.WriteEndElement(); // test element
        }

        private void TerminateXmlFile()
        {
            xmlWriter.WriteEndElement(); // test-results
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            ((IDisposable)xmlWriter).Dispose();
        }


        #region Element Creation Helpers

        private void StartTestElement(ITestResult result)
        {
            ITest test = result.Test;
            TestSuite suite = test as TestSuite;

            if (suite != null)
            {
                xmlWriter.WriteStartElement("test-suite");
                xmlWriter.WriteAttributeString("type", suite.TestType == "ParameterizedMethod" ? "ParameterizedTest" : suite.TestType);
                xmlWriter.WriteAttributeString("name", suite.TestType == "Assembly" || suite.TestType == "Project"
                    ? result.Test.FullName
                    : result.Test.Name);
            }
            else
            {
                xmlWriter.WriteStartElement("test-case");
                xmlWriter.WriteAttributeString("name", result.Name);
            }

            if (test.Properties.ContainsKey(PropertyNames.Description))
            {
                string description = (string)test.Properties.Get(PropertyNames.Description);
                xmlWriter.WriteAttributeString("description", description);
            }

            TestStatus status = result.ResultState.Status;
            string translatedResult = TranslateResult(result.ResultState);

            if (status != TestStatus.Skipped)
            {
                xmlWriter.WriteAttributeString("executed", "True");
                xmlWriter.WriteAttributeString("result", translatedResult);
                xmlWriter.WriteAttributeString("success", status == TestStatus.Passed ? "True" : "False");
                xmlWriter.WriteAttributeString("time", result.Duration.ToString("0.000", NumberFormatInfo.InvariantInfo));
                xmlWriter.WriteAttributeString("asserts", result.AssertCount.ToString());
            }
            else
            {
                xmlWriter.WriteAttributeString("executed", "False");
                xmlWriter.WriteAttributeString("result", translatedResult);
            }
        }

        private string TranslateResult(ResultState resultState)
        {
            switch (resultState.Status)
            {
                default:
                case TestStatus.Passed:
                    return "Success";
                case TestStatus.Inconclusive:
                    return "Inconclusive";
                case TestStatus.Failed:
                    switch (resultState.Label)
                    {
                        case "Error":
                        case "Cancelled":
                            return resultState.Label;
                        default:
                            return "Failure";
                    }
                case TestStatus.Skipped:
                    switch (resultState.Label)
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

        private void WriteCategories(ITestResult result)
        {
            IPropertyBag properties = result.Test.Properties;

            if (properties.ContainsKey(PropertyNames.Category))
            {
                xmlWriter.WriteStartElement("categories");

                foreach (string category in properties[PropertyNames.Category])
                {
                    xmlWriter.WriteStartElement("category");
                    xmlWriter.WriteAttributeString("name", category);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }
        }

        private void WriteProperties(ITestResult result)
        {
            IPropertyBag properties = result.Test.Properties;
            int nprops = 0;

            foreach (string key in properties.Keys)
            {
                if (key != PropertyNames.Category)
                {
                    if (nprops++ == 0)
                        xmlWriter.WriteStartElement("properties");

                    foreach (object prop in properties[key])
                    {
                        xmlWriter.WriteStartElement("property");
                        xmlWriter.WriteAttributeString("name", key);
                        xmlWriter.WriteAttributeString("value", prop.ToString());
                        xmlWriter.WriteEndElement();
                    }
                }
            }

            if (nprops > 0)
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

        private void WriteChildResults(ITestResult result)
        {
            xmlWriter.WriteStartElement("results");

            foreach (ITestResult childResult in result.Children)
                WriteResultElement(childResult);

            xmlWriter.WriteEndElement();
        }

#endregion

#region Output Helpers
        ///// <summary>
        ///// Makes string safe for xml parsing, replacing control chars with '?'
        ///// </summary>
        ///// <param name="encodedString">string to make safe</param>
        ///// <returns>xml safe string</returns>
        //private static string CharacterSafeString(string encodedString)
        //{
        //    /*The default code page for the system will be used.
        //    Since all code pages use the same lower 128 bytes, this should be sufficient
        //    for finding unprintable control characters that make the xslt processor error.
        //    We use characters encoded by the default code page to avoid mistaking bytes as
        //    individual characters on non-latin code pages.*/
        //    char[] encodedChars = System.Text.Encoding.Default.GetChars(System.Text.Encoding.Default.GetBytes(encodedString));

        //    System.Collections.ArrayList pos = new System.Collections.ArrayList();
        //    for (int x = 0; x < encodedChars.Length; x++)
        //    {
        //        char currentChar = encodedChars[x];
        //        //unprintable characters are below 0x20 in Unicode tables
        //        //some control characters are acceptable. (carriage return 0x0D, line feed 0x0A, horizontal tab 0x09)
        //        if (currentChar < 32 && (currentChar != 9 && currentChar != 10 && currentChar != 13))
        //        {
        //            //save the array index for later replacement.
        //            pos.Add(x);
        //        }
        //    }
        //    foreach (int index in pos)
        //    {
        //        encodedChars[index] = '?';//replace unprintable control characters with ?(3F)
        //    }
        //    return System.Text.Encoding.Default.GetString(System.Text.Encoding.Default.GetBytes(encodedChars));
        //}

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
