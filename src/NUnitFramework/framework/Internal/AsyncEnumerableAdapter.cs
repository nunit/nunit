// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;

namespace NUnit.Framework.Internal
{
    internal static partial class AsyncEnumerableAdapter
    {
        public static IEnumerable? CoalesceToEnumerable(object enumerable)
        {
            if (enumerable is null)
            {
                return null;
            }
            else if (TryGetAsyncBlockingEnumerable(enumerable, out var result))
            {
                return result;
            }
            else
            {
                return (IEnumerable?)enumerable;
            }
        }
    }
}
