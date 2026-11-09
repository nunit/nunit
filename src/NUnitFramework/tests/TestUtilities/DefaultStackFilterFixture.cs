// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.TestUtilities
{
    /// <summary>
    /// Base class for fixtures that verify stack trace filtering and therefore need the default <see cref="StackFilter"/>.
    /// The tests assembly has a <c>SetUpFixture</c> that replaces it so NUnit's own test code is not filtered out.
    /// </summary>
    /// <remarks>
    /// Derived fixtures are not parallelizable because <see cref="StackFilter.DefaultFilter"/> is static.
    /// </remarks>
    [NonParallelizable]
    public abstract class DefaultStackFilterFixture
    {
        private StackFilter _savedFilter;

        [OneTimeSetUp]
        public void UseDefaultStackFilter()
        {
            _savedFilter = StackFilter.DefaultFilter;
            StackFilter.DefaultFilter = new();
        }

        [OneTimeTearDown]
        public void RestoreStackFilter()
        {
            StackFilter.DefaultFilter = _savedFilter;
        }
    }
}
