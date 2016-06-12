// ***********************************************************************
// Copyright (c) 2008-2015 Charlie Poole
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
#if NETCF || PORTABLE
using System.Linq;
#endif
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
        public static readonly Type NonmatchingType = typeof( NonmatchingTypeClass );

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
                if (index >= 0) name = name.Substring(index+1);

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
                ? type.FullName.Substring(lastdot+1)
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

            StringBuilder sb = new StringBuilder( baseName );

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
            if ( type1 == TypeHelper.NonmatchingType ) return TypeHelper.NonmatchingType;
            if ( type2 == TypeHelper.NonmatchingType ) return TypeHelper.NonmatchingType;

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

            if ( type1.IsAssignableFrom( type2 ) ) return type1;
            if ( type2.IsAssignableFrom( type1 ) ) return type2;

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
            return type == typeof(double) ||
                    type == typeof(float) ||
                    type == typeof(decimal) ||
                    type == typeof(Int64) ||
                    type == typeof(Int32) ||
                    type == typeof(Int16) ||
                    type == typeof(UInt64) ||
                    type == typeof(UInt32) ||
                    type == typeof(UInt16) ||
                    type == typeof(byte) ||
                    type == typeof(sbyte);
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

            for (int i = 0; i < arglist.Length; i++)
            {
                object arg = arglist[i];

#if PORTABLE
                if (arg != null)
#else
                if (arg != null && arg is IConvertible)
#endif
                {
                    Type argType = arg.GetType();
                    Type targetType = parameters[i].ParameterType;
                    bool convert = false;

                    if (argType != targetType && !argType.IsAssignableFrom(targetType))
                    {
                        if (IsNumeric(argType) && IsNumeric(targetType))
                        {
                            if (targetType == typeof(double) || targetType == typeof(float))
                                convert = arg is int || arg is long || arg is short || arg is byte || arg is sbyte;
                            else
                                if (targetType == typeof(long))
                                    convert = arg is int || arg is short || arg is byte || arg is sbyte;
                                else
                                    if (targetType == typeof(short))
                                        convert = arg is byte || arg is sbyte;
                        }
                    }

                    if (convert)
                        arglist[i] = Convert.ChangeType(arg, targetType,
                            System.Globalization.CultureInfo.InvariantCulture);
                }
            }
        }

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

#if NETCF || PORTABLE
            Type[] argTypes = arglist.Select(a => a == null ? typeof(object) : a.GetType()).ToArray();
            if (argTypes.Length != typeParameters.Length || argTypes.Any(at => at.GetTypeInfo().IsGenericType))
                return false;
            try
            {
                type = type.MakeGenericType(argTypes);
            }
            catch (Exception)
            {
                return false;
            }
#endif

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
        /// Gets the _values for an enumeration, using Enum.GetTypes
        /// where available, otherwise through reflection.
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static Array GetEnumValues(Type enumType)
        {
#if NETCF || SILVERLIGHT
            FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            Array enumValues = Array.CreateInstance(enumType, fields.Length);

            for (int index = 0; index < fields.Length; index++)
                enumValues.SetValue(fields[index].GetValue(enumType), index);

            return enumValues;
#else
            return Enum.GetValues(enumType);
#endif
        }

        /// <summary>
        /// Gets the ids of the _values for an enumeration, 
        /// using Enum.GetNames where available, otherwise
        /// through reflection.
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string[] GetEnumNames(Type enumType)
        {
#if NETCF || SILVERLIGHT
            FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            string[] names = new string[fields.Length];

            for (int index = 0; index < fields.Length; index++)
                names[index] =  fields[index].Name;

            return names;
#else
            return Enum.GetNames(enumType);
#endif
        }
    }
}
