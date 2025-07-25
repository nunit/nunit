using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    internal sealed class LogTestActionAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            TestLog.LogCurrentMethodWithContextInfo(test.IsSuite ? "Suite" : "Test");
        }

        public void AfterTest(ITest test)
        {
            TestLog.LogCurrentMethodWithContextInfo(test.IsSuite ? "Suite" : "Test");
        }

        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;
    }
}
