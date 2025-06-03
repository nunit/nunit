// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NUnit.TestData.HookExtensionTests
{
    public static class TestLog
    {
        private static readonly AsyncLocal<List<string>?> LocalLogs = new();
        private static readonly object LogLock = new();

        private static void Log(string infoToLog)
        {
            Logs ??= [];
            Logs.Add(infoToLog);
        }

        // Each aync context gets its own instance of Logs
        public static List<string>? Logs
        {
            get
            {
                lock (LogLock)
                {
                    return LocalLogs.Value;
                }
            }

            private set
            {
                lock (LogLock)
                {
                    LocalLogs.Value = value;
                }
            }
        }

        public static void LogCurrentMethod([CallerMemberName] string callerMethodName = "")
        {
            Log(callerMethodName);
        }

        public static void Clear() => Logs?.Clear();
    }
}
