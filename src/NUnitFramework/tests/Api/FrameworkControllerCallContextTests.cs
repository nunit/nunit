// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK
using System.Runtime.Remoting.Messaging;

namespace NUnit.Framework.Api
{
    // https://github.com/nunit/nunit/issues/2614
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
