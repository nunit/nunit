// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NET471_OR_GREATER || NETCOREAPP2_0_OR_GREATER

using System.Runtime.CompilerServices;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class TupleAccessor
    {
        public static ITuple? For(object? tuple)
        {
            return tuple as ITuple;
        }
    }
}

#endif
