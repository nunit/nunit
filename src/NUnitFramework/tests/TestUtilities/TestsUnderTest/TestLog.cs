// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NUnit.Framework.Tests.TestUtilities.TestsUnderTest
{
    public static class TestLog
    {
        public const string TestLogContextIdParameterName = "TestLogContextId";
        private static readonly ConcurrentDictionary<string, List<string>> ConcurrentLogs = new ConcurrentDictionary<string, List<string>>();
        private static readonly object Lock = new object();
        private static readonly ThreadLocal<string> ContextId = new ThreadLocal<string>(() =>
        {
            if (TestContext.Parameters.Exists(TestLogContextIdParameterName))
            {
                return TestContext.Parameters.Get(TestLogContextIdParameterName)!;
            }
            return string.Empty;
        });

        private static List<string> Logs
        {
            get
            {
                if (string.IsNullOrEmpty(ContextId.Value))
                {
                    throw new System.Exception("ContextId is not set");
                }
                return ConcurrentLogs[ContextId.Value];
            }
        }

        public static void NewLogContext(string contextName)
        {
            ContextId.Value = contextName;
            ConcurrentLogs[ContextId.Value] = new List<string>();
        }

        public static void Log(string infoToLog)
        {
            lock (Lock)
            {
                Logs.Add(infoToLog);
            }
        }

        public static void LogCurrentMethod([CallerMemberName] string callerMethodName = "")
        {
            Log(callerMethodName);
        }

        public static void LogCurrentMethodWithContextInfo(string contextInfo, [CallerMemberName] string callerMethodName = "")
        {
            Log($"{callerMethodName}({contextInfo})");
        }

        public static string[] GetLogsFor(string context)
        {
            return ConcurrentLogs[context].ToArray();
        }
    }
}
