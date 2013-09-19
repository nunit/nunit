// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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

namespace NUnit.Engine
{
    /// <summary>
    /// NUnitFrameworkDriver is used by the test-runner to load and run
    /// tests using the NUnit framework assembly.
    /// </summary>
    public class NUnitFrameworkDriver : IFrameworkDriver
    {
        private static readonly string CONTROLLER_TYPE = "NUnit.Framework.Api.TestController";
        private static readonly string LOAD_ACTION = CONTROLLER_TYPE + "+LoadTestsAction";
        private static readonly string EXPLORE_ACTION = CONTROLLER_TYPE + "+ExploreTestsAction";
        private static readonly string RUN_ACTION = CONTROLLER_TYPE + "+RunTestsAction";

        AppDomain testDomain;
        string assemblyPath;
        IDictionary<string, object> settings;

        object testController;

        public NUnitFrameworkDriver(AppDomain testDomain, string assemblyPath, IDictionary<string, object> settings)
        {
            this.testDomain = testDomain;
            this.assemblyPath = assemblyPath;
            this.settings = settings;
            this.testController = CreateObject(CONTROLLER_TYPE, assemblyPath, (System.Collections.IDictionary)settings);
        }

        public TestEngineResult Load()
        {
            CallbackHandler handler = new CallbackHandler();

            CreateObject(LOAD_ACTION, testController, handler);

            return handler.Result;
        }

        public void Unload()
        {
        }

        public TestEngineResult Run(ITestEventHandler listener, TestFilter filter)
        {
            CallbackHandler handler = new RunTestsCallbackHandler(listener);

            CreateObject(RUN_ACTION, testController, filter.Text, handler);

            return handler.Result;
        }

        public TestEngineResult Explore(TestFilter filter)
        {
            CallbackHandler handler = new CallbackHandler();

            CreateObject(EXPLORE_ACTION, testController, filter.Text, handler);

            return handler.Result;
        }

        //public TestEngineResult GetLoadedTests()
        //{
        //    CallbackHandler handler = new CallbackHandler();

        //    CreateObject("NUnit.Framework.Api.TestController+GetLoadedTestsAction",
        //        testController, handler.Callback);

        //    return handler.Result;
        //}

        #region Helper Methods

        private object CreateObject(string typeName, params object[] args)
        {
            return this.testDomain.CreateInstanceAndUnwrap(
                "nunit.framework", typeName, false, 0,
#if !NET_4_0
                null, args, null, null, null );
#else
                null, args, null, null );
#endif
        }

        #endregion
    }
}
