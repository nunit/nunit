// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Runtime.CompilerServices;

namespace NUnit.Framework.Internal
{
    internal static class ArrayHelper
    {
#if NET45_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] Empty<T>()
        {
#if NETSTANDARD2_0_OR_GREATER
            return Array.Empty<T>();
#else
            return InternalArrayHelper<T>.Empty;
#endif
        }

#if !NETSTANDARD2_0_OR_GREATER
        private static class InternalArrayHelper<T>
        {
            public static readonly T[] Empty = new T[0];
        }
#endif
    }
}
