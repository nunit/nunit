// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
#if NETFRAMEWORK
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
#endif

namespace NUnit.Framework.Internal
{
    internal static class DisposeHelper
    {
        public static bool IsDisposable(Type type)
        {
            if (typeof(IDisposable).IsAssignableFrom(type))
            {
                return true;
            }
#if NETFRAMEWORK
            return TryGetAsyncDispose(type, out _);
#else
            return typeof(IAsyncDisposable).IsAssignableFrom(type);
#endif
        }

        public static void EnsureDisposed(object? value)
        {
            if (value is not null)
            {
#if NETFRAMEWORK
                if (TryGetAsyncDispose(value.GetType(), out var method))
                {
                    AsyncToSyncAdapter.Await(() => method.Invoke(value, null));
                }
#else
                if (value is IAsyncDisposable asyncDisposable)
                {
                    AsyncToSyncAdapter.Await(() => asyncDisposable.DisposeAsync());
                }
#endif
                else if (value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

#if NETFRAMEWORK
        private static bool TryGetAsyncDispose(Type type, [NotNullWhen(true)] out MethodInfo? method)
        {
            method = null;

            var asyncDisposable = type.GetInterface("System.IAsyncDisposable");
            if (asyncDisposable is null)
                return false;

            method = asyncDisposable.GetMethod("DisposeAsync", Type.EmptyTypes);
            return method is not null;
        }
#endif
    }
}
