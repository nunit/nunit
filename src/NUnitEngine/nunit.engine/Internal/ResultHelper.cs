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
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Common;

namespace NUnit.Engine.Internal
{
    /// <summary>
    /// ResultHelper provides static methods for working with
    /// TestEngineResults to wrap, combiner and aggregate them
    /// in various ways.
    /// </summary>
    public static class ResultHelper
    {
        private const string TEST_SUITE_ELEMENT = "test-suite";
        private const string PROJECT_SUITE_TYPE = "Project";

        #region TestEngineResult extension methods

        /// <summary>
        /// Aggregate the XmlNodes under a TestEngineResult into a single XmlNode.
        /// </summary>
        /// <param name="result">A new TestEngineResult with xml nodes for each assembly or project</param>
        /// <returns>A TestEngineResult with a single top-level element.</returns>
        public static TestEngineResult Aggregate(this TestEngineResult result, string elementName, string suiteType, string name, string fullname)
        {
            return new TestEngineResult(Aggregate(elementName, suiteType, name, fullname, result.XmlNodes));
        }

        /// <summary>
        /// Aggregate the XmlNodes under a TestEngineResult into a single XmlNode.
        /// </summary>
        /// <param name="result">A new TestEngineResult with xml nodes for each assembly or project</param>
        /// <returns>A TestEngineResult with a single top-level element.</returns>
        public static TestEngineResult Aggregate(this TestEngineResult result, string elementName, string name, string fullname)
        {
            return new TestEngineResult(Aggregate(elementName, name, fullname, result.XmlNodes));
        }

        /// <summary>
        /// Aggregate all the separate assembly results of a project as a single node.
        /// </summary>
        public static TestEngineResult MakePackageResult(this TestEngineResult result, string name, string fullName)
        {
            return Aggregate(result, TEST_SUITE_ELEMENT, PROJECT_SUITE_TYPE, name, fullName);
        }

        #endregion

        #region Methods that operate on a list of TestEngineResults

        /// <summary>
        /// Merges multiple test engine results into a single result. The
        /// result element contains all the XML nodes found in the input.
        /// </summary>
        /// <param name="results">A list of TestEngineResults</param>
        /// <returns>A TestEngineResult merging all the imput results</returns>
        /// <remarks>Used by AbstractTestRunner MakePackageResult method.</remarks>
        public static TestEngineResult Merge(IList<TestEngineResult> results)
        {
            TestEngineResult mergedResult = new TestEngineResult();

            foreach (TestEngineResult result in results)
                foreach (XmlNode node in result.XmlNodes)
                    mergedResult.Add(node);

            return mergedResult;
        }

        #endregion

        #region XmlNode extension methods

        /// <summary>
        /// Insert an environment element as a child of the node provided.
        /// </summary>
        /// <param name="resultNode"></param>
        public static void InsertEnvironmentElement(this XmlNode resultNode)
        {
            XmlNode env = resultNode.OwnerDocument.CreateElement("environment");
            resultNode.InsertAfter(env, null);
            env.AddAttribute("nunit-version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            env.AddAttribute("clr-version", Environment.Version.ToString());
            env.AddAttribute("os-version", Environment.OSVersion.ToString());
            env.AddAttribute("platform", Environment.OSVersion.Platform.ToString());
            env.AddAttribute("cwd", Environment.CurrentDirectory);
            env.AddAttribute("machine-name", Environment.MachineName);
            env.AddAttribute("user", Environment.UserName);
            env.AddAttribute("user-domain", Environment.UserDomainName);
            env.AddAttribute("culture", System.Globalization.CultureInfo.CurrentCulture.ToString());
            env.AddAttribute("uiculture", System.Globalization.CultureInfo.CurrentUICulture.ToString());
        }

        #endregion

        #region Methods that operate on a list of XmlNodes

        public static XmlNode Aggregate(string elementName, string name, string fullname, IList<XmlNode> resultNodes)
        {
            return Aggregate(elementName, null, name, fullname, resultNodes);
        }

        public static XmlNode Aggregate(string elementName, string testType, string name, string fullname, IList<XmlNode> resultNodes)
        {
            XmlNode combinedNode = XmlHelper.CreateTopLevelElement(elementName);
            if (testType != null)
                combinedNode.AddAttribute("type", testType);
            combinedNode.AddAttribute("id", "2"); // TODO: Should not be hard-coded
            if (name != null && name != string.Empty)
                combinedNode.AddAttribute("name", name);
            if (fullname != null && fullname != string.Empty)
                combinedNode.AddAttribute("fullname", fullname);

            string status = "Inconclusive";
            //double totalDuration = 0.0d;
            int testcasecount = 0;
            int total = 0;
            int passed = 0;
            int failed = 0;
            int inconclusive = 0;
            int skipped = 0;
            int asserts = 0;

            bool isTestRunResult = false;

            foreach (XmlNode node in resultNodes)
            {
                testcasecount += node.GetAttribute("testcasecount", 0);

                XmlAttribute resultAttribute = node.Attributes["result"];
                if (resultAttribute != null)
                {
                    isTestRunResult = true;

                    switch (resultAttribute.Value)
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

                    total += node.GetAttribute("total", 0);
                    passed += node.GetAttribute("passed", 0);
                    failed += node.GetAttribute("failed", 0);
                    inconclusive += node.GetAttribute("inconclusive", 0);
                    skipped += node.GetAttribute("skipped", 0);
                    asserts += node.GetAttribute("asserts", 0);
                }

                XmlNode import = combinedNode.OwnerDocument.ImportNode(node, true);
                combinedNode.AppendChild(import);
            }

            combinedNode.AddAttribute("testcasecount", testcasecount.ToString());

            if (isTestRunResult)
            {
                combinedNode.AddAttribute("result", status);
                //combinedNode.AddAttribute("duration", totalDuration.ToString("0.000000", NumberFormatInfo.InvariantInfo));
                combinedNode.AddAttribute("total", total.ToString());
                combinedNode.AddAttribute("passed", passed.ToString());
                combinedNode.AddAttribute("failed", failed.ToString());
                combinedNode.AddAttribute("inconclusive", inconclusive.ToString());
                combinedNode.AddAttribute("skipped", skipped.ToString());
                combinedNode.AddAttribute("asserts", asserts.ToString());
            }

            return combinedNode;
        }

        #endregion
    }
}
