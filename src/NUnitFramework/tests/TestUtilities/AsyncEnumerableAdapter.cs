// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NUnit.Framework.Tests.TestUtilities
{
    internal static class AsyncEnumerableAdapter
    {
        public static IAsyncEnumerable<T> FromEnumerable<T>(IEnumerable<T> enumerable)
            => new AsyncEnumerableWrapper<T>(enumerable);

        private class AsyncEnumerableWrapper<T> : IAsyncEnumerable<T>
        {
            private readonly IEnumerable<T> _source;

            public AsyncEnumerableWrapper(IEnumerable<T> source)
            {
                _source = source;
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
                => new AsyncEnumeratorWrapper<T>(_source.GetEnumerator());
        }

        private class AsyncEnumeratorWrapper<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _source;

            public AsyncEnumeratorWrapper(IEnumerator<T> source)
            {
                _source = source;
            }

            public T Current => _source.Current;

            public ValueTask DisposeAsync() => new();

            public ValueTask<bool> MoveNextAsync() => new(_source.MoveNext());
        }
    }
}
