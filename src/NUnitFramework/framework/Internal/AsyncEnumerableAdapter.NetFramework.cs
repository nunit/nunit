// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
#if NETFRAMEWORK

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Internal.Extensions;

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
            private readonly MethodInfo _getAsyncEnumeratorMethod;

            private static readonly object[] EnumeratorArgs = new object[] { CancellationToken.None };

            public AsyncEnumerableWrapper(AsyncEnumerableShapeInfo shape, object asyncEnumerable)
            {
                _shape = shape;
                _asyncEnumerable = asyncEnumerable;
                _getAsyncEnumeratorMethod = shape.GetAsyncEnumeratorMethod;
            }

            public IEnumerator<object?> GetEnumerator()
                => new AsyncEnumeratorWrapper(_shape, _getAsyncEnumeratorMethod.Invoke(_asyncEnumerable, EnumeratorArgs)!);

            IEnumerator IEnumerable.GetEnumerator()
                => new AsyncEnumeratorWrapper(_shape, _getAsyncEnumeratorMethod.Invoke(_asyncEnumerable, EnumeratorArgs)!);
        }

        private class AsyncEnumeratorWrapper : IEnumerator<object?>
        {
            private readonly object _asyncEnumerator;

            private readonly MethodInfo _disposeAsyncMethod;
            private readonly Func<ValueTask<bool>> _moveNextAsyncMethod;
            private readonly Func<object?> _getCurrentMethod;

            public AsyncEnumeratorWrapper(AsyncEnumerableShapeInfo shape, object asyncEnumerator)
            {
                _asyncEnumerator = asyncEnumerator;

                _disposeAsyncMethod = shape.DisposeAsyncMethod;
                _moveNextAsyncMethod = shape.MoveNextAsyncMethod.CreateDelegate<Func<ValueTask<bool>>>(asyncEnumerator);
                _getCurrentMethod = shape.CurrentProperty.GetGetMethod()!.CreateDelegate<Func<object?>>(asyncEnumerator);
            }

            public object? Current => _getCurrentMethod.Invoke();

            public void Dispose()
                => AsyncToSyncAdapter.Await(() => _disposeAsyncMethod.Invoke(_asyncEnumerator, null));

            public bool MoveNext()
                => AsyncToSyncAdapter.Await<bool>(() => _moveNextAsyncMethod.Invoke());

            public void Reset()
                => throw new InvalidOperationException("Can not reset an async enumerable.");
        }

        private record AsyncEnumerableShapeInfo(MethodInfo GetAsyncEnumeratorMethod, PropertyInfo CurrentProperty, MethodInfo MoveNextAsyncMethod, MethodInfo DisposeAsyncMethod);
    }
}
#endif
