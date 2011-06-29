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
#if CLR_2_0 || CLR_4_0
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Xml;
using NUnit.Engine.Interfaces;

namespace NUnit.Engine
{
    /// <summary>
    /// NUnitFrameworkDriver is used by the test-runner to load and run
    /// tests using the NUnit framework assembly.
    /// </summary>
    public class NUnitFrameworkDriver : IFrameworkDriver
    {
        AppDomain testDomain;

        object testController;

        public NUnitFrameworkDriver(AppDomain testDomain)
        {
            this.testDomain = testDomain;
            this.testController = CreateObject("NUnit.Framework.Api.TestController");
        }

#if CLR_2_0 || CLR_4_0
        public bool Load(string assemblyFileName, IDictionary<string,object> options)
#else
        public bool Load(string assemblyFileName, IDictionary options)
#endif
        {
            CallbackHandler handler = new CallbackHandler();

            CreateObject("NUnit.Framework.Api.TestController+LoadTestsAction",
                testController, assemblyFileName, options, handler.Callback);

            Debug.Assert(handler.Result is bool, "Returned result was not a bool");

            return (bool)handler.Result;
        }

        public void Unload()
        {
        }

#if CLR_2_0 || CLR_4_0
        public XmlNode ExploreTests(string assemblyFileName, IDictionary<string,object> options)
#else
        public XmlNode ExploreTests(string assemblyFileName, IDictionary options)
#endif
        {
            CallbackHandler handler = new CallbackHandler();

            CreateObject("NUnit.Framework.Api.TestController+ExploreTestsAction",
                testController, assemblyFileName, options, handler.Callback);

 			XmlDocument doc = new XmlDocument();
			doc.LoadXml((string)handler.Result);
            return doc.FirstChild;
        }

        public XmlNode GetLoadedTests()
        {
            CallbackHandler handler = new CallbackHandler();

            CreateObject("NUnit.Framework.Api.TestController+GetLoadedTestsAction",
                testController, handler.Callback);

            Debug.Assert(handler.Result == null || handler.Result is string,
                "Returned result was not an XmlNode");

			XmlDocument doc = new XmlDocument();
			doc.LoadXml((string)handler.Result);
            return doc.FirstChild;
        }

#if CLR_2_0 || CLR_4_0
        public TestResult Run(IDictionary<string,object> runOptions)
#else
        public TestResult Run(IDictionary runOptions)
#endif
        {
            CallbackHandler handler = new RunTestsCallbackHandler();

            CreateObject("NUnit.Framework.Api.TestController+RunTestsAction", testController, runOptions, handler.Callback);

            Debug.Assert(handler.Result is string, "Returned result was not a string");

            return new TestResult((string)handler.Result);
        }

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
