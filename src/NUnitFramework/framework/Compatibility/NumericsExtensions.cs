// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK

namespace System;

internal static partial class NumericsExtensions
{
    extension(nint x)
    {
        public int CompareTo(nint value)
        {
            if (x < value)
                return -1;
            else if (x > value)
                return 1;

            return 0;
        }
    }

    extension(nuint x)
    {
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
