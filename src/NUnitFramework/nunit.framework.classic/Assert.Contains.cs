// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Classic
{
    public partial class Assert
    {
        /// <summary>
        /// Asserts that an object is contained in a collection. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The collection to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Contains(object? expected, ICollection? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, new SomeItemsConstraint(new EqualConstraint(expected)), message, args);
        }

        /// <summary>
        /// Asserts that an object is contained in a collection. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The collection to be examined</param>
        public static void Contains(object? expected, ICollection? actual)
        {
            Assert.That(actual, new SomeItemsConstraint(new EqualConstraint(expected)), null, null);
        }
    }
}
