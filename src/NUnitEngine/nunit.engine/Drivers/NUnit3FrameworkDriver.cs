// ***********************************************************************
// Copyright (c) 2009-2014 Charlie Poole
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
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Web.UI;
using System.Xml;
using NUnit.Engine.Internal;

namespace NUnit.Engine.Drivers
{
    /// <summary>
    /// NUnitFrameworkDriver is used by the test-runner to load and run
    /// tests using the NUnit framework assembly.
    /// </summary>
    public class NUnit3FrameworkDriver : IFrameworkDriver
    {
        private const string NUNIT_FRAMEWORK = "nunit.framework";

        private static readonly string CONTROLLER_TYPE = "NUnit.Framework.Api.FrameworkController";
        private static readonly string LOAD_ACTION = CONTROLLER_TYPE + "+LoadTestsAction";
        private static readonly string EXPLORE_ACTION = CONTROLLER_TYPE + "+ExploreTestsAction";
        private static readonly string COUNT_ACTION = CONTROLLER_TYPE + "+CountTestsAction";
        private static readonly string RUN_ACTION = CONTROLLER_TYPE + "+RunTestsAction";
        private static readonly string STOP_RUN_ACTION = CONTROLLER_TYPE + "+StopRunAction";

        static ILogger log = InternalTrace.GetLogger("NUnitFrameworkDriver");

        AppDomain _testDomain;
        string _testAssemblyPath;
        string _frameworkAssemblyName;

        object _frameworkController;

        public NUnit3FrameworkDriver(AppDomain testDomain, string testAssemblyPath, IDictionary<string, object> settings)
            : this(testDomain, NUNIT_FRAMEWORK, testAssemblyPath, settings) { }

        public NUnit3FrameworkDriver(AppDomain testDomain, string frameworkAssemblyName, string testAssemblyPath, IDictionary<string, object> settings)
        {
            Guard.ArgumentValid(File.Exists(testAssemblyPath), "testAssemblyPath", "Framework driver constructor called with a file name that doesn't exist.");

            _testDomain = testDomain;
            _testAssemblyPath = testAssemblyPath;
            _frameworkAssemblyName = frameworkAssemblyName;
            _frameworkController = CreateObject(CONTROLLER_TYPE, testAssemblyPath, (System.Collections.IDictionary)settings);
        }

        /// <summary>
        /// Loads the tests in an assembly.
        /// </summary>
        /// <returns>An Xml string representing the loaded test</returns>
        public string Load()
        {
            CallbackHandler handler = new CallbackHandler();

            log.Info("Loading {0} - see separate log file", Path.GetFileName(_testAssemblyPath));
            CreateObject(LOAD_ACTION, _frameworkController, handler);

            return handler.Result;
        }

        public int CountTestCases(TestFilter filter)
        {
            CallbackHandler handler = new CallbackHandler();

            CreateObject(COUNT_ACTION, _frameworkController, filter.Text, handler);

            return int.Parse(handler.Result);
        }

        /// <summary>
        /// Executes the tests in an assembly.
        /// </summary>
        /// <param name="listener">An ITestEventHandler that receives progress notices</param>
        /// <param name="filter">A filter that controls which tests are executed</param>
        /// <returns>An Xml string representing the result</returns>
        public string Run(ITestEventListener listener, TestFilter filter)
        {
            CallbackHandler handler = new RunTestsCallbackHandler(listener);

            log.Info("Running {0} - see separate log file", Path.GetFileName(_testAssemblyPath));
            CreateObject(RUN_ACTION, _frameworkController, filter.Text, handler);

            return handler.Result;
        }

        /// <summary>
        /// Cancel the ongoing test run. If no  test is running, the call is ignored.
        /// </summary>
        /// <param name="force">If true, cancel any ongoing test threads, otherwise wait for them to complete.</param>
        public void StopRun(bool force)
        {
            CreateObject(STOP_RUN_ACTION, _frameworkController, force);
        }

        /// <summary>
        /// Returns information about the tests in an assembly.
        /// </summary>
        /// <param name="filter">A filter indicating which tests to include</param>
        /// <returns>An Xml string representing the tests</returns>
        public string Explore(TestFilter filter)
        {
            CallbackHandler handler = new CallbackHandler();

            log.Info("Exploring {0} - see separate log file", Path.GetFileName(_testAssemblyPath));
            CreateObject(EXPLORE_ACTION, _frameworkController, filter.Text, handler);

            return handler.Result;
        }

        #region Helper Methods

        private object CreateObject(string typeName, params object[] args)
        {
            return _testDomain.CreateInstanceAndUnwrap(
                _frameworkAssemblyName, typeName, false, 0,
#if !NET_4_0
                null, args, null, null, null );
#else
                null, args, null, null );
#endif
        }

        #endregion
    }
}
