// ***********************************************************************
// Copyright (c) 2008-2015 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TypeHelper provides static methods that operate on Types.
    /// </summary>
    public class TypeHelper
    {
        private const int STRING_MAX = 40;
        private const int STRING_LIMIT = STRING_MAX - 3;
        private const string THREE_DOTS = "...";

        internal sealed class NonmatchingTypeClass
        {
        }

        /// <summary>
        /// A special value, which is used to indicate that BestCommonType() method
        /// was unable to find a common type for the specified arguments.
        /// </summary>
        public static readonly Type NonmatchingType = typeof(NonmatchingTypeClass);

        /// <summary>
        /// Gets the display name for a Type as used by NUnit.
        /// </summary>
        /// <param name="type">The Type for which a display name is needed.</param>
        /// <returns>The display name for the Type</returns>
        public static string GetDisplayName(Type type)
        {
            if (type.IsGenericParameter)
                return type.Name;

            if (type.GetTypeInfo().IsGenericType)
            {
                string name = type.FullName;
                int index = name.IndexOf('[');
                if (index >= 0) name = name.Substring(0, index);

                index = name.LastIndexOf('.');
                if (index >= 0) name = name.Substring(index + 1);

                var genericArguments = type.GetGenericArguments();
                var currentArgument = 0;

                StringBuilder sb = new StringBuilder();

                bool firstClassSeen = false;
                foreach (string nestedClass in name.Split('+'))
                {
                    if (firstClassSeen)
                        sb.Append("+");

                    firstClassSeen = true;

                    index = nestedClass.IndexOf('`');
                    if (index >= 0)
                    {
                        var nestedClassName = nestedClass.Substring(0, index);
                        sb.Append(nestedClassName);
                        sb.Append("<");

                        var argumentCount = Int32.Parse(nestedClass.Substring(index + 1));
                        for (int i = 0; i < argumentCount; i++)
                        {
                            if (i > 0)
                                sb.Append(",");

                            sb.Append(GetDisplayName(genericArguments[currentArgument++]));
                        }
                        sb.Append(">");
                    }
                    else
                        sb.Append(nestedClass);
                }

                return sb.ToString();
            }

            int lastdot = type.FullName.LastIndexOf('.');
            return lastdot >= 0
                ? type.FullName.Substring(lastdot + 1)
                : type.FullName;
        }

        /// <summary>
        /// Gets the display name for a Type as used by NUnit.
        /// </summary>
        /// <param name="type">The Type for which a display name is needed.</param>
        /// <param name="arglist">The arglist provided.</param>
        /// <returns>The display name for the Type</returns>
        public static string GetDisplayName(Type type, object[] arglist)
        {
            string baseName = GetDisplayName(type);
            if (arglist == null || arglist.Length == 0)
                return baseName;

            StringBuilder sb = new StringBuilder(baseName);

            sb.Append("(");
            for (int i = 0; i < arglist.Length; i++)
            {
                if (i > 0) sb.Append(",");

                object arg = arglist[i];
                string display = arg == null ? "null" : arg.ToString();

                if (arg is double || arg is float)
                {
                    if (display.IndexOf('.') == -1)
                        display += ".0";
                    display += arg is double ? "d" : "f";
                }
                else if (arg is decimal) display += "m";
                else if (arg is long) display += "L";
                else if (arg is ulong) display += "UL";
                else if (arg is string)
                {
                    if (display.Length > STRING_MAX)
                        display = display.Substring(0, STRING_LIMIT) + THREE_DOTS;
                    display = "\"" + display + "\"";
                }

                sb.Append(display);
            }
            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// Returns the best fit for a common type to be used in
        /// matching actual arguments to a methods Type parameters.
        /// </summary>
        /// <param name="type1">The first type.</param>
        /// <param name="type2">The second type.</param>
        /// <returns>Either type1 or type2, depending on which is more general.</returns>
        public static Type BestCommonType(Type type1, Type type2)
        {
            if (type1 == TypeHelper.NonmatchingType) return TypeHelper.NonmatchingType;
            if (type2 == TypeHelper.NonmatchingType) return TypeHelper.NonmatchingType;

            if (type1 == type2) return type1;
            if (type1 == null) return type2;
            if (type2 == null) return type1;

            if (TypeHelper.IsNumeric(type1) && TypeHelper.IsNumeric(type2))
            {
                if (type1 == typeof(double)) return type1;
                if (type2 == typeof(double)) return type2;

                if (type1 == typeof(float)) return type1;
                if (type2 == typeof(float)) return type2;

                if (type1 == typeof(decimal)) return type1;
                if (type2 == typeof(decimal)) return type2;

                if (type1 == typeof(UInt64)) return type1;
                if (type2 == typeof(UInt64)) return type2;

                if (type1 == typeof(Int64)) return type1;
                if (type2 == typeof(Int64)) return type2;

                if (type1 == typeof(UInt32)) return type1;
                if (type2 == typeof(UInt32)) return type2;

                if (type1 == typeof(Int32)) return type1;
                if (type2 == typeof(Int32)) return type2;

                if (type1 == typeof(UInt16)) return type1;
                if (type2 == typeof(UInt16)) return type2;

                if (type1 == typeof(Int16)) return type1;
                if (type2 == typeof(Int16)) return type2;

                if (type1 == typeof(byte)) return type1;
                if (type2 == typeof(byte)) return type2;

                if (type1 == typeof(sbyte)) return type1;
                if (type2 == typeof(sbyte)) return type2;
            }

            if (type1.IsAssignableFrom(type2)) return type1;
            if (type2.IsAssignableFrom(type1)) return type2;

            return TypeHelper.NonmatchingType;
        }

        /// <summary>
        /// Determines whether the specified type is numeric.
        /// </summary>
        /// <param name="type">The type to be examined.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is numeric; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumeric(Type type)
        {
            return type == typeof(byte)
                   || type == typeof(sbyte)
                   || type == typeof(short)
                   || type == typeof(ushort)
                   || type == typeof(int)
                   || type == typeof(uint)
                   || type == typeof(long)
                   || type == typeof(ulong)
                   || type == typeof(decimal)
                   || type == typeof(float)
                   || type == typeof(double)
                ;
        }

        /// <summary>
        /// Return default value for selected <paramref name="type">type</paramref>.
        /// </summary>
        /// <param name="type">The type to get default value for.</param>
        public static object GetDefaultValue(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        #region Type conversions
        /// <summary>
        /// Attempt to convert some <paramref name="value">value</paramref> into
        /// another <paramref name="targetType">type</paramref>.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <param name="targetType">Taget type for the conversion.</param>
        /// <returns>
        /// Returns true if conversion was successful.
        /// </returns>
        public static bool TryConvert(ref object value, Type targetType)
        {
            // Nulls should be converted to default value of the target type.
            if (value == null)
            {
                value = GetDefaultValue(targetType);
                return true;
            }

            // DBNull.Value should be converted to default value of the target type.
            if (value is DBNull)
            {
                value = GetDefaultValue(targetType);
                return true;
            }

            // If target type is Nullable<T>, we can unwrap this interface.
            // If source value is Nullable, CLR will unwrap it automatically while
            // boxing Nullable<T> to value object we are working with.
            var targetInfo = targetType.GetTypeInfo();
            if (targetInfo.IsGenericType && targetInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                targetType = targetType.GetGenericArguments()[0];
            }

            // If source and target types are equal or assignable, then we reuse the same value.
            // Equality checking is redundant, but improves performance.
            Type type = value.GetType();
            if (targetType == type || targetType.IsAssignableFrom(type))
            {
                return true;
            }

            // Types that may be converted using Convert.ChangeType.
            if (ConvertibleTypes.Internal[type, targetType])
            {
                value = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                return true;
            }

            // Custom string parsing routines those are not supported by IConvertible.
            var stringValue = value as string;
            if (stringValue != null)
            {
                if (targetType == typeof(TimeSpan))
                {
                    value = TimeSpan.Parse(stringValue);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Convert an argument list to the required parameter types.
        /// Currently, only widening numeric conversions are performed.
        /// </summary>
        /// <param name="arglist">An array of args to be converted</param>
        /// <param name="parameters">A ParameterInfo[] whose types will be used as targets</param>
        public static void ConvertArgumentList(object[] arglist, IParameterInfo[] parameters)
        {
            System.Diagnostics.Debug.Assert(arglist.Length <= parameters.Length);

            for (var i = 0; i < arglist.Length; i++)
            {
                TryConvert(ref arglist[i], parameters[i].ParameterType);
            }
        }

        #endregion

        /// <summary>
        /// Determines whether this instance can deduce type args for a generic type from the supplied arguments.
        /// </summary>
        /// <param name="type">The type to be examined.</param>
        /// <param name="arglist">The arglist.</param>
        /// <param name="typeArgsOut">The type args to be used.</param>
        /// <returns>
        /// 	<c>true</c> if this the provided args give sufficient information to determine the type args to be used; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanDeduceTypeArgsFromArgs(Type type, object[] arglist, ref Type[] typeArgsOut)
        {
            Type[] typeParameters = type.GetGenericArguments();

            foreach (ConstructorInfo ctor in type.GetConstructors())
            {
                ParameterInfo[] parameters = ctor.GetParameters();
                if (parameters.Length != arglist.Length)
                    continue;

                Type[] typeArgs = new Type[typeParameters.Length];
                for (int i = 0; i < typeArgs.Length; i++)
                {
                    for (int j = 0; j < arglist.Length; j++)
                    {
                        if (typeParameters[i].IsGenericParameter || parameters[j].ParameterType.Equals(typeParameters[i]))
                            typeArgs[i] = TypeHelper.BestCommonType(
                                typeArgs[i],
                                arglist[j].GetType());
                    }

                    if (typeArgs[i] == null)
                    {
                        typeArgs = null;
                        break;
                    }
                }

                if (typeArgs != null)
                {
                    typeArgsOut = typeArgs;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Return the interfaces implemented by a Type.
        /// </summary>
        /// <param name="type">The Type to be examined.</param>
        /// <returns>An array of Types for the interfaces.</returns>
        public static Type[] GetDeclaredInterfaces(Type type)
        {
            List<Type> interfaces = new List<Type>(type.GetInterfaces());

            if (type.GetTypeInfo().BaseType == typeof(object))
                return interfaces.ToArray();

            List<Type> baseInterfaces = new List<Type>(type.GetTypeInfo().BaseType.GetInterfaces());
            List<Type> declaredInterfaces = new List<Type>();

            foreach (Type interfaceType in interfaces)
            {
                if (!baseInterfaces.Contains(interfaceType))
                    declaredInterfaces.Add(interfaceType);
            }

            return declaredInterfaces.ToArray();
        }

        /// <summary>
        /// Converts an array of objects to the <paramref name="targetType"/>, if it is supported.
        /// </summary>
        public static IEnumerable ConvertData(object[] data, Type targetType)
        {
            if (data.Length == 0)
            {
                if (targetType == typeof(bool))
                {
                    return new object[] { true, false };
                }

                if (targetType.GetTypeInfo().IsEnum)
                {
                    return Enum.GetValues(targetType);
                }
            }

            for (int i = 0; i < data.Length; i++)
            {
                TryConvert(ref data[i], targetType);
            }

            return data;
        }

        /// <summary>
        /// Determines whether the cast to the given type would succeed.
        /// If <paramref name="obj"/> is <see langword="null"/> and <typeparamref name="T"/>
        /// can be <see langword="null"/>, the cast succeeds just like the C# language feature.
        /// </summary>
        /// <param name="obj">The object to cast.</param>
        internal static bool CanCast<T>(object obj)
        {
            return obj is T || (obj == null && default(T) == null);
        }

        /// <summary>
        /// Casts to a value of the given type if possible.
        /// If <paramref name="obj"/> is <see langword="null"/> and <typeparamref name="T"/>
        /// can be <see langword="null"/>, the cast succeeds just like the C# language feature.
        /// </summary>
        /// <param name="obj">The object to cast.</param>
        /// <param name="value">The value of the object, if the cast succeeded.</param>
        internal static bool TryCast<T>(object obj, out T value)
        {
            if (obj is T)
            {
                value = (T)obj;
                return true;
            }

            value = default(T);
            return obj == null && default(T) == null;
        }
    }
}
