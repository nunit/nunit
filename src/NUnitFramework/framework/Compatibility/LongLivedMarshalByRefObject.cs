// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Compatibility
{
    /// <summary>
    /// A MarshalByRefObject that lives forever
    /// </summary>
    public class LongLivedMarshalByRefObject : MarshalByRefObject
    {
        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
#if NET6_0_OR_GREATER
        [Obsolete("Preventing throwing PlatformNotSupportedException")]
#endif
        public override object InitializeLifetimeService()
        {
            return null!;
        }
    }
}
