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
using System.IO;
using System.Collections;
using NUnit.Core;

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

        public FrameworkController(CommandLineOptions options)
        {
            this.testDomain = options.UseAppDomain
                ? CreateDomain(Path.GetDirectoryName(Path.GetFullPath(options.Parameters[0])))
                : AppDomain.CurrentDomain;

            this.testController = CreateObject("NUnit.Framework.Api.TestController");
        }

        public bool Load(string assemblyFileName)
        {
            CallbackHandler handler = new CallbackHandler();

            CreateObject("NUnit.Framework.Api.TestController+LoadTestsAction", 
                testController, assemblyFileName, handler.Callback);

            return (bool)handler.Result;
        }

        public TestResult Run(ITestListener listener, TestFilter filter)
        {
            CallbackHandler handler = new CallbackHandler();

            CreateObject("NUnit.Framework.Api.TestController+RunTestsAction", testController, handler.Callback);

            return (TestResult)handler.Result;
        }

        #region CallbackHandler class

        private class CallbackHandler : MarshalByRefObject
        {
            private object result;
            private TextWriter output;

            public CallbackHandler()
            {
                this.output = Console.Out;
            }

            public object Result
            {
                get { return result; }
            }

            public AsyncCallback Callback
            {
                get { return new AsyncCallback(CallbackMethod); }
            }

            private void CallbackMethod(IAsyncResult ar)
            {
                object state = ar.AsyncState;

                if (ar.IsCompleted)
                    this.result = state;
                else if (state is TestOutput)
                    output.Write(((TestOutput)state).Text);
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }
        }

        #endregion

        #region Helper Methods

        private static AppDomain CreateDomain(string appBase)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = appBase;
            AppDomain domain = AppDomain.CreateDomain("test-domain", null, setup);
            return domain;
        }

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
