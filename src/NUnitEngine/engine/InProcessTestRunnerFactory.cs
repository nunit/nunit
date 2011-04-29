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
using System.IO;
using NUnit.Engine.Api;

namespace NUnit.Engine
{
    /// <summary>
    /// InProcessTestRunnerFactory handles creation of a suitable test 
    /// runner for a given package to be loaded and run within the
    /// same process.
    /// </summary>
    public class InProcessTestRunnerFactory : ITestRunnerFactory
    {
        #region ITestRunnerFactory Members

        /// <summary>
        /// Returns a test runner based on the settings in a TestPackage.
        /// Any setting that is "consumed" by the factory is removed, so
        /// that downstream runners using the factory will not repeatedly
        /// create the same type of runner.
        /// </summary>
        /// <param name="package">The TestPackage to be loaded and run</param>
        /// <returns>A TestRunner</returns>
        public virtual ITestRunner MakeTestRunner(TestPackage package)
        {
            DomainUsage domainUsage = 
                (DomainUsage)package.GetSetting("DomainUsage", DomainUsage.Default);

            AppDomain testDomain = AppDomain.CurrentDomain;

            switch (domainUsage)
            {
                //case DomainUsage.Multiple:
                //    package.Settings.Remove("DomainUsage");
                //    return new MultipleTestDomainRunner();
                case DomainUsage.None:
                    return new DirectTestRunner();
                case DomainUsage.Single:
                default:
                    testDomain = AppDomain.CreateDomain(Path.GetDirectoryName(Path.GetFullPath(package.TestFiles[0])));
                    return new DirectTestRunner(testDomain);
            }
        }

        public virtual bool CanReuse(ITestRunner runner, TestPackage package)
        {
            return false;
        }

        #endregion
    }
}
