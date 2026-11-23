// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.TestUtilities
{
    /// <summary>
    /// Base class for fixtures that verify stack trace filtering and therefore need the default <see cref="StackFilter"/>.
    /// The tests assembly has a <c>SetUpFixture</c> that replaces it so NUnit's own test code is not filtered out.
    /// </summary>
    /// <remarks>
    /// Derived fixtures are not parallelizable because <see cref="StackFilter.Current"/> is static.
    /// </remarks>
    [NonParallelizable]
    public abstract class DefaultStackFilterFixture
    {
        private IDisposable _stackFilter;

        [OneTimeSetUp]
        public void UseDefaultStackFilter()
        {
            _stackFilter = StackFilter.SetCurrentFilterToDefault();
        }

        [OneTimeTearDown]
        public void RestoreStackFilter()
        {
            _stackFilter.Dispose();
        }
    }
}
