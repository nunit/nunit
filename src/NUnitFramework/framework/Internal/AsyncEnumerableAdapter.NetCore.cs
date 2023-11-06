// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
#if !NETFRAMEWORK

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    internal static partial class AsyncEnumerableAdapter
    {
        private static partial bool TryGetAsyncBlockingEnumerable(object enumerable, out IEnumerable<object>? result)
        {
            if (enumerable is IAsyncEnumerable<object> asyncEnumerable)
            {
                // Allow for lazily enumeration
                result = new AsyncEnumerableWrapper(asyncEnumerable);
                return true;
            }

            result = default;
            return default;
        }

        private class AsyncEnumerableWrapper : IEnumerable<object>
        {
            private readonly IAsyncEnumerable<object> _asyncEnumerable;

            public AsyncEnumerableWrapper(IAsyncEnumerable<object> asyncEnumerable)
            {
                _asyncEnumerable = asyncEnumerable;
            }

            public IEnumerator<object> GetEnumerator()
                => new AsyncEnumeratorWrapper(_asyncEnumerable.GetAsyncEnumerator());

            IEnumerator IEnumerable.GetEnumerator()
                => new AsyncEnumeratorWrapper(_asyncEnumerable.GetAsyncEnumerator());
        }

        private class AsyncEnumeratorWrapper : IEnumerator<object>
        {
            private readonly IAsyncEnumerator<object> _asyncEnumerator;

            public AsyncEnumeratorWrapper(IAsyncEnumerator<object> asyncEnumerator)
            {
                _asyncEnumerator = asyncEnumerator;
            }

            public object Current => _asyncEnumerator.Current;

            public void Dispose()
                => AsyncToSyncAdapter.Await(() => _asyncEnumerator.DisposeAsync());

            public bool MoveNext()
                => AsyncToSyncAdapter.Await<bool>(() => _asyncEnumerator.MoveNextAsync());

            public void Reset()
                => throw new InvalidOperationException("Can not reset an async enumerable.");
        }
    }
}
#endif
