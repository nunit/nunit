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
using System.Collections.Generic;
using System.Xml;

namespace NUnit.Framework.TestHarness
{
    /// <summary>
    /// FrameworkDriver is used by the test-runner to load and run
    /// tests using the NUnit framework assembly. It's a simplified
    /// version of the FrameworkDriver found in the NUnit engine.
    /// </summary>
    public class FrameworkDriver
    {
        private static readonly string CONTROLLER_TYPE = "NUnit.Framework.Api.FrameworkController";
        private static readonly string LOAD_ACTION = CONTROLLER_TYPE + "+LoadTestsAction";
        private static readonly string EXPLORE_ACTION = CONTROLLER_TYPE + "+ExploreTestsAction";
        private static readonly string RUN_ACTION = CONTROLLER_TYPE + "+RunTestsAction";

        AppDomain testDomain;
        string assemblyPath;
        IDictionary<string, object> settings;

        object testController;

        /// <summary>
        /// Construct a FrameworkDriver for a particular assembly in a domain,
        /// and associate some settings with it. The assembly must reference
        /// the NUnit framework so that we can remotely create the FrameworkController.
        /// </summary>
        /// <param name="assemblyPath">The path to the test assembly</param>
        /// <param name="testDomain">The domain in which the assembly will be loaded</param>
        /// <param name="settings">A dictionary of load and run settings</param>
        public FrameworkDriver(string assemblyPath, AppDomain testDomain, IDictionary<string, object> settings)
        {
            this.testDomain = testDomain;
            this.assemblyPath = assemblyPath;
            this.settings = settings;
            this.testController = CreateObject(CONTROLLER_TYPE, assemblyPath, settings);
        }

        public XmlNode Load()
        {
            CallbackHandler handler = new CallbackHandler();

            CreateObject(LOAD_ACTION, testController, handler);

            return MakeXmlNode(handler.Result);
        }

        public XmlNode ExploreTests(string filter)
        {
            CallbackHandler handler = new CallbackHandler();

            CreateObject(EXPLORE_ACTION, testController, filter, handler);

            return MakeXmlNode(handler.Result);
        }

        public XmlNode Run(string filter)
        {
            CallbackHandler handler = new RunTestsCallbackHandler(settings);

            CreateObject(RUN_ACTION, testController, filter, handler);

            return MakeXmlNode(handler.Result);
        }

        #region Helper Methods

        private object CreateObject(string typeName, params object[] args)
        {
            return this.testDomain.CreateInstanceAndUnwrap(
                "nunit.framework", typeName, false, 0,
#if !NET_4_0 && !NET_4_5
                null, args, null, null, null );
#else
                null, args, null, null );
#endif
        }

        private XmlNode MakeXmlNode(string result)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            return doc.FirstChild;
        }

        #endregion
    }
}
