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
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NUnit.Util.Tests
{
    using Engine;
    using Engine.Internal;
    using Framework;

    using Runner = NUnit.Framework.Api.DefaultTestAssemblyRunner;
    using Builder = NUnit.Framework.Api.DefaultTestAssemblyBuilder;
    using TestListener = NUnit.Framework.Internal.TestListener;
    using TestFilter = NUnit.Framework.Internal.TestFilter;

    /// <summary>
    /// This is the abstract base for all XML output tests,
    /// which need to work on a TestEngineResult. Creating a 
    /// second level engine in the test domain causes
    /// problems, so this class uses internal framework
    /// classes to run the test and then transforms the XML
    /// result into a TestEngineResult for use by derived tests.
    /// </summary>
    public abstract class XmlOutputTest
    {
        private ITestEngine engine;
        private string localDirectory;

        protected TestEngineResult EngineResult { get; private set; }

        // Method used by deribed classes to get the path to a file name
        protected string GetLocalPath(string fileName)
        {
            return Path.Combine(localDirectory, fileName);
        }

        [OneTimeSetUp]
        public void InitializeTestEngineResult()
        {
            // Save the local directory - used by GetLocalPath
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            localDirectory = Path.GetDirectoryName(uri.LocalPath);

            // Create a fresh copy of the engine, since we can't use the
            // one that is running this test.
            engine = TestEngineActivator.CreateInstance();
            engine.InternalTraceLevel = InternalTraceLevel.Off;

            // Create a new DefaultAssemblyRunner, which is actually a framework class,
            // because we can't use the one that's currently running this test.
            var runner = new Runner(new Builder());
            var assemblyPath = GetLocalPath("mock-assembly.dll");
            var settings = new Dictionary<string, object>();

            // Make sure the runner loaded the mock assembly.
            Assert.That(
                runner.Load(assemblyPath, settings).RunState.ToString(),
                Is.EqualTo("Runnable"), 
                "Unable to load mock-assembly.dll");

            // Run the tests, saving the result as an XML string
            var xmlText = runner.Run(TestListener.NULL, TestFilter.Empty).ToXml(true).OuterXml;

            // Create a TestEngineResult from the string, just as the TestEngine does,
            // then add a test-run element to the result, wrapping the result so it
            // looks just like what the engine would return!
            this.EngineResult = new TestEngineResult(xmlText).Aggregate("test-run", "NAME", "FULLNAME");
        }
    }
}
