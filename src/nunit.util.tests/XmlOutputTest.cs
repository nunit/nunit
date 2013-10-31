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
using NUnit.Engine;
using NUnit.Framework;

using Runner = NUnit.Framework.Internal.DefaultTestAssemblyRunner;
using Builder = NUnit.Framework.Internal.DefaultTestAssemblyBuilder;
using TestListener = NUnit.Framework.Internal.TestListener;
using TestFilter = NUnit.Framework.Internal.TestFilter;

namespace NUnit.Util.Tests
{
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

        protected string GetLocalPath(string fileName)
        {
            return Path.Combine(localDirectory, fileName);
        }

        [TestFixtureSetUp]
        public void InitializeTestEngineResult()
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            localDirectory = Path.GetDirectoryName(uri.LocalPath);
            engine = TestEngineActivator.CreateInstance(null, InternalTraceLevel.Off);

            var assemblyPath = GetLocalPath("mock-assembly.dll");
            var settings = new Dictionary<string, object>();

            var runner = new Runner(new Builder());
            Assert.True(runner.Load(assemblyPath, settings), "Unable to load mock-assembly.dll");

            // Convert our own framework XmlNode to a TestEngineResult
            var package = new TestPackage(assemblyPath);
            this.EngineResult = TestEngineResult.MakeTestRunResult(
                package,
                DateTime.Now,
                new TestEngineResult(
                    runner.Run(TestListener.NULL, TestFilter.Empty).ToXml(true).OuterXml));
        }
    }
}
