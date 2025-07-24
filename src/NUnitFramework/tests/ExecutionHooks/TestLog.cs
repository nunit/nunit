// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.ExecutionHooks
{
    internal static class TestLog
    {
        internal const string TestLogPropertyKey = "TestLog";

        public static List<string> Logs(ITest? test = null)
        {
            string s = GetPropertyBag(test);
            return s.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
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

        public static string GetPropertyBag(ITest? test = null)
        {
            var props = GetBaseParent(test).Properties;
            return props.Get(TestLogPropertyKey) as string ?? string.Empty;
        }

        public static void LogToPropertyBag(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return;
            }
            var props = GetBaseParent().Properties;
            string currentValue = props.Get(TestLogPropertyKey) as string ?? string.Empty;
            currentValue += s + Environment.NewLine;
            props.Set(TestLogPropertyKey, currentValue);
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
