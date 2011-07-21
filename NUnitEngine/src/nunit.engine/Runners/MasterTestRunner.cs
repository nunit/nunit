using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    public class MasterTestRunner : ITestRunner
    {
        private TestPackage package;
        private ServiceContext services;
        private AbstractTestRunner realRunner;

        public MasterTestRunner(ServiceContext services)
        {
            this.services = services;
        }

        #region Static Methods

        /// <summary>
        /// Make a &lt;test-run&gt; result from a set of subordinate results. If a
        /// subordinate result is itself a &lt;test-run&gt; result, then its child
        /// nodes are used, otherwise the node itself is used.
        /// </summary>
        /// <param name="results">The results to be combined into a &lt;test-run&gt; result.</param>
        /// <returns>A TestEngineResult with a single top-level &lt;test-run&gt; element.</returns>
        public static TestEngineResult MakeTestRunResult(TestPackage package, DateTime startTime, TestEngineResult result)
        {
            XmlNode combinedNode = TestEngineResult.Aggregate("test-run", package, result.XmlNodes);
            InsertEnvironmentElement(combinedNode);

            //if (result.Xml.Name == "test-wrapper")
            //    foreach (XmlNode child in result.Xml.ChildNodes)
            //        assemblyNodes.Add(child);
            //else
            //    assemblyNodes.Add(result.Xml);

            XmlHelper.AddAttribute(combinedNode, "run-date", XmlConvert.ToString(startTime, "yyyy-MM-dd"));
            XmlHelper.AddAttribute(combinedNode, "start-time", XmlConvert.ToString(startTime, "HH:mm:ss"));

            return new TestEngineResult(combinedNode);
        }

        private static void InsertEnvironmentElement(XmlNode resultNode)
        {
            XmlNode env = resultNode.OwnerDocument.CreateElement("environment");
            resultNode.InsertAfter(env, null);
            XmlHelper.AddAttribute(env, "nunit-version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            XmlHelper.AddAttribute(env, "clr-version", Environment.Version.ToString());
            XmlHelper.AddAttribute(env, "os-version", Environment.OSVersion.ToString());
            XmlHelper.AddAttribute(env, "platform", Environment.OSVersion.Platform.ToString());
            XmlHelper.AddAttribute(env, "cwd", Environment.CurrentDirectory);
            XmlHelper.AddAttribute(env, "machine-name", Environment.MachineName);
            XmlHelper.AddAttribute(env, "user", Environment.UserName);
            XmlHelper.AddAttribute(env, "user-domain", Environment.UserDomainName);
            XmlHelper.AddAttribute(env, "culture", System.Globalization.CultureInfo.CurrentCulture.ToString());
            XmlHelper.AddAttribute(env, "uiculture", System.Globalization.CultureInfo.CurrentUICulture.ToString());
        }

        #endregion

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public ITestEngineResult Load(TestPackage package)
        {
            this.package = package;

            int projectCount = 0;
            int assemblyCount = 0;

            if (package.TestFiles.Length > 0)
            {
                foreach (string testFile in package.TestFiles)
                {
                    TestPackage subPackage = new TestPackage(testFile);
                    if (services.ProjectService.IsProjectFile(testFile))
                    {
                        services.ProjectService.ExpandProjectPackage(subPackage);
                        projectCount++;
                    }
                    else
                        assemblyCount++;
                }
            }
            else
            {
                if (services.ProjectService.IsProjectFile(package.FullName))
                {
                    services.ProjectService.ExpandProjectPackage(package);
                    projectCount++;
                }
                else
                    assemblyCount++;
            }

            if (projectCount > 1 || projectCount > 0 && assemblyCount > 0)
                this.realRunner = new AggregatingTestRunner(services);
            else
                this.realRunner = (AbstractTestRunner)services.TestRunnerFactory.MakeTestRunner(package);

            return this.realRunner.Load(package);
        }

        /// <summary>
        /// Unload any loaded TestPackage.
        /// </summary>
        public void Unload()
        {
            if (this.realRunner != null)
                this.realRunner.Unload();
        }

        public ITestEngineResult Run(ITestEventHandler listener, ITestFilter filter)
        {
            DateTime startTime = DateTime.Now;

            TestEngineResult result = realRunner.Run(listener, filter);

            return MakeTestRunResult(this.package, startTime, result);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.realRunner != null)
                this.realRunner.Dispose();
        }

        #endregion
    }
}
