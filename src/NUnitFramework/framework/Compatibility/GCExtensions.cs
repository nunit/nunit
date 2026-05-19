// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace System
{
#if NETFRAMEWORK
    internal static class GCExtensions
    {
        extension(GC)
        {
            public static T[] AllocateUninitializedArray<T>(int length)
            {
                // NOTE: 'pinned' parameter has been omitted from the signature for GC.AllocateUninitializedArray<T> for implementation simplicity.
                // This will then surface as a compile error by-design if we try to pass it in .NET Framework, at which point we can
                // consider adding it at that time if necessary.
                return new T[length];
            }
        }
    }
#endif
}
