// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using System.Threading;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks a test as needing to be run in a particular threading apartment state. This will cause it
    /// to run in a separate thread if necessary.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class ApartmentAttribute : PropertyAttribute
    {
        /// <summary>
        /// Construct an ApartmentAttribute
        /// </summary>
        /// <param name="apartmentState">The apartment state that this test must be run under. You must pass in a valid apartment state.</param>
        public ApartmentAttribute(ApartmentState apartmentState)
        {
            Guard.ArgumentValid(apartmentState != ApartmentState.Unknown, "must be STA or MTA", nameof(apartmentState));
            Properties.Add(PropertyNames.ApartmentState, apartmentState);
        }
    }
}
