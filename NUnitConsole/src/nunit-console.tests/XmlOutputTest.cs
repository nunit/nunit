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

namespace NUnit.ConsoleRunner.Tests
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
