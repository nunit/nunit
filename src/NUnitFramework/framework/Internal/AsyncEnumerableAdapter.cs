// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NUnit.Framework.Internal
{
    internal static partial class AsyncEnumerableAdapter
    {
        public static IEnumerable? CoalesceToEnumerable(object? enumerable)
        {
            if (enumerable is null)
            {
                return null;
            }
            else if (enumerable is IEnumerable ie)
            {
                return ie;
            }
            else if (TryGetAsyncBlockingEnumerable(enumerable, out var result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException($"Argument can not be converted to an {nameof(IEnumerable)}.", nameof(enumerable));
            }
        }

        private static partial bool TryGetAsyncBlockingEnumerable(object enumerable, [NotNullWhen(true)] out IEnumerable<object?>? result);
    }
}
