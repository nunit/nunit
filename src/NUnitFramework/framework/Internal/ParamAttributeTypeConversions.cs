// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
                data[i] = NUnitConvert(data[i], targetType);
            }

            return data;
        }

        public static object NUnitConvert(object value, Type targetType)
        {
            if (value == null || targetType.GetTypeInfo().IsInstanceOfType(value))
            {
                return value;
            }

            if (value.GetType().FullName == "System.DBNull")
            {
                return null;
            }

            bool convert = false;

            if (targetType == typeof(short) || targetType == typeof(byte) || targetType == typeof(sbyte) || targetType == typeof(long?) ||
                targetType == typeof(short?) || targetType == typeof(byte?) || targetType == typeof(sbyte?) || targetType == typeof(double?))
            {
                convert = value is int;
            }
            else if (targetType == typeof(decimal) || targetType == typeof(decimal?))
            {
                convert = value is double || value is string || value is int;
            }
            else if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
            {
                convert = value is string;
            }

            if (convert)
            {
                Type convertTo = targetType.GetTypeInfo().IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                    targetType.GetGenericArguments()[0] : targetType;
                return Convert.ChangeType(value, convertTo, System.Globalization.CultureInfo.InvariantCulture);
            }

            // Convert.ChangeType doesn't work for TimeSpan from string
            if ((targetType == typeof(TimeSpan) || targetType == typeof(TimeSpan?)) && value is string)
            {
                return TimeSpan.Parse((string)value);
            }

            return value;
        }
    }
}
