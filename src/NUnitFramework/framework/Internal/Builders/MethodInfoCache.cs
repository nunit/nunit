// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Concurrent;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// Caches static information for IMethodInfo to reduce re-calculations and memory allocations from reflection.
    /// </summary>
    internal static class MethodInfoCache
    {
        // we would otherwise do a lot of attribute allocations when repeatedly checking the same method for example
        // in case of building TestCaseAttribute-based tests
#if NET35
        private static readonly Dictionary<IMethodInfo, TestMethodMetadata> MethodMetadataCache = new Dictionary<IMethodInfo, TestMethodMetadata>();
#else
        private static readonly ConcurrentDictionary<IMethodInfo, TestMethodMetadata> MethodMetadataCache = new ConcurrentDictionary<IMethodInfo, TestMethodMetadata>();
#endif

        /// <summary>
        /// Returns cached metadata for method instance.
        /// </summary>
        internal static TestMethodMetadata Get(IMethodInfo method)
        {
#if NET35
            lock (MethodMetadataCache)
            {
                if (!MethodMetadataCache.TryGetValue(method, out var metadata))
                    MethodMetadataCache.Add(method, metadata = new TestMethodMetadata(method));

                return metadata;
            }
#else
            return MethodMetadataCache.GetOrAdd(method, m => new TestMethodMetadata(m));
#endif
        }

        /// <summary>
        /// Memoization of TestMethod information to reduce subsequent allocations from parameter and attribute information.
        /// </summary>
        internal sealed class TestMethodMetadata
        {
            public TestMethodMetadata(IMethodInfo method)
            {
                Parameters = method.GetParameters();
                IsAsyncOperation = AsyncToSyncAdapter.IsAsyncOperation(method.MethodInfo);
                IsVoidOrUnit = Reflect.IsVoidOrUnit(method.ReturnType.Type);

                // TODO could probably go trough inherited and non inherited in two passes instead of multiple

                // inherited
                RepeatTestAttributes = method.GetCustomAttributes<IRepeatTest>(true);
                WrapTestMethodAttributes = method.GetCustomAttributes<IWrapTestMethod>(true);
                WrapSetupTearDownAttributes = method.GetCustomAttributes<IWrapSetUpTearDown>(true);
                ApplyToContextAttributes = method.GetCustomAttributes<IApplyToContext>(true);

                // non-inherited
                TestBuilderAttributes = method.GetCustomAttributes<ITestBuilder>(false);
                TestActionAttributes = method.GetCustomAttributes<ITestAction>(false);
            }

            public IParameterInfo[] Parameters { get; }
            public bool IsAsyncOperation { get; }
            public bool IsVoidOrUnit { get; }

            public IRepeatTest[] RepeatTestAttributes { get; }
            public ITestBuilder[] TestBuilderAttributes { get; }
            public IWrapTestMethod[] WrapTestMethodAttributes { get; }
            public ITestAction[] TestActionAttributes { get; }
            public IWrapSetUpTearDown[] WrapSetupTearDownAttributes { get; }
            public IApplyToContext[] ApplyToContextAttributes { get; }
        }
    }
}
