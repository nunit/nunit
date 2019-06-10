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
using System.ComponentModel;
using System.Globalization;

#if !(NET35 || NET40 || NET45)
using System.Reflection;
#endif

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// <para>
    /// Examines an attribute argument and tries to simulate what that value would have been if the literal syntax
    /// which might have defined the value in C# had instead been used as an argument to a given method parameter in a direct call.
    /// </para>
    /// <para>
    /// For example, since you can’t apply attributes using <see cref="decimal"/> arguments, we allow the C# syntax
    /// <c>10</c> (<see cref="int"/> value) or <c>0.1</c> (<see cref="double"/> value) to be specified.
    /// NUnit then converts it to match the method’s <see cref="decimal"/> parameters, just as if you were actually
    /// using the syntax <c>TestMethod(10)</c> or <c>TestMethod(0.1)</c>.
    /// </para>
    /// <para>
    /// For another example, you might have written the syntax <c>10</c> and picked up the <see cref="int"/> attribute
    /// constructor overload; however, the test method for which this value is intended only has a <see cref="byte"/>
    /// signature. Again, NUnit simulates what would have happened if the inferred C# syntax was transplanted
    /// and you were actually using the syntax <c>TestMethod(10)</c>.
    /// </para>
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
            return GetData(data, targetType);
        }

        private static IEnumerable GetData(object[] data, Type targetType)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (TryConvert(data[i], targetType, out var convertedValue))
                    data[i] = convertedValue;
            }

            return data;
        }

        /// <summary>
        /// Converts a single value to the <paramref name="targetType"/>, if it is supported.
        /// </summary>
        public static object Convert(object value, Type targetType)
        {
            if (TryConvert(value, targetType, out var convertedValue))
                return convertedValue;

            throw new InvalidOperationException(
                (value == null ? "Null" : $"A value of type {value.GetType()} ({value})")
                + $" cannot be passed to a parameter of type {targetType}.");
        }

        /// <summary>
        /// Performs several special conversions allowed by NUnit in order to
        /// permit arguments with types that cannot be used in the constructor
        /// of an Attribute such as TestCaseAttribute or to simplify their use.
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <param name="targetType">The target <see cref="Type"/> in which the <paramref name="value"/> should be converted</param>
        /// <param name="convertedValue">If conversion was successfully applied, the <paramref name="value"/> converted into <paramref name="targetType"/></param>
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> was converted and <paramref name="convertedValue"/> should be used;
        /// <c>false</c> is no conversion was applied and <paramref name="convertedValue"/> should be ignored
        /// </returns>
        public static bool TryConvert(object value, Type targetType, out object convertedValue)
        {
            if (targetType.IsInstanceOfType(value))
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
            var underlyingTargetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingTargetType == typeof(short) || underlyingTargetType == typeof(byte) || underlyingTargetType == typeof(sbyte)
                || underlyingTargetType == typeof(long) || underlyingTargetType == typeof(double))
            {
                convert = value is int;
            }
            else if (underlyingTargetType == typeof(decimal))
            {
                convert = value is double || value is string || value is int;
            }
            else if (underlyingTargetType == typeof(DateTime))
            {
                convert = value is string;
            }

            if (convert)
            {
                convertedValue = System.Convert.ChangeType(value, underlyingTargetType, CultureInfo.InvariantCulture);
                return true;
            }

            var converter = TypeDescriptor.GetConverter(underlyingTargetType);
            if (converter.CanConvertFrom(value.GetType()))
            {
                convertedValue = converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
                return true;
            }

            convertedValue = null;
            return false;
        }
    }
}
