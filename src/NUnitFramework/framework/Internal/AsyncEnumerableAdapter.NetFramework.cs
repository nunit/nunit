// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
#if NETFRAMEWORK

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace NUnit.Framework.Internal
{
    internal static partial class AsyncEnumerableAdapter
    {
        public static bool TryGetAsyncBlockingEnumerable(object enumerable, [NotNullWhen(true)] out IEnumerable? result)
        {
            //var asyncDisposable = enumerable.GetType().GetInterface("System.IAsyncDisposable");

            //if (asyncDisposable is null)
            {
                result = default;
                return default;
            }
        }
    }
}
#endif
