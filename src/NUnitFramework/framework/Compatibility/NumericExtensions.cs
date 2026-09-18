// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK

namespace System;

internal static class NintExtensions
{
    extension(nint x)
    {
        public static nint MaxValue => UIntPtr.Size == 8 ? unchecked((nint)Int64.MaxValue) : unchecked((nint)Int32.MaxValue);

        public static nint MinValue => UIntPtr.Size == 8 ? unchecked((nint)Int64.MinValue) : unchecked((nint)Int32.MinValue);

        public int CompareTo(nint value)
        {
            if (x < value)
                return -1;
            else if (x > value)
                return 1;

            return 0;
        }
    }
}

internal static class NuintExtensions
{
    extension(nuint x)
    {
        public static nuint MaxValue => UIntPtr.Size == 8 ? unchecked((nuint)UInt64.MaxValue) : unchecked((nuint)UInt32.MaxValue);

        public static nuint MinValue => UIntPtr.Size == 8 ? unchecked((nuint)UInt64.MinValue) : unchecked((nuint)UInt32.MinValue);

        public int CompareTo(nuint value)
        {
            if (x < value)
                return -1;
            else if (x > value)
                return 1;

            return 0;
        }
    }
}
#endif
