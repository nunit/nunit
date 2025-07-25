// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NUnit.Framework.Tests.ExecutionHooks
{
    internal static class TestLog
    {
        private static readonly AsyncLocal<List<string>> LocalLogs = new();
        private static readonly object LogLock = new();

        public static List<string> Logs
        {
            get
            {
                lock (LogLock)
                {
                    return LocalLogs.Value ??= new List<string>();
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
            Logs.Add(callerMethodName);
        }

        public static void LogCurrentMethodWithContextInfo(string contextInfo, [CallerMemberName] string callerMethodName = "")
        {
            Logs.Add($"{callerMethodName}({contextInfo})");
        }

        public static void LogMessage(string message)
        {
            Logs.Add(message);
        }

        public static void Clear()
        {
            Logs.Clear();
        }
    }
}
