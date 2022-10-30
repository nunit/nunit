// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

namespace NUnit.Framework
{
    /// <summary>
    /// Provides literal arguments for an individual parameter of a test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ValuesAttribute : NUnitAttribute, IParameterDataSource
    {
        /// <summary>
        /// The collection of data to be returned. Must
        /// be set by any derived attribute classes.
        /// We use an object[] so that the individual
        /// elements may have their type changed in GetData
        /// if necessary
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected object?[] data;
#pragma warning restore IDE1006

        /// <summary>
        /// Constructs for use with an Enum parameter. Will pass every enum
        /// value in to the test.
        /// </summary>
        public ValuesAttribute()
        {
            data = Internal.TestParameters.NoArguments;
        }

        /// <summary>
        /// Construct with one argument
        /// </summary>
        /// <param name="arg1"></param>
        public ValuesAttribute(object? arg1)
        {
            data = new[] { arg1 };
        }

        /// <summary>
        /// Construct with two arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public ValuesAttribute(object? arg1, object? arg2)
        {
            data = new[] { arg1, arg2 };
        }

        /// <summary>
        /// Construct with three arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public ValuesAttribute(object? arg1, object? arg2, object? arg3)
        {
            data = new[] { arg1, arg2, arg3 };
        }

        /// <summary>
        /// Construct with an array of arguments
        /// </summary>
        /// <param name="args"></param>
        public ValuesAttribute(params object?[]? args)
        {
            data = args ?? new object?[] { null };
        }

        /// <summary>
        /// Retrieves a list of arguments which can be passed to the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter of a parameterized test.</param>
        public IEnumerable GetData(IParameterInfo parameter)
        {
            if (data.Length == 0)
                return GenerateData(parameter.ParameterType);
            else
                return ParamAttributeTypeConversions.ConvertData(data, parameter.ParameterType);
        }

        /// <summary>
        /// To generate data for Values attribute, in case no data is provided.
        /// </summary>
        private static IEnumerable GenerateData(Type targetType)
        {
            if (IsNullableEnum(targetType))
            {
                var enumValues = Enum.GetValues(Nullable.GetUnderlyingType(targetType));
                var enumValuesWithNull = new object[enumValues.Length + 1];
                Array.Copy(enumValues, enumValuesWithNull, enumValues.Length);
                return enumValuesWithNull;
            }
            if (targetType.IsEnum)
            {
                return Enum.GetValues(targetType);
            }
            if (targetType == typeof(bool?))
            {
                return new object?[] { null, true, false };
            }
            if (targetType == typeof(bool))
            {
                return new object[] { true, false };
            }

            return Internal.TestParameters.NoArguments;
        }

        /// <summary>
        /// To Check if type is nullable enum.
        /// </summary>
        private static bool IsNullableEnum(Type t)
        {
            Type u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }
    }
}
