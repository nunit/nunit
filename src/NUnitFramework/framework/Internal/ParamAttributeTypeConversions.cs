// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

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
        public static IEnumerable ConvertData(object?[] data, Type targetType)
        {
            ArgumentNullException.ThrowIfNull(data);
            ArgumentNullException.ThrowIfNull(targetType);
            return GetData(data, targetType);
        }

        private static object?[] GetData(object?[] data, Type targetType)
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
        public static object Convert(object? value, Type targetType)
        {
            if (TryConvert(value, targetType, out var convertedValue))
                return convertedValue;

            throw new InvalidOperationException(
                (value is null ? "Null" : $"A value of type {value.GetType()} ({value})")
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
        /// <see langword="true"/> if <paramref name="value"/> was converted and <paramref name="convertedValue"/> should be used;
        /// <see langword="false"/> is no conversion was applied and <paramref name="convertedValue"/> should be ignored
        /// </returns>
        public static bool TryConvert(object? value, Type targetType, [NotNullWhen(true)] out object? convertedValue)
        {
            if (targetType.IsInstanceOfType(value))
            {
                convertedValue = value;
                return true;
            }

            if (value is null || value.GetType().FullName == "System.DBNull")
            {
                convertedValue = null;
                return Reflect.IsAssignableFromNull(targetType);
            }

            var underlyingTargetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            var valueType = value.GetType();

            bool convert = Reflect.HasNUnitConversion(valueType, underlyingTargetType);

            if (convert)
            {
                convertedValue = System.Convert.ChangeType(value, underlyingTargetType, CultureInfo.InvariantCulture);
                return true;
            }

            var converter = TypeDescriptor.GetConverter(underlyingTargetType);
            if (converter.CanConvertFrom(valueType))
            {
                convertedValue = converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
                return convertedValue is not null;
            }

            if (TryGetTupleType(underlyingTargetType, out var tupleTypes))
            {
                convertedValue = TryConvertToTuple(value, underlyingTargetType, tupleTypes);
                return convertedValue is not null;
            }

            convertedValue = null;
            return false;
        }

        private static object? TryConvertToTuple(object? value, Type targetType, Type[] tupleTypes)
        {
            var inflatedValue = InflateTupleRestArg(value, tupleTypes);

            if (inflatedValue is object?[] array &&
                array.Length == tupleTypes.Length &&
                TryConvertTupleComponents(array, tupleTypes, out var ctorArgs))
            {
                var ctor = targetType.GetConstructor(tupleTypes)!;
                return ctor.Invoke(ctorArgs);
            }
            else if (tupleTypes.Length == 1 &&
                     TryConvert(inflatedValue, tupleTypes[0], out var tupleArg))
            {
                var ctor = targetType.GetConstructor(tupleTypes)!;
                return ctor.Invoke(new[] { tupleArg });
            }

            return null;
        }

        private static object? InflateTupleRestArg(object? value, Type[] tupleTypes)
        {
            object? inflatedValue;
            if (value is object?[] longArray &&
                longArray.Length > tupleTypes.Length &&
                tupleTypes.Length > 0 &&
                TryGetTupleType(tupleTypes[tupleTypes.Length - 1], out _))
            {
                var inflatedArray = new object[tupleTypes.Length];
                Array.Copy(longArray, inflatedArray, tupleTypes.Length - 1);
                var restArray = new object[longArray.Length - tupleTypes.Length + 1];
                Array.Copy(longArray, tupleTypes.Length - 1, restArray, 0, restArray.Length);
                inflatedArray[tupleTypes.Length - 1] = restArray;

                inflatedValue = inflatedArray;
            }
            else
            {
                inflatedValue = value;
            }

            return inflatedValue;
        }

        private static bool TryConvertTupleComponents
            (object?[] array, Type[] tupleTypes, [MaybeNullWhen(false)] out object?[] tupleComponents)
        {
            var components = new object?[array.Length];
            for (int i = 0; i < components.Length; i++)
            {
                if (!TryConvert(array[i], tupleTypes[i], out components[i]))
                {
                    tupleComponents = null;
                    return false;
                }
            }

            tupleComponents = components;
            return true;
        }

        private static bool TryGetTupleType(Type type, [MaybeNullWhen(false)] out Type[] typeArgs)
        {
            if (!type.IsGenericType)
            {
                typeArgs = null;
                return false;
            }

            Type genericType = type.GetGenericTypeDefinition();
            if (genericType.Assembly == typeof(Tuple).Assembly &&
                genericType.Namespace == typeof(Tuple).Namespace &&
                genericType.Name.StartsWith("Tuple`", StringComparison.Ordinal))
            {
                typeArgs = type.GetGenericArguments();
                return true;
            }

            if (genericType.Assembly == typeof(ValueTuple).Assembly &&
                genericType.Namespace == typeof(ValueTuple).Namespace &&
                genericType.Name.StartsWith("ValueTuple`", StringComparison.Ordinal))
            {
                typeArgs = type.GetGenericArguments();
                return true;
            }

            typeArgs = null;
            return false;
        }
    }
}
