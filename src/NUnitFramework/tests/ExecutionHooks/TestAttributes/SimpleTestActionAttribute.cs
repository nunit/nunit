// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Tests.ExecutionHooks.TestAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class SimpleTestActionAttribute : Attribute, ITestAction
    {
        public static readonly string LogStringForBeforeTest = $"{nameof(SimpleTestActionAttribute)}.{nameof(BeforeTest)}";
        public static readonly string LogStringForAfterTest = $"{nameof(SimpleTestActionAttribute)}.{nameof(AfterTest)}";

        public void BeforeTest(ITest test)
        {
            TestLog.LogMessage(LogStringForBeforeTest);
        }

        public void AfterTest(ITest test)
        {
            TestLog.LogMessage(LogStringForAfterTest);
        }

        public ActionTargets Targets => ActionTargets.Suite;
    }
}
