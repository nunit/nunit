// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Reflection;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class MethodInfoExtensions
    {
        public static TReturn? InvokeMaybeAwait<TReturn>(this MethodInfo m) =>
            InvokeMaybeAwait<TReturn?>(m, null);

        public static TReturn? InvokeMaybeAwait<TReturn>(this MethodInfo m, object?[]? methodArgs)
        {
            if (AsyncToSyncAdapter.IsAsyncOperation(m))
                return (TReturn?)AsyncToSyncAdapter.Await(context: null, () => m.Invoke(null, methodArgs));
            return (TReturn?)m.Invoke(null, methodArgs);
        }

#if NETFRAMEWORK
        public static T CreateDelegate<T>(this MethodInfo m, object target)
            where T : System.Delegate
            => (T)m.CreateDelegate(typeof(T), target);
#endif
    }
}
