using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    public abstract class AbstractTestRunner :ITestRunner
    {
        protected TestPackage TestPackage;

        #region ITestRunner Members

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public abstract TestEngineResult Load(TestPackage package);

        /// <summary>
        /// Unload any loaded TestPackage. If none is loaded,
        /// the call is ignored. The default implementation
        /// does nothing.
        /// </summary>
        public virtual void Unload()
        {
        }

        /// <summary>
        /// Run the tests in a loaded TestPackage
        /// </summary>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>
        /// A TestEngineResult giving the result of the test execution. The 
        /// top-level node of the result is &lt;direct-runner&gt; and wraps
        /// all the &lt;test-assembly&gt; elements returned by the drivers.
        /// </returns>
        public virtual TestEngineResult Run(ITestEventHandler listener, ITestFilter filter)
        {           
            DateTime startTime = DateTime.Now;
            TestEngineResult[] results = RunDirect(listener, filter);

            return MakeTestRunResult(this.TestPackage, startTime, results);
        }

        /// <summary>
        /// Run the tests in a loaded TestPackage, returning separate results
        /// for each test assembly as an array. This method is primarily intended
        /// to avoid overhead in consolidating test results when one runner calls
        /// a subordinate runner.
        /// </summary>
        /// <param name="listener">An ITestEventHandler to receive events</param>
        /// <param name="filter">A TestFilter used to select tests</param>
        /// <returns>A TestEngineResult[] with separate results for each assembly</returns>
        public abstract TestEngineResult[] RunDirect(ITestEventHandler listener, ITestFilter filter);

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            Unload();
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Make a &lt;test-run&gt; result from a set of subordinate results. If a
        /// subordinate result is itself a &lt;test-run&gt; result, then its child
        /// nodes are used, otherwise the node itself is used.
        /// </summary>
        /// <param name="results">The results to be combined into a &lt;test-run&gt; result.</param>
        /// <returns>A TestEngineResult with a single top-level &lt;test-run&gt; element.</returns>
        public static TestEngineResult MakeTestRunResult(TestPackage package, DateTime startTime, IList<TestEngineResult> results)
        {
            bool isProject = false;
            if (package.HasSubPackages)
            {
                string filePath = package.FilePath;
                isProject = filePath != null && filePath != string.Empty;
            }

            XmlNode resultNode = XmlHelper.CreateTopLevelElement("test-run");

            AddEnvironmentElement(resultNode);

            string status = "Inconclusive";
            double time = 0.0;
            int total = 0;
            int passed = 0;
            int failed = 0;
            int inconclusive = 0;
            int skipped = 0;
            int asserts = 0;

            foreach (TestEngineResult result in results)
            {
                XmlNode assemblyNode = result.Xml;

                switch (XmlHelper.GetAttribute(assemblyNode, "result"))
                {
                    case "Skipped":
                        if (status == "Inconclusive")
                            status = "Skipped";
                        break;
                    case "Passed":
                        if (status != "Failed")
                            status = "Passed";
                        break;
                    case "Failed":
                        status = "Failed";
                        break;
                }

                total += XmlHelper.GetAttribute(assemblyNode, "total", 0);
                time += XmlHelper.GetAttribute(assemblyNode, "time", 0.0);
                passed += XmlHelper.GetAttribute(assemblyNode, "passed", 0);
                failed += XmlHelper.GetAttribute(assemblyNode, "failed", 0);
                inconclusive += XmlHelper.GetAttribute(assemblyNode, "inconclusive", 0);
                skipped += XmlHelper.GetAttribute(assemblyNode, "skipped", 0);
                asserts += XmlHelper.GetAttribute(assemblyNode, "asserts", 0);

                XmlNode import = resultNode.OwnerDocument.ImportNode(assemblyNode, true);
                resultNode.AppendChild(import);
            }

            XmlHelper.AddAttribute(resultNode, "id", "1");
            if (package.Name != null && package.Name != string.Empty)
                XmlHelper.AddAttribute(resultNode, "name", package.Name);
            if (package.FilePath != null && package.FilePath != string.Empty)
                XmlHelper.AddAttribute(resultNode, "fullName", package.FilePath);
            XmlHelper.AddAttribute(resultNode, "result", status);
            XmlHelper.AddAttribute(resultNode, "time", time.ToString());
            XmlHelper.AddAttribute(resultNode, "total", total.ToString());
            XmlHelper.AddAttribute(resultNode, "passed", passed.ToString());
            XmlHelper.AddAttribute(resultNode, "failed", failed.ToString());
            XmlHelper.AddAttribute(resultNode, "inconclusive", inconclusive.ToString());
            XmlHelper.AddAttribute(resultNode, "skipped", skipped.ToString());
            XmlHelper.AddAttribute(resultNode, "asserts", asserts.ToString());

            XmlHelper.AddAttribute(resultNode, "run-date", XmlConvert.ToString(startTime, "yyyy-MM-dd"));
            XmlHelper.AddAttribute(resultNode, "start-time", XmlConvert.ToString(startTime, "HH:mm:ss"));

            return new TestEngineResult(resultNode);
        }

        private static void AddEnvironmentElement(XmlNode resultNode)
        {
            XmlNode env = XmlHelper.AddElement(resultNode, "environment");
            XmlHelper.AddAttribute(env, "nunit-version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            XmlHelper.AddAttribute(env, "clr-version",   Environment.Version.ToString());
            XmlHelper.AddAttribute(env, "os-version",    Environment.OSVersion.ToString());
            XmlHelper.AddAttribute(env, "platform",      Environment.OSVersion.Platform.ToString());
            XmlHelper.AddAttribute(env, "cwd",           Environment.CurrentDirectory);
            XmlHelper.AddAttribute(env, "machine-name",  Environment.MachineName);
            XmlHelper.AddAttribute(env, "user",          Environment.UserName);
            XmlHelper.AddAttribute(env, "user-domain",   Environment.UserDomainName);
            XmlHelper.AddAttribute(env, "culture", System.Globalization.CultureInfo.CurrentCulture.ToString());
            XmlHelper.AddAttribute(env, "uiculture", System.Globalization.CultureInfo.CurrentUICulture.ToString());
        }

        #endregion
    }
}
