// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// Caches static information for ITestAction to reduce re-calculations and memory allocations from reflection.
    /// </summary>
    internal static class TestMetadataCache
    {
        private static readonly ConcurrentDictionary<Type, TestMetadata> Cache = new();

        /// <summary>
        /// Returns cached metadata for method instance.
        /// </summary>
        internal static TestMetadata Get(Type testType)
        {
            return Cache.GetOrAdd(testType, static m => new TestMetadata(m));
        }

        /// <summary>
        /// Memoization of Test information to reduce subsequent allocations from parameter and attribute information.
        /// </summary>
        internal sealed class TestMetadata
        {
            public TestMetadata(Type testType)
            {
                TestActionAttributes = GetActionsForType(testType);
            }

            public ITestAction[] TestActionAttributes { get; }

            private static ITestAction[] GetActionsForType(Type type)
            {
                if (type == null || type == typeof(object))
                {
                    return Array.Empty<ITestAction>();
                }

                var actions = new List<ITestAction>();

                actions.AddRange(GetActionsForType(type.GetTypeInfo().BaseType));

                foreach (Type interfaceType in TypeHelper.GetDeclaredInterfaces(type))
                    actions.AddRange(interfaceType.GetTypeInfo().GetAttributes<ITestAction>(false));

                actions.AddRange(type.GetTypeInfo().GetAttributes<ITestAction>(false));

                return actions.ToArray();
            }
        }
    }
}
