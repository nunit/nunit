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
using System.Collections;
using System.Xml;

namespace NUnit.AdhocTestRunner
{
    /// <summary>
    /// FrameworkController is used by the test-runner to load and run
    /// tests using the NUnit framework assembly.
    /// </summary>
    public class FrameworkController
    {
        AppDomain testDomain;

        object testController;

        public FrameworkController(AppDomain testDomain)
        {
            this.testDomain = testDomain;
            this.testController = CreateObject("NUnit.Framework.Api.TestController");
        }

        public bool Load(string assemblyFileName, IDictionary options)
        {
            CallbackHandler handler = new CallbackHandler();

            CreateObject("NUnit.Framework.Api.TestController+LoadTestsAction", 
                testController, assemblyFileName, options, handler.Callback);

            Debug.Assert(handler.Result is bool, "Returned result was not a bool");

            return (bool)handler.Result;
        }

        public XmlNode Run()
        {
            CallbackHandler handler = new RunTestsCallbackHandler();

            CreateObject("NUnit.Framework.Api.TestController+RunTestsAction", testController, handler.Callback);

            Debug.Assert(handler.Result is XmlNode, "Returned result was not an XmlNode");

            return handler.Result as XmlNode;
        }

        #region Helper Methods

        private object CreateObject(string typeName, params object[] args)
        {
            return this.testDomain.CreateInstanceAndUnwrap(
                "nunit.framework",
                typeName,
                false,
                0,
                null,
                args,
                null,
                null,
                null);
        }

        #endregion
    }
}
