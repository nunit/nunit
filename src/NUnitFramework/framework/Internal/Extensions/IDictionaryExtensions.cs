// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Globalization;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class IDictionaryExtensions
    {
        public static bool TryGetValue<T>(this IDictionary<string, object> settings, string key, out T value)
        {
            if (!settings.TryGetValue(key, out var raw))
            {
                value = default!;
                return false;
            }

            // If already the right type, return it
            if (raw is T t)
            {
                value = t;
                return true;
            }

            // Handle nulls
            if (raw is null)
            {
                // If T is reference type or nullable, we can return default (null) and store null
                var targetType = typeof(T);
                if (!targetType.IsValueType || Nullable.GetUnderlyingType(targetType) is not null)
                {
                    settings[key] = null!;
                    value = default!;
                    return true;
                }

                value = default!;
                return false;
            }

            var nonNullableTarget = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            try
            {
                var converted = Convert.ChangeType(raw, nonNullableTarget, CultureInfo.InvariantCulture);

                // Cache converted value back into dictionary to avoid double-conversion
                settings[key] = converted!;

                value = (T)converted!;
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Failed to convert setting '{key}' with value '{raw}' to type '{typeof(T).FullName}'", ex);
            }
        }
    }
}
