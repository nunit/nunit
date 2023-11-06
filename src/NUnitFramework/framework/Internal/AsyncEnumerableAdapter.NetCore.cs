// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
#if !NETFRAMEWORK

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NUnit.Framework.Internal
{
    internal static partial class AsyncEnumerableAdapter
    {
        public static bool TryGetAsyncBlockingEnumerable(object enumerable, [NotNullWhen(true)] out IEnumerable<object>? result)
        {
            if (enumerable is IAsyncEnumerable<object> asyncEnumerable)
            {
                // Allow for lazily enumeration
                result = new AsyncWrapperEnumerable(asyncEnumerable);
                return true;
            }

            result = default;
            return default;
        }

        private class AsyncWrapperEnumerable : IEnumerable<object>
        {
            private readonly IAsyncEnumerable<object> _asyncEnumerable;

            public AsyncWrapperEnumerable(IAsyncEnumerable<object> asyncEnumerable)
            {
                _asyncEnumerable = asyncEnumerable;
            }

            public IEnumerator<object> GetEnumerator()
                => new AsyncWrapperEnumerator(_asyncEnumerable.GetAsyncEnumerator());

            IEnumerator IEnumerable.GetEnumerator()
                => new AsyncWrapperEnumerator(_asyncEnumerable.GetAsyncEnumerator());
        }

        private class AsyncWrapperEnumerator : IEnumerator<object>
        {
            private readonly IAsyncEnumerator<object> _asyncEnumerator;

            public AsyncWrapperEnumerator(IAsyncEnumerator<object> asyncEnumerator)
            {
                _asyncEnumerator = asyncEnumerator;
            }

            public object Current => _asyncEnumerator.Current;

            public void Dispose()
                => AsyncToSyncAdapter.Await(() => _asyncEnumerator.DisposeAsync());

            public bool MoveNext()
                => (bool)AsyncToSyncAdapter.Await(() => _asyncEnumerator.MoveNextAsync());

            public void Reset()
                => throw new InvalidOperationException("Can not reset an async enumerable.");
        }
    }
}
#endif
