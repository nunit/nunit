// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

#if (NET20 || NET35 || NET40 || NET45)
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

namespace NUnit.Framework.Api
{
    //https://github.com/nunit/nunit/issues/2614
    class FrameworkControllerCallContextTests
    {
        private object _origExecutionContext;
        private const string MockAssemblyFile = "mock-assembly.dll";

        [SetUp]
        public void SetUp()
        {
            _origExecutionContext = CallContext.GetData(NUnitCallContext.TestExecutionContextKey);
        }

        [TestCaseSource(nameof(FrameworkActions))]
        public void CallContextIsRestoredAroundFrameworkActions(Action frameworkAction)
        {
            //This test only has value if the CallContext of nunit.framework.tests is first cleared. 
            //Otherwise no new call context will be created by the framework action
            CallContext.FreeNamedDataSlot(NUnitCallContext.TestExecutionContextKey);
            frameworkAction();
            var actual = CallContext.GetData(NUnitCallContext.TestExecutionContextKey);
            Assert.That(actual, Is.Null);
        }

        [TearDown]
        public void RestoreCallContext()
        {
            CallContext.SetData(NUnitCallContext.TestExecutionContextKey, _origExecutionContext);
        }

        private static IEnumerable<Action> FrameworkActions
        {
            get
            {
                //RunOnMainThread to ensure we're testing worse-case-scenario. The original issue was found
                //when TestExecutionContext was created in a TestCaseSource,in test exploring code
                //which always runs on the main thread - however, it could be that the entire test
                //run is carried out on the main thread - and we should be protecting against that case too.
                var settings = new Dictionary<string, object>
                {
                    { FrameworkPackageSettings.RunOnMainThread, true }
                };
                var controller = new FrameworkController(MockAssemblyFile, Test.IdPrefix, settings);
                var handler = new CallbackEventHandler();
                yield return () => new FrameworkController.LoadTestsAction(controller, handler);
                yield return () => new FrameworkController.ExploreTestsAction(controller, null, handler);
                yield return () => new FrameworkController.CountTestsAction(controller, null, handler);
                yield return () => new FrameworkController.RunTestsAction(controller, null, handler);
                yield return () => new FrameworkController.RunAsyncAction(controller, null, handler);
            }
        }
    }
}
#endif
