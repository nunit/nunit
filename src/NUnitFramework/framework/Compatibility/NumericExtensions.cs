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
        public static nuint MaxValue
        {
            get => unchecked((nuint)UInt64.MaxValue);
        }

        public static nuint MinValue
        {
            get => unchecked((nuint)UInt64.MinValue);
        }

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
