// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        internal static void AddIgnoreUntilReason(this IPropertyBag properties, DateTimeOffset untilDate, string? reason)
        {
            var skipReason = reason is null ?
               $"Ignoring until {untilDate:u}." :
               $"Ignoring until {untilDate:u}. {reason}";
            properties.Set(PropertyNames.SkipReason, skipReason);
        }

        /// <summary>
        /// Gets the single value for a key or
        /// returns <paramref name="defaultValue"/> if the key is not found.
        /// </summary>
        /// <typeparam name="T">The type of the value to return.</typeparam>
        /// <param name="properties">The propertybag to search the value for.</param>
        /// <param name="key">The key to get the value for.</param>
        /// <param name="defaultValue">The value to return if <paramref name="properties"/> does not contain an entry for <paramref name="key"/>.</param>
        /// <returns>The value for <paramref name="key"/> or <paramref name="defaultValue"/>.</returns>
        internal static T TryGet<T>(this IPropertyBag properties, string key, T defaultValue)
        {
            object? value = properties.Get(key);
            return value is null ? defaultValue : (T)value;
        }
    }
}
