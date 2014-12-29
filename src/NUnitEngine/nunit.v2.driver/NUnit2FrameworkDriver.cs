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
using System.Text;
using System.Xml;
using NUnit.Core;

using TestPackage = NUnit.Core.TestPackage;

namespace NUnit.Engine.Drivers
{
    public class NUnit2FrameworkDriver : IFrameworkDriver
    {
        private const string NUNIT_FRAMEWORK = "nunit.framework";

        // TODO: The id should not be hard-coded
        private const string LOAD_RESULT_FORMAT =
            "<test-suite type='Assembly' id='2' name='{0}' fullname='{1}' testcasecount='0' runstate='NotRunnable'>" +
                "<properties>" +
                    "<property name='_SKIPREASON' value='{2}'/>" +
                "</properties>" +
            "</test-suite>";

        private const string RUN_RESULT_FORMAT =
            "<test-suite type='Assembly' id='2' name='{0}' fullname='{1}' testcasecount='0' runstate='NotRunnable' result='Skipped' label='NotRunnable'>" +
                "<properties>" +
                    "<property name='_SKIPREASON' value='{2}'/>" +
                "</properties>" +
                "<reason>" +
                    "<message>{2}</message>" +
                "</reason>" +
            "</test-suite>";

        AppDomain _testDomain;
        string _testAssemblyPath;
        string _frameworkAssemblyName;

        string _name;
        string _fullname;

        TestRunner _runner;
        Core.TestPackage _package;

        public NUnit2FrameworkDriver(AppDomain testDomain, string testAssemblyPath, IDictionary<string, object> settings)
            : this(testDomain, NUNIT_FRAMEWORK, testAssemblyPath, settings) { }

        public NUnit2FrameworkDriver(AppDomain testDomain, string frameworkAssemblyName, string testAssemblyPath, IDictionary<string, object> settings)
        {
            if (!File.Exists(testAssemblyPath))
                throw new ArgumentException("testAssemblyPath", "Framework driver constructor called with a file name that doesn't exist.");

            _testDomain = testDomain;
            _testAssemblyPath = testAssemblyPath;
            _frameworkAssemblyName = frameworkAssemblyName;

            _name = Escape(Path.GetFileName(_testAssemblyPath));
            _fullname = Escape(_testAssemblyPath);

            var initializer = DomainInitializer.CreateInstance(_testDomain);
            initializer.InitializeDomain((int)InternalTrace.Level);

            _runner = RemoteTestRunner.CreateInstance(_testDomain, 1);

            _package = new Core.TestPackage(_frameworkAssemblyName);
            foreach (var key in settings.Keys)
                _package.Settings[key] = settings[key];

        }

        public string Load()
        {
            if (!_runner.Load(new Core.TestPackage(_testAssemblyPath)))
                return string.Format(LOAD_RESULT_FORMAT, _name, _fullname, "No tests were found");

            Core.ITest test = _runner.Test;
            // TODO: Handle error where test is null

            return test.ToXml(false).OuterXml;
        }

        public int CountTestCases(TestFilter filter)
        {
            return 0;
        }

        public string Run(ITestEventListener listener, TestFilter filter)
        {
            if (_runner.Test == null)
                return String.Format(LOAD_RESULT_FORMAT, _name, _fullname, "Error loading test");

            var result = _runner.Run(Core.NullListener.NULL, Core.TestFilter.Empty, false, LoggingThreshold.Off);

            return result.ToXml(true).OuterXml;
        }

        public string Explore(TestFilter filter)
        {
            if (_runner.Test == null)
                return String.Format(LOAD_RESULT_FORMAT, _name, _fullname, "Error loading test");

            return _runner.Test.ToXml(true).OuterXml;
        }

        public void StopRun(bool force)
        {
            throw new NotImplementedException();
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
    }
}
