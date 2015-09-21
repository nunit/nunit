// ***********************************************************************
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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NUnit.Core;
using NUnit.Core.Filters;
using NUnit.Engine.Extensibility;


namespace NUnit.Engine.Drivers
{
    [Extension]
    public class NUnit2FrameworkDriver : IFrameworkDriver
    {
        // TODO: The id should not be hard-coded
        private const string LOAD_RESULT_FORMAT =
            "<test-suite type='Assembly' id='{0}' name='{1}' fullname='{2}' testcasecount='0' runstate='NotRunnable'>" +
                "<properties>" +
                    "<property name='_SKIPREASON' value='{3}'/>" +
                "</properties>" +
            "</test-suite>";

        AppDomain _testDomain;
        string _testAssemblyPath;

        string _name;
        string _fullname;

        TestRunner _runner;
        Core.TestPackage _package;

        /// <summary>
        /// Create a new NUnit2FrameworkDriver
        /// </summary>
        /// <param name="testDomain">The AppDomain to use for the runner</param>
        /// <remarks>
        /// The framework assembly name is needed because this driver is used for both the
        /// nunit.framework 2.x and nunitlite 1.0.
        /// </remarks>
        public NUnit2FrameworkDriver(AppDomain testDomain)
        {
            _testDomain = testDomain;

            var initializer = DomainInitializer.CreateInstance(_testDomain);
            initializer.InitializeDomain((int)InternalTrace.Level);

            _runner = RemoteTestRunner.CreateInstance(_testDomain, 1);
        }

        public string ID { get; set; }

        public string Load(string testAssemblyPath, IDictionary<string, object> settings)
        {
            if (!File.Exists(testAssemblyPath))
                throw new ArgumentException("testAssemblyPath", "Framework driver Load called with a file name that doesn't exist.");

            _testAssemblyPath = testAssemblyPath;
            _name = Escape(Path.GetFileName(_testAssemblyPath));
            _fullname = Escape(_testAssemblyPath);

            _package = new Core.TestPackage(_testAssemblyPath);
            foreach (var key in settings.Keys)
                _package.Settings[key] = settings[key];

            if (!_runner.Load(_package))
                return string.Format(LOAD_RESULT_FORMAT, TestID, _name, _fullname, "No tests were found");

            Core.ITest test = _runner.Test;
            // TODO: Handle error where test is null

            return test.ToXml(false).OuterXml;
        }

        public int CountTestCases(string filter)
        {
            ITestFilter v2Filter = CreateNUnit2TestFilter(filter);
            return _runner.CountTestCases(v2Filter);
        }

        public string Run(ITestEventListener listener, string filter)
        {
            if (_runner.Test == null)
                return String.Format(LOAD_RESULT_FORMAT, TestID, _name, _fullname, "Error loading test");

            ITestFilter v2Filter = CreateNUnit2TestFilter(filter);

            var result = _runner.Run(new TestEventAdapter(listener), v2Filter, false, LoggingThreshold.Off);

            return result.ToXml(true).OuterXml;
        }

        public string Explore(string filter)
        {
            if (_runner.Test == null)
                return String.Format(LOAD_RESULT_FORMAT, TestID, _name, _fullname, "Error loading test");

            return _runner.Test.ToXml(true).OuterXml;
        }

        public void StopRun(bool force)
        {
            _runner.CancelRun();
        }

        private static string Escape(string original)
        {
            return original
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        private string TestID
        {
            get { return string.IsNullOrEmpty(ID) ? "1" : ID + "-1";}
        }

        private static ITestFilter CreateNUnit2TestFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return Core.TestFilter.Empty;

            var doc = new XmlDocument();
            doc.LoadXml(filter);
            var topNode = doc.FirstChild;
            if (topNode.Name != "filter")
                throw new Exception("Expected filter element at top level");

            switch (topNode.ChildNodes.Count)
            {
                case 0:
                    return Core.TestFilter.Empty;

                case 1:
                    return FromXml(topNode.FirstChild);

                default:
                    return FromXml(topNode);
            }
        }

        private static readonly char[] COMMA = { ',' };

        private static Core.TestFilter FromXml(XmlNode xmlNode)
        {
            switch (xmlNode.Name)
            {
                case "filter":
                case "and":
                    var andFilter = new Core.Filters.AndFilter();
                    foreach (XmlNode childNode in xmlNode.ChildNodes)
                        andFilter.Add(FromXml(childNode));
                    return andFilter;

                case "or":
                    var orFilter = new Core.Filters.OrFilter();
                    foreach (System.Xml.XmlNode childNode in xmlNode.ChildNodes)
                        orFilter.Add(FromXml(childNode));
                    return orFilter;

                case "not":
                    return new NotFilter(FromXml(xmlNode.FirstChild));

                case "tests":
                    var testFilter = new Core.Filters.SimpleNameFilter();
                    var testNodes = xmlNode.SelectNodes("test");
                    if(testNodes != null)
                        foreach (XmlNode childNode in testNodes)
                            testFilter.Add(childNode.InnerText);
                    return testFilter;

                case "cat":
                    var catFilter = new Core.Filters.CategoryFilter();
                    foreach (string cat in xmlNode.InnerText.Split(COMMA))
                        catFilter.AddCategory(cat);
                    return catFilter;

                default:
                    throw new ArgumentException("Invalid filter element: " + xmlNode.Name, "xmlNode");
            }
        }
    }
}
