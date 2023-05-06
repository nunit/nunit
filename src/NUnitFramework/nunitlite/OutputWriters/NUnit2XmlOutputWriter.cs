// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.IO;
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
        private XmlWriter _xmlWriter;

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
            this._xmlWriter = xmlWriter;

            InitializeXmlFile(result);
            WriteResultElement(result);
            TerminateXmlFile();
        }

        private void InitializeXmlFile(ITestResult result)
        {
            ResultSummary summary = new ResultSummary(result);

            _xmlWriter.WriteStartDocument(false);
            _xmlWriter.WriteComment("This file represents the results of running a test suite");

            _xmlWriter.WriteStartElement("test-results");

            _xmlWriter.WriteAttributeString("name", result.FullName);
            _xmlWriter.WriteAttributeString("total", summary.TestCount.ToString());
            _xmlWriter.WriteAttributeString("errors", summary.ErrorCount.ToString());
            _xmlWriter.WriteAttributeString("failures", summary.FailureCount.ToString());
            _xmlWriter.WriteAttributeString("not-run", summary.NotRunCount.ToString());
            _xmlWriter.WriteAttributeString("inconclusive", summary.InconclusiveCount.ToString());
            _xmlWriter.WriteAttributeString("ignored", summary.IgnoreCount.ToString());
            _xmlWriter.WriteAttributeString("skipped", summary.SkipCount.ToString());
            _xmlWriter.WriteAttributeString("invalid", summary.InvalidCount.ToString());

            _xmlWriter.WriteAttributeString("date", result.StartTime.ToString("yyyy-MM-dd"));
            _xmlWriter.WriteAttributeString("time", result.StartTime.ToString("HH:mm:ss"));
            WriteEnvironment();
            WriteCultureInfo();
        }

        private void WriteCultureInfo()
        {
            _xmlWriter.WriteStartElement("culture-info");
            _xmlWriter.WriteAttributeString("current-culture",
                                           CultureInfo.CurrentCulture.ToString());
            _xmlWriter.WriteAttributeString("current-uiculture",
                                           CultureInfo.CurrentUICulture.ToString());
            _xmlWriter.WriteEndElement();
        }

        private void WriteEnvironment()
        {
            _xmlWriter.WriteStartElement("environment");
            var assemblyName = AssemblyHelper.GetAssemblyName(typeof(NUnit2XmlOutputWriter).Assembly);
            _xmlWriter.WriteAttributeString("nunit-version",
                                           assemblyName.Version.ToString());
            _xmlWriter.WriteAttributeString("clr-version",
                Environment.Version.ToString());
#if NETSTANDARD2_0
            _xmlWriter.WriteAttributeString("os-version",
                                           System.Runtime.InteropServices.RuntimeInformation.OSDescription);
#else
            _xmlWriter.WriteAttributeString("os-version",
                                           OSPlatform.CurrentPlatform.ToString());
#endif
            _xmlWriter.WriteAttributeString("platform",
                Environment.OSVersion.Platform.ToString());
            _xmlWriter.WriteAttributeString("cwd",
                                           Directory.GetCurrentDirectory());
            _xmlWriter.WriteAttributeString("machine-name",
                                           Environment.MachineName);
            _xmlWriter.WriteAttributeString("user",
                                           Environment.UserName);
            _xmlWriter.WriteAttributeString("user-domain",
                                           Environment.UserDomainName);
            _xmlWriter.WriteEndElement();
        }

        private void WriteResultElement(ITestResult result)
        {
            StartTestElement(result);

            WriteCategories(result);
            WriteProperties(result);

            switch (result.ResultState.Status)
            {
                case TestStatus.Skipped:
                    if (result.Message is not null)
                        WriteReasonElement(result.Message);
                    break;
                case TestStatus.Failed:
                    WriteFailureElement(result.Message, result.StackTrace);
                    break;
            }

            if (result.Test is TestSuite)
                WriteChildResults(result);

            _xmlWriter.WriteEndElement(); // test element
        }

        private void TerminateXmlFile()
        {
            _xmlWriter.WriteEndElement(); // test-results
            _xmlWriter.WriteEndDocument();
            _xmlWriter.Flush();
            ((IDisposable)_xmlWriter).Dispose();
        }


        #region Element Creation Helpers

        private void StartTestElement(ITestResult result)
        {
            ITest test = result.Test;

            if (test is TestSuite suite)
            {
                _xmlWriter.WriteStartElement("test-suite");
                _xmlWriter.WriteAttributeString("type", suite.TestType == "ParameterizedMethod" ? "ParameterizedTest" : suite.TestType);
                _xmlWriter.WriteAttributeString("name", suite.TestType == "Assembly" || suite.TestType == "Project"
                    ? result.Test.FullName
                    : result.Test.Name);
            }
            else
            {
                _xmlWriter.WriteStartElement("test-case");
                _xmlWriter.WriteAttributeString("name", result.Name);
            }

            if (test.Properties.ContainsKey(PropertyNames.Description))
            {
                string description = (string)test.Properties.Get(PropertyNames.Description);
                _xmlWriter.WriteAttributeString("description", description);
            }

            TestStatus status = result.ResultState.Status;
            string translatedResult = TranslateResult(result.ResultState);

            if (status != TestStatus.Skipped)
            {
                _xmlWriter.WriteAttributeString("executed", "True");
                _xmlWriter.WriteAttributeString("result", translatedResult);
                _xmlWriter.WriteAttributeString("success", status == TestStatus.Passed ? "True" : "False");
                _xmlWriter.WriteAttributeString("time", result.Duration.ToString("0.000", NumberFormatInfo.InvariantInfo));
                _xmlWriter.WriteAttributeString("asserts", result.AssertCount.ToString());
            }
            else
            {
                _xmlWriter.WriteAttributeString("executed", "False");
                _xmlWriter.WriteAttributeString("result", translatedResult);
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
                _xmlWriter.WriteStartElement("categories");

                foreach (string category in properties[PropertyNames.Category])
                {
                    _xmlWriter.WriteStartElement("category");
                    _xmlWriter.WriteAttributeString("name", category);
                    _xmlWriter.WriteEndElement();
                }

                _xmlWriter.WriteEndElement();
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
                        _xmlWriter.WriteStartElement("properties");

                    foreach (object prop in properties[key])
                    {
                        _xmlWriter.WriteStartElement("property");
                        _xmlWriter.WriteAttributeString("name", key);
                        _xmlWriter.WriteAttributeString("value", prop.ToString());
                        _xmlWriter.WriteEndElement();
                    }
                }
            }

            if (nprops > 0)
                _xmlWriter.WriteEndElement();
        }

        private void WriteReasonElement(string message)
        {
            _xmlWriter.WriteStartElement("reason");
            _xmlWriter.WriteStartElement("message");
            WriteCData(message);
            _xmlWriter.WriteEndElement();
            _xmlWriter.WriteEndElement();
        }

        private void WriteFailureElement(string message, string stackTrace)
        {
            _xmlWriter.WriteStartElement("failure");
            _xmlWriter.WriteStartElement("message");
            WriteCData(message);
            _xmlWriter.WriteEndElement();
            _xmlWriter.WriteStartElement("stack-trace");
            if (stackTrace is not null)
                WriteCData(stackTrace);
            _xmlWriter.WriteEndElement();
            _xmlWriter.WriteEndElement();
        }

        private void WriteChildResults(ITestResult result)
        {
            _xmlWriter.WriteStartElement("results");

            foreach (ITestResult childResult in result.Children)
                WriteResultElement(childResult);

            _xmlWriter.WriteEndElement();
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
                int illegal = text.IndexOf("]]>", start, StringComparison.Ordinal);
                if (illegal < 0)
                    break;
                _xmlWriter.WriteCData(text.Substring(start, illegal - start + 2));
                start = illegal + 2;
                if (start >= text.Length)
                    return;
            }

            if (start > 0)
                _xmlWriter.WriteCData(text.Substring(start));
            else
                _xmlWriter.WriteCData(text);
        }

#endregion
    }
}
