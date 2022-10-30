// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;

namespace NUnit.TestData
{
    [RequiresThread(ApartmentState.Unknown)]
    public class ApartmentDataRequiresThreadAttribute
    {
    }

    [Apartment(ApartmentState.Unknown)]
    public class ApartmentDataApartmentAttribute
    {
    }
}
