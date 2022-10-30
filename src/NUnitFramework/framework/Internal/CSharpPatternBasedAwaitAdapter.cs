// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System.Collections.Concurrent;

namespace NUnit.Framework.Internal
{
    internal static partial class CSharpPatternBasedAwaitAdapter
    {
        private static readonly ConcurrentDictionary<Type, AwaitShapeInfo?> ShapeInfoByType = new ConcurrentDictionary<Type, AwaitShapeInfo?>();

        public static AwaitAdapter? TryCreate(object? awaitable)
        {
            if (awaitable == null) return null;

            return GetShapeInfo(awaitable.GetType())?.CreateAwaitAdapter(awaitable);
        }

        public static bool IsAwaitable(Type awaitableType)
        {
            return GetShapeInfo(awaitableType) != null;
        }

        public static Type? GetResultType(Type awaitableType)
        {
            return GetShapeInfo(awaitableType)?.ResultType;
        }

        private static AwaitShapeInfo? GetShapeInfo(Type type)
        {
            return ShapeInfoByType.GetOrAdd(type, AwaitShapeInfo.TryCreate);
        }
    }
}
