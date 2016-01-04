// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using System.Linq;
using System.Collections.Generic;
using NUnit.Engine.Internal;
using System.Runtime.Serialization;
using System.Reflection;

namespace NUnit.Engine
{
    /// <summary>
    /// NUnit3PortableDriver is used by the test-runner to load and run
    /// tests using the NUnit framework assembly.
    /// </summary>
    public class NUnit3PortableDriver
    {
        private const string LOAD_MESSAGE = "Method called without calling Load first";

        private static readonly string CONTROLLER_TYPE = "NUnit.Framework.Api.FrameworkController";
        private static readonly string LOAD_ACTION = CONTROLLER_TYPE + "+LoadTestsAction";
        private static readonly string EXPLORE_ACTION = CONTROLLER_TYPE + "+ExploreTestsAction";
        private static readonly string COUNT_ACTION = CONTROLLER_TYPE + "+CountTestsAction";
        private static readonly string RUN_ACTION = CONTROLLER_TYPE + "+RunTestsAction";
        private static readonly string STOP_RUN_ACTION = CONTROLLER_TYPE + "+StopRunAction";

        static ILogger log = InternalTrace.GetLogger("NUnit3PortableDriver");

        Assembly _testAssembly;
        Assembly _frameworkAssembly;
        object _frameworkController;

        public string ID { get; set; }

        /// <summary>
        /// Loads the tests in an assembly.
        /// </summary>
        /// <param name="frameworkAssembly">The NUnit Framework that the tests reference</param>
        /// <param name="testAssembly">The test assembly</param>
        /// <param name="settings">The test settings</param>
        /// <returns>An Xml string representing the loaded test</returns>
        public string Load(Assembly frameworkAssembly, Assembly testAssembly, IDictionary<string, object> settings)
        {
            var idPrefix = string.IsNullOrEmpty(ID) ? "" : ID + "-";
            _frameworkAssembly = frameworkAssembly;
            _testAssembly = testAssembly;

            _frameworkController = CreateObject(CONTROLLER_TYPE, testAssembly, idPrefix, (System.Collections.IDictionary)settings);

            CallbackHandler handler = new CallbackHandler();

            log.Info("Loading {0} - see separate log file", _testAssembly.FullName);
            CreateObject(LOAD_ACTION, _frameworkController, handler);

            return handler.Result;
        }

        /// <summary>
        /// Counts the number of test cases for the loaded test assembly
        /// </summary>
        /// <param name="filter">The XML test filter</param>
        /// <returns>The number of test cases</returns>
        public int CountTestCases(string filter)
        {
            CheckLoadWasCalled();

            CallbackHandler handler = new CallbackHandler();

            CreateObject(COUNT_ACTION, _frameworkController, filter, handler);

            return int.Parse(handler.Result);
        }

        /// <summary>
        /// Executes the tests in an assembly.
        /// </summary>
        /// <param name="listener">An ITestEventHandler that receives progress notices</param>
        /// <param name="filter">A filter that controls which tests are executed</param>
        /// <returns>An Xml string representing the result</returns>
        public string Run(ITestEventListener listener, string filter)
        {
            CheckLoadWasCalled();

            CallbackHandler handler = new RunTestsCallbackHandler(listener);

            log.Info("Running {0} - see separate log file", _testAssembly.FullName);
            CreateObject(RUN_ACTION, _frameworkController, filter, handler);

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
        public string Explore(string filter)
        {
            CheckLoadWasCalled();

            CallbackHandler handler = new CallbackHandler();

            log.Info("Exploring {0} - see separate log file", _testAssembly.FullName);
            CreateObject(EXPLORE_ACTION, _frameworkController, filter, handler);

            return handler.Result;
        }

        #region Helper Methods

        private void CheckLoadWasCalled()
        {
            if (_frameworkController == null)
                throw new InvalidOperationException(LOAD_MESSAGE);
        }

        private object CreateObject(string typeName, params object[] args)
        {
            var typeinfo = _frameworkAssembly.DefinedTypes.FirstOrDefault(t => t.FullName == typeName);
            if (typeinfo == null)
            {
                log.Error("Could not find type {0}", typeName);
            }
            return Activator.CreateInstance(typeinfo.AsType(), args);
        }

        #endregion
    }
}
