// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
#if NETFRAMEWORK

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace NUnit.Framework.Internal
{
    internal static partial class AsyncEnumerableAdapter
    {
        private static partial bool TryGetAsyncBlockingEnumerable(object enumerable, out IEnumerable<object?>? result)
        {
            result = default;

            var asyncEnumerable = enumerable.GetType().GetInterface("System.Collections.Generic.IAsyncEnumerable`1");
            if (asyncEnumerable is null)
                return false;

            var getEnumeratorMethod = asyncEnumerable.GetMethod("GetAsyncEnumerator");
            if (getEnumeratorMethod is null)
                return false;

            var enumeratorType = getEnumeratorMethod.ReturnType;

            var currentProperty = enumeratorType.GetProperty("Current");
            if (currentProperty is null)
                return false;

            var moveNextMethod = enumeratorType.GetMethod("MoveNextAsync");
            if (moveNextMethod is null)
                return false;

            var asyncDisposableType = enumeratorType.GetInterface("System.IAsyncDisposable");
            if (asyncDisposableType is null)
                return false;

            var asyncDisposeMethod = asyncDisposableType.GetMethod("DisposeAsync");
            if (asyncDisposeMethod is null)
                return false;

            var shape = new AsyncEnumerableShapeInfo(
                getEnumeratorMethod,
                currentProperty,
                moveNextMethod,
                asyncDisposeMethod);

            result = new AsyncEnumerableWrapper(shape, enumerable);
            return true;
        }

        private class AsyncEnumerableWrapper : IEnumerable<object?>
        {
            private readonly AsyncEnumerableShapeInfo _shape;
            private readonly object _asyncEnumerable;

            public AsyncEnumerableWrapper(AsyncEnumerableShapeInfo shape, object asyncEnumerable)
            {
                _shape = shape;
                _asyncEnumerable = asyncEnumerable;
            }

            public IEnumerator<object?> GetEnumerator()
                => new AsyncEnumeratorWrapper(_shape, _shape.GetAsyncEnumeratorMethod.Invoke(_asyncEnumerable, new object[] { CancellationToken.None })!);

            IEnumerator IEnumerable.GetEnumerator()
                => new AsyncEnumeratorWrapper(_shape, _shape.GetAsyncEnumeratorMethod.Invoke(_asyncEnumerable, new object[] { CancellationToken.None })!);
        }

        private class AsyncEnumeratorWrapper : IEnumerator<object?>
        {
            private readonly AsyncEnumerableShapeInfo _shape;
            private readonly object _asyncEnumerator;

            public AsyncEnumeratorWrapper(AsyncEnumerableShapeInfo shape, object asyncEnumerator)
            {
                _shape = shape;
                _asyncEnumerator = asyncEnumerator;
            }

            public object? Current => _shape.CurrentProperty.GetValue(_asyncEnumerator);

            public void Dispose()
                => AsyncToSyncAdapter.Await(() => _shape.DisposeAsyncMethod.Invoke(_asyncEnumerator, null));

            public bool MoveNext()
                => AsyncToSyncAdapter.Await<bool>(() => _shape.MoveNextAsyncMethod.Invoke(_asyncEnumerator, null));

            public void Reset()
                => throw new InvalidOperationException("Can not reset an async enumerable.");
        }

        private record AsyncEnumerableShapeInfo
        {
            public MethodInfo GetAsyncEnumeratorMethod { get; init; }
            public PropertyInfo CurrentProperty { get; init; }
            public MethodInfo MoveNextAsyncMethod { get; init; }
            public MethodInfo DisposeAsyncMethod { get; init; }

            public AsyncEnumerableShapeInfo(MethodInfo getAsyncEnumeratorMethod, PropertyInfo currentProperty, MethodInfo moveNextAsyncMethod, MethodInfo disposeAsyncMethod)
            {
                GetAsyncEnumeratorMethod = getAsyncEnumeratorMethod;
                CurrentProperty = currentProperty;
                MoveNextAsyncMethod = moveNextAsyncMethod;
                DisposeAsyncMethod = disposeAsyncMethod;
            }
        }
    }
}
#endif
