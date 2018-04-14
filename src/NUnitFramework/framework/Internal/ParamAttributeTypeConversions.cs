// ***********************************************************************
// Copyright (c) 2017–2018 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Helper methods for converting parameters to numeric values to supported types
    /// </summary>
    internal static class ParamAttributeTypeConversions
    {
        /// <summary>
        /// Converts an array of objects to the <paramref name="targetType"/>, if it is supported.
        /// </summary>
        public static IEnumerable ConvertData(object[] data, Type targetType)
        {
            Guard.ArgumentNotNull(data, nameof(data));
            Guard.ArgumentNotNull(targetType, nameof(targetType));

            if (targetType.GetTypeInfo().IsEnum && data.Length == 0)
            {
                return Enum.GetValues(targetType);
            }
            if (targetType == typeof(bool) && data.Length == 0)
            {
                return new object[] { true, false };
            }
            return GetData(data, targetType);
        }

        private static IEnumerable GetData(object[] data, Type targetType)
        {
            for (int i = 0; i < data.Length; i++)
            {
                object convertedValue;
                if (TryConvert(data[i], targetType, out convertedValue))
                    data[i] = convertedValue;
            }

            return data;
        }

        /// <summary>
        /// Converts a single value to the <paramref name="targetType"/>, if it is supported.
        /// </summary>
        public static object Convert(object value, Type targetType)
        {
            object convertedValue;
            if (TryConvert(value, targetType, out convertedValue))
                return convertedValue;

            throw new InvalidOperationException(
                (value == null ? "Null" : $"A value of type {value.GetType()} ({value})")
                + $" cannot be passed to a parameter of type {targetType}.");
        }

        /// <summary>
        /// Converts a single value to the <paramref name="targetType"/>, if it is supported.
        /// </summary>
        public static bool TryConvert(object value, Type targetType, out object convertedValue)
        {
            if (targetType.GetTypeInfo().IsInstanceOfType(value))
            {
                convertedValue = value;
                return true;
            }

            if (value == null || value.GetType().FullName == "System.DBNull")
            {
                convertedValue = null;
                return Reflect.IsAssignableFromNull(targetType);
            }

            bool convert = false;

            if (targetType == typeof(short) || targetType == typeof(byte) || targetType == typeof(sbyte))
            {
                convert = value is int;
            }
            else if (targetType == typeof(decimal))
            {
                convert = value is double || value is string || value is int;
            }
            else if (targetType == typeof(DateTime) || targetType == typeof(TimeSpan))
            {
                convert = value is string;
            }

            if (convert)
            {
                convertedValue = System.Convert.ChangeType(value, targetType, System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }

            convertedValue = null;
            return false;
        }
    }
}
