using System;
using System.Threading;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly,
        AllowMultiple = false,
        Inherited = true)]
    public sealed class ApartmentAttribute : PropertyAttribute
    {
        public ApartmentAttribute(ApartmentState apartmentState)
        {
            if (apartmentState == ApartmentState.Unknown)
                throw new ArgumentOutOfRangeException(
                    nameof(apartmentState),
                    "ApartmentState must be STA or MTA");

            Properties.Add(PropertyNames.ApartmentState, apartmentState);
        }
    }
}

