// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework
{
#if NETFRAMEWORK
    using System;

    internal static class TypeExtensions
    {
        public static bool IsAssignableTo(this Type target, Type c)
        {
            return c.IsAssignableFrom(target);
        }
    }
#endif
}
