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
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace NUnit.Engine.Api
{
    public class DirectTestRunner : ITestRunner
    {
        private TestPackage package;
        private AppDomain testDomain;
        private List<FrameworkDriver> drivers = new List<FrameworkDriver>();

        public DirectTestRunner() : this(AppDomain.CurrentDomain) { }

        public DirectTestRunner(AppDomain testDomain)
        {
            this.testDomain = testDomain;
        }

        #region ITestRunner Members

        public bool Load(TestPackage package)
        {
            this.package = package;
            int count = 0;

            foreach (string testFile in package.TestFiles)
            {
                var driver = new FrameworkDriver(testDomain);
                if (driver.Load(testFile, new Hashtable()))
                {
                    drivers.Add(driver);
                    count++;
                }
            }

            return count == package.TestFiles.Count;
        }

        public XmlNode Run(ITestFilter filter)
        {
            var results = new List<XmlNode>();

            foreach (FrameworkDriver driver in drivers)
                results.Add(driver.Run(new Hashtable()));

            switch (results.Count)
            {
                case 0:
                    return null;
                case 1:
                    return results[0];
                default:
                    return CombineResults(results);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region Helper Methods

        private XmlNode CombineResults(IList<XmlNode> results)
        {
            var combined = XmlHelper.CreateTopLevelElement("test-suite");

            string status = "Inconclusive";
            double time = 0.0;
            int total = 0;
            int passed = 0;
            int failed = 0;
            int inconclusive = 0;
            int skipped = 0;
            int asserts = 0;

            foreach (XmlNode result in results)
            {
                switch (XmlHelper.GetAttribute(result, "result"))
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

                total += XmlHelper.GetAttribute(result, "total", 0);
                time += XmlHelper.GetAttribute(result, "time", 0.0);
                passed += XmlHelper.GetAttribute(result, "passed", 0);
                failed += XmlHelper.GetAttribute(result, "failed", 0);
                inconclusive += XmlHelper.GetAttribute(result, "inconclusive", 0);
                skipped += XmlHelper.GetAttribute(result, "skipped", 0);
                asserts += XmlHelper.GetAttribute(result, "asserts", 0);

                var import = combined.OwnerDocument.ImportNode(result, true);
                combined.AppendChild(import);
            }

            XmlHelper.AddAttribute(combined, "type", "test-package");
            XmlHelper.AddAttribute(combined, "id", "1000");
            //XmlHelper.AddAttribute(combined, "name", package.Name);
            //XmlHelper.AddAttribute(combined, "fullName", package.FullName);
            XmlHelper.AddAttribute(combined, "result", status);
            XmlHelper.AddAttribute(combined, "time", time.ToString());
            XmlHelper.AddAttribute(combined, "total", total.ToString());
            XmlHelper.AddAttribute(combined, "passed", passed.ToString());
            XmlHelper.AddAttribute(combined, "failed", failed.ToString());
            XmlHelper.AddAttribute(combined, "inconclusive", inconclusive.ToString());
            XmlHelper.AddAttribute(combined, "skipped", skipped.ToString());
            XmlHelper.AddAttribute(combined, "asserts", asserts.ToString());


            return combined;
        }

        #endregion
    }
}
