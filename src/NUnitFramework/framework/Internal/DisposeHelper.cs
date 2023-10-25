// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    internal static class DisposeHelper
    {
        public static bool IsDisposable(Type type)
        {
            if (typeof(IDisposable).IsAssignableFrom(type))
                return true;

            return TryGetAsyncDispose(type, out _);
        }

        public static void EnsureDisposed(object? value)
        {
            if (value is not null)
            {
                if (value is IDisposable disposable)
                    disposable.Dispose();
                else if (TryGetAsyncDispose(value.GetType(), out var method))
                    AsyncToSyncAdapter.Await(() => method.Invoke(value, null));

            }
        }

        private static bool TryGetAsyncDispose(Type type, [NotNullWhen(true)] out MethodInfo? method)
        {
            method = null;

            var asyncDisposable = type.GetInterface("System.IAsyncDisposable");
            if (asyncDisposable is null)
                return false;

            var interfaceMethod = asyncDisposable.GetMethod("DisposeAsync");
            if (interfaceMethod is null)
                return false;

            if (interfaceMethod.ReturnType.FullName == "System.Threading.Tasks.ValueType")
                return false;

            var map = type.GetInterfaceMap(asyncDisposable);
            if (map.TargetMethods.Length == 0)
                return false;

            method = map.TargetMethods[0];
            return true;
        }
    }
}
