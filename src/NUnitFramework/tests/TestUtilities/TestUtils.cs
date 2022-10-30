// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading;

namespace NUnit.TestUtilities
{
    internal static class TestUtils
    {
        public static IDisposable TemporarySynchronizationContext(SynchronizationContext synchronizationContext)
        {
            var restore = RestoreSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);
            return restore;
        }

        public static IDisposable RestoreSynchronizationContext()
        {
            var originalContext = SynchronizationContext.Current;
            return On.Dispose(() => SynchronizationContext.SetSynchronizationContext(originalContext));
        }
    }
}
