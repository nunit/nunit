// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// IImplyFixture is an empty marker interface used by attributes like
    /// TestAttribute that cause the class where they are used to be treated
    /// as a TestFixture even without a TestFixtureAttribute.
    ///
    /// Marker interfaces are not usually considered a good practice, but
    /// we use it here to avoid cluttering the attribute hierarchy with
    /// classes that don't contain any extra implementation.
    /// </summary>
    public interface IImplyFixture
    {
    }
}
