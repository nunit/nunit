// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace System
{
#if NETFRAMEWORK
    internal struct HashCode
    {
        public static int Combine<T1, T2>(T1 value1, T2 value2)
        {
            int hash = 0;
            hash = (hash * 31) + (value1?.GetHashCode() ?? 0);
            hash = (hash * 31) + (value2?.GetHashCode() ?? 0);
            return hash;
        }
    }
#endif
}
