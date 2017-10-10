using System;
using System.Collections;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Helper methods for converting parameters to numeric values
    /// </summary>
    internal static class ParamNumberConversions
    {
        /// <summary>
        /// Converts an array of objects to the <paramref name="targetType"/>, if it is numeric.
        /// </summary>
        public static IEnumerable ConvertDataToNumeric(object[] data, Type targetType)
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
                object arg = data[i];

                if (arg == null)
                {
                    continue;
                }

                if (targetType.GetTypeInfo().IsAssignableFrom(arg.GetType().GetTypeInfo()))
                {
                    continue;
                }

#if !NETSTANDARD1_3 && !NETSTANDARD1_6
                if (arg is DBNull)
                {
                    data[i] = null;
                    continue;
                }
#endif

                bool convert = false;

                if (targetType == typeof(short) || targetType == typeof(byte) || targetType == typeof(sbyte))
                {
                    convert = arg is int;
                }
                else if (targetType == typeof(decimal))
                {
                    convert = arg is double || arg is string || arg is int;
                }
                else if (targetType == typeof(DateTime) || targetType == typeof(TimeSpan))
                {
                    convert = arg is string;
                }

                if (convert)
                {
                    data[i] = Convert.ChangeType(arg, targetType, System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            return data;
        }
    }
}