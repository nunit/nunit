// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Extensions
{
    /// <summary>
    /// Extensions to <see cref="IPropertyBag"/>.
    /// </summary>
    internal static class IPropertyBagDataExtensions
    {
        /// <summary>
        /// Adds the skip reason to tests that are ignored until a specific date.
        /// </summary>
        /// <param name="properties">The test properties to add the skip reason to</param>
        /// <param name="untilDate">The date that the test is being ignored until</param>
        /// <param name="reason">The reason the test is being ignored until that date</param>
        internal static void AddIgnoreUntilReason(this IPropertyBag properties, DateTimeOffset untilDate, string reason)
        {
            string skipReason = $"Ignoring until {untilDate:u}. {reason}";
            properties.Set(PropertyNames.SkipReason, skipReason);
        }
    }
}
