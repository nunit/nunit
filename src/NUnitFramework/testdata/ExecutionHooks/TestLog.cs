// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.TestData.ExecutionHooks
{
    public static class TestLog
    {
        internal const string TestLogPropertyKey = "TestLog";

        public static IEnumerable Logs(ITest? test = null)
        {
            var props = GetBaseParent(test).Properties;
            if (!props.TryGet(TestLogPropertyKey, out IList? value))
            {
                return Enumerable.Empty<string>();
            }
            return value;
        }

        public static void LogCurrentMethod([CallerMemberName] string callerMethodName = "")
        {
            LogToPropertyBag(callerMethodName);
        }

        public static void LogCurrentMethodWithContextInfo(string contextInfo, [CallerMemberName] string callerMethodName = "")
        {
            LogToPropertyBag($"{callerMethodName}({contextInfo})");
        }

        public static void LogMessage(string message)
        {
            LogToPropertyBag(message);
        }

        public static void LogToPropertyBag(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return;
            }
            var props = GetBaseParent().Properties;
            props.Add(TestLogPropertyKey, s);
        }

        public static ITest GetBaseParent(ITest? test = null)
        {
            ITest current = test is null ? TestExecutionContext.CurrentContext.CurrentTest : test;
            while (current.Parent is not null)
            {
                current = current.Parent;
            }
            return current;
        }
    }
}
