// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestData.ExecutionHooks
{
    public static class HookIdentifiers
    {
        public static readonly string Hook = "_Hook";

        public static readonly string AfterTestHook = $"AfterTestHook{Hook}";
        public static readonly string BeforeEverySetUpHook = $"BeforeEverySetUpHook{Hook}";
        public static readonly string AfterEverySetUpHook = $"AfterEverySetUpHook{Hook}";
        public static readonly string BeforeTestHook = $"BeforeTestHook{Hook}";
        public static readonly string BeforeEveryTearDownHook = $"BeforeEveryTearDownHook{Hook}";
        public static readonly string AfterEveryTearDownHook = $"AfterEveryTearDownHook{Hook}";
    }
}
