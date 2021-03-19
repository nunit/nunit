// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Security;

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
        [SecurityCritical]  // Override of security critical method must be security critical itself
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
