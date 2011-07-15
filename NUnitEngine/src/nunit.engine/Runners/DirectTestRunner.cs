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
using System.Text;
using System.Xml;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Runners
{
    /// <summary>
    /// DirectTestRunner is the abstract base for runners 
    /// that deal directly with a framework driver.
    /// </summary>
    public abstract class DirectTestRunner : AbstractTestRunner
    {
        private List<IFrameworkDriver> drivers = new List<IFrameworkDriver>();
        private ServiceContext services;

        protected AppDomain TestDomain;

        protected ServiceContext Services
        {
            get { return services; }
        }

        public DirectTestRunner(ServiceContext services)
        {
            this.services = services;
        }

        #region AbstractTestRunner Overrides

        /// <summary>
        /// Load a TestPackage for possible execution
        /// </summary>
        /// <param name="package">The TestPackage to be loaded</param>
        /// <returns>A TestEngineResult.</returns>
        public override TestEngineResult Load(TestPackage package)
        {
            this.TestPackage = package;
            var loadResults = new List<string>();

            foreach (string testFile in package.GetAssemblies())
            {
                // TODO: Should get the appropriate driver for the file
                IFrameworkDriver driver = new NUnitFrameworkDriver(TestDomain);
                var loadResult = driver.Load(testFile, package.Settings);

                loadResults.Add(loadResult.Text);

                if (!loadResult.IsError)
                    drivers.Add(driver);
            }

            string element = drivers.Count == loadResults.Count
                ? "load"
                : "error";

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<{0}>", element);
            foreach (string result in loadResults)
                sb.Append(result);
            sb.AppendFormat("</{0}>", element);

            return new TestEngineResult(sb.ToString());
        }

        public override TestEngineResult[] RunDirect(ITestEventHandler listener, ITestFilter filter)
        {
            List<TestEngineResult> results = new List<TestEngineResult>();

            foreach (NUnitFrameworkDriver driver in drivers)
                results.Add(driver.Run(this.TestPackage.Settings, listener));

            return results.ToArray();
        }

        #endregion
    }
}
