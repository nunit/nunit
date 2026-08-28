// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK

namespace System;

internal static class NintExtensions
{
    extension(nint x)
    {
        public static nint MaxValue
        {
            get => unchecked((nint)Int64.MaxValue);
        }

        public static nint MinValue
        {
            get => unchecked((nint)Int64.MinValue);
        }
    }
}

internal static class NuintExtensions
{
    extension(nuint x)
    {
        public static nuint MaxValue
        {
            get => unchecked((nuint)UInt64.MaxValue);
        }

        public static nuint MinValue
        {
            get => unchecked((nuint)UInt64.MinValue);
        }
    }
}
#endif
