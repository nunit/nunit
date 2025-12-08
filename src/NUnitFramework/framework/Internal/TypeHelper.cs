// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TypeHelper provides static methods that operate on Types.
    /// </summary>
    public static class TypeHelper
    {
        private const int StringMax = 40;

        /// <param name="type">The type to evaluate.</param>
        extension(Type type)
        {
            /// <summary>
            /// Determines whether the specified type represents a ValueTuple.
            /// </summary>
            /// <returns><see langword="true"/> if the type is a <see cref="ValueTuple"/>; otherwise, <see langword="false"/>.</returns>
            public bool IsValueTuple()
            {
                return type.IsTupleInternal("System.ValueTuple");
            }

            /// <summary>
            /// Determines whether the specified type represents a Tuple.
            /// </summary>
            /// <returns><see langword="true"/> if the type is a <see cref="Tuple"/>; otherwise, <see langword="false"/>.</returns>
            public bool IsTuple()
            {
                return type.IsTupleInternal("System.Tuple");
            }

            private bool IsTupleInternal(string tupleName)
            {
                string typeName = type.FullName();

                if (typeName.EndsWith("[]", StringComparison.Ordinal))
                    return false;

                string typeNameWithoutGenerics = GetTypeNameWithoutGenerics(typeName);
                return typeNameWithoutGenerics == tupleName;
            }

            /// <summary>
            /// Determines whether the specified type is numeric.
            /// </summary>
            /// <returns>
            /// <see langword="true"/> if the specified type is numeric; otherwise, <see langword="false"/>.
            /// </returns>
            public bool IsNumeric() =>
                type == typeof(double) ||
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

            /// <summary>
            /// Gets the display name for a Type as used by NUnit.
            /// </summary>
            /// <returns>The display name for the Type</returns>
            public string GetDisplayName()
            {
                if (type.IsGenericParameter)
                    return type.Name;

                if (type.IsGenericType)
                {
                    string name = type.FullName();
                    int index = name.IndexOf('[');
                    if (index >= 0)
                        name = name.Substring(0, index);

                    index = name.LastIndexOf('.');
                    if (index >= 0)
                        name = name.Substring(index + 1);

                    var genericArguments = type.GetGenericArguments();
                    var currentArgument = 0;

                    StringBuilder sb = new StringBuilder();

                    bool firstClassSeen = false;
                    foreach (string nestedClass in name.Tokenize('+'))
                    {
                        if (firstClassSeen)
                            sb.Append('+');

                        firstClassSeen = true;

                        index = nestedClass.IndexOf('`');
                        if (index >= 0)
                        {
                            var nestedClassName = nestedClass.Substring(0, index);
                            sb.Append(nestedClassName);
                            sb.Append('<');

                            var argumentCount = Int32.Parse(nestedClass.Substring(index + 1));
                            for (int i = 0; i < argumentCount; i++)
                            {
                                if (i > 0)
                                    sb.Append(',');

                                sb.Append(genericArguments[currentArgument++].GetDisplayName());
                            }
                            sb.Append('>');
                        }
                        else
                        {
                            sb.Append(nestedClass);
                        }
                    }

                    return sb.ToString();
                }

                string typeFullName = type.FullName();
                int lastdot = typeFullName.LastIndexOf('.');
                return lastdot >= 0
                    ? typeFullName.Substring(lastdot + 1)
                    : typeFullName;
            }

            /// <summary>
            /// Return the interfaces implemented by a Type.
            /// </summary>
            /// <returns>An array of Types for the interfaces.</returns>
            public Type[] GetDeclaredInterfaces()
            {
                List<Type> interfaces = new(type.GetInterfaces());

                if (type.BaseType is null || type.BaseType == typeof(object))
                    return interfaces.ToArray();

                List<Type> baseInterfaces = new(type.BaseType.GetInterfaces());
                List<Type> declaredInterfaces = new();

                foreach (Type interfaceType in interfaces)
                {
                    if (!baseInterfaces.Contains(interfaceType))
                        declaredInterfaces.Add(interfaceType);
                }

                return declaredInterfaces.ToArray();
            }

            /// <summary>
            /// Determines whether this instance can deduce type args for a generic type from the supplied arguments.
            /// </summary>
            /// <param name="arglist">The arglist.</param>
            /// <param name="typeArgsOut">The type args to be used.</param>
            /// <returns>
            /// <see langword="true"/> if this the provided args give sufficient information to determine the type args to be used; otherwise, <see langword="false"/>.
            /// </returns>
            public bool CanDeduceTypeArgsFromArgs(object?[] arglist, [NotNullWhen(true)] ref Type[]? typeArgsOut)
            {
                Type[] typeParameters = type.GetGenericArguments();

                foreach (ConstructorInfo ctor in type.GetConstructors())
                {
                    ParameterInfo[] parameters = ctor.GetParameters();
                    if (parameters.Length != arglist.Length)
                        continue;

                    var typeArgs = new Type?[typeParameters.Length];
                    for (int i = 0; i < typeArgs.Length; i++)
                    {
                        for (int j = 0; j < arglist.Length; j++)
                        {
                            if (parameters[j].ParameterType.Equals(typeParameters[i]) &&
                                !TryGetBestCommonType(
                                    typeArgs[i],
                                    arglist[j]?.GetType(),
                                    out typeArgs[i]))
                            {
                                typeArgs[i] = null;
                                break;
                            }
                        }

                        if (typeArgs[i] is null)
                        {
                            typeArgs = null;
                            break;
                        }
                    }

                    if (typeArgs is not null)
                    {
                        typeArgsOut = typeArgs!;
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Gets the display name for a Type as used by NUnit.
            /// </summary>
            /// <param name="arglist">The arglist provided.</param>
            /// <returns>The display name for the Type</returns>
            public string GetDisplayName(object?[]? arglist)
            {
                string baseName = type.GetDisplayName();
                if (arglist is null || arglist.Length == 0)
                    return baseName;

                StringBuilder sb = new StringBuilder(baseName);

                sb.Append('(');
                sb.Append(DisplayName.GetValueString(arglist[0], StringMax));

                for (int i = 1; i < arglist.Length; i++)
                {
                    sb.Append(',');
                    sb.Append(DisplayName.GetValueString(arglist[i], StringMax));
                }

                sb.Append(')');

                return sb.ToString();
            }

            /// <summary>
            /// For the given type, find the primary <see cref="IEnumerable{T}"/> interface declared
            /// on it and return its generic type argument. If multiple <see cref="IEnumerable{T}"/> interfaces are declared,
            /// return <see langword="null"/>.
            /// </summary>
            /// <returns>
            /// The generic type argument of the primary <see cref="IEnumerable{T}"/> interface declared on the given
            /// type, or <see langword="null"/> if zero interfaces are found or more than one interface is found.
            /// </returns>
            public Type? FindPrimaryEnumerableInterfaceGenericTypeArgument()
            {
                static bool Predicate(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);

                Type? primaryEnumerableType = null;
                var found = false;
                var queue = new Queue<Type>();
                queue.Enqueue(type);

                // We need to scan interfaces declared on interfaces recursively here because the type
                // might only implement `IList<T>`, which technically isn't an `IEnumerable<T>` and would
                // cause a basic interface scan to fail.
                while (queue.Count > 0)
                {
                    var @interface = queue.Dequeue();

                    // if we run into multiple `IEnumerable<T>` that have the same T generic argument then that's fine,
                    // i.e. if the primary enumerable type is equal to the interface type we can skip this check.
                    if (primaryEnumerableType != @interface && Predicate(@interface))
                    {
                        if (found)
                        {
                            return null;
                        }
                        primaryEnumerableType = @interface;
                        found = true;
                    }

                    foreach (var subInterface in @interface.GetInterfaces())
                    {
                        queue.Enqueue(subInterface);
                    }
                }

                return primaryEnumerableType?.GenericTypeArguments[0];
            }

            /// <summary>
            /// Gets the <see cref="Type.FullName"/> if available.
            /// </summary>
            /// <returns><see cref="Type.FullName"/> if available, throws otherwise.</returns>
            /// <exception cref="InvalidOperationException">If <see cref="Type.FullName"/> returns <see langword="null"/>.</exception>
            internal string FullName() => type.FullName ?? throw new InvalidOperationException("No name for type: " + type);

            internal bool HasCompilerGeneratedEquals()
            {
                if (type.HasAttribute<CompilerGeneratedAttribute>(false))
                    return true;

                var equalsMethod = type.GetMethod(nameof(type.Equals), BindingFlags.Instance | BindingFlags.Public,
                    null, [type], null);

                return equalsMethod?.HasAttribute<CompilerGeneratedAttribute>(false) is true;
            }

            internal bool OverridesEqualsObject()
            {
                // Check for Equals(object) override
                var equalsObject = type.GetMethod(nameof(type.Equals), BindingFlags.Instance | BindingFlags.Public,
                    null, EqualsObjectParameterTypes, null);
                return equalsObject is not null && equalsObject.DeclaringType != (type.IsValueType ? typeof(ValueType) : typeof(object));
            }
        }

        /// <summary>
        /// Returns the best fit for a common type to be used in
        /// matching actual arguments to a methods Type parameters.
        /// </summary>
        public static bool TryGetBestCommonType(Type? type1, Type? type2, [NotNullIfNotNull("type1"), NotNullIfNotNull("type2")] out Type? bestCommonType)
        {
#pragma warning disable SA1107 // Code should not contain multiple statements on one line
#pragma warning disable SA1501 // Statement should not be on a single line
            if (type1 == type2) { bestCommonType = type1; return true; }
            if (type1 is null) { bestCommonType = type2; return true; }
            if (type2 is null) { bestCommonType = type1; return true; }

            if (type1.IsNumeric() && type2.IsNumeric())
            {
                if (type1 == typeof(double)) { bestCommonType = type1; return true; }
                if (type2 == typeof(double)) { bestCommonType = type2; return true; }

                if (type1 == typeof(float)) { bestCommonType = type1; return true; }
                if (type2 == typeof(float)) { bestCommonType = type2; return true; }

                if (type1 == typeof(decimal)) { bestCommonType = type1; return true; }
                if (type2 == typeof(decimal)) { bestCommonType = type2; return true; }

                if (type1 == typeof(UInt64)) { bestCommonType = type1; return true; }
                if (type2 == typeof(UInt64)) { bestCommonType = type2; return true; }

                if (type1 == typeof(Int64)) { bestCommonType = type1; return true; }
                if (type2 == typeof(Int64)) { bestCommonType = type2; return true; }

                if (type1 == typeof(UInt32)) { bestCommonType = type1; return true; }
                if (type2 == typeof(UInt32)) { bestCommonType = type2; return true; }

                if (type1 == typeof(Int32)) { bestCommonType = type1; return true; }
                if (type2 == typeof(Int32)) { bestCommonType = type2; return true; }

                if (type1 == typeof(UInt16)) { bestCommonType = type1; return true; }
                if (type2 == typeof(UInt16)) { bestCommonType = type2; return true; }

                if (type1 == typeof(Int16)) { bestCommonType = type1; return true; }
                if (type2 == typeof(Int16)) { bestCommonType = type2; return true; }

                if (type1 == typeof(byte)) { bestCommonType = type1; return true; }
                if (type2 == typeof(byte)) { bestCommonType = type2; return true; }

                if (type1 == typeof(sbyte)) { bestCommonType = type1; return true; }
                if (type2 == typeof(sbyte)) { bestCommonType = type2; return true; }
            }

            if (type1.IsAssignableFrom(type2)) { bestCommonType = type1; return true; }
            if (type2.IsAssignableFrom(type1)) { bestCommonType = type2; return true; }

            bestCommonType = typeof(object);
            return false;
#pragma warning restore SA1501 // Statement should not be on a single line
#pragma warning restore SA1107 // Code should not contain multiple statements on one line
        }

        /// <summary>
        /// Convert an argument list to the required parameter types.
        /// Currently, only widening numeric conversions are performed.
        /// </summary>
        /// <param name="arglist">An array of args to be converted</param>
        /// <param name="parameters">A ParameterInfo[] whose types will be used as targets</param>
        public static void ConvertArgumentList(object?[] arglist, IParameterInfo[] parameters)
        {
            System.Diagnostics.Debug.Assert(arglist.Length <= parameters.Length);

            for (int i = 0; i < arglist.Length; i++)
            {
                object? arg = arglist[i];

                if (arg is IConvertible)
                {
                    Type argType = arg.GetType();
                    Type targetType = parameters[i].ParameterType;
                    bool convert = false;

                    if (argType != targetType && argType.IsNumeric() && targetType.IsNumeric())
                    {
                        if (targetType == typeof(double) || targetType == typeof(float))
                            convert = arg is int or long or short or byte or sbyte;
                        else if (targetType == typeof(long))
                            convert = arg is int or short or byte or sbyte;
                        else if (targetType == typeof(short))
                            convert = arg is byte or sbyte;
                    }

                    if (convert)
                    {
                        arglist[i] = Convert.ChangeType(arg, targetType, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }
        }

        private static string GetTypeNameWithoutGenerics(string fullTypeName)
        {
            int index = fullTypeName.IndexOf('`');
            return index == -1 ? fullTypeName : fullTypeName.Substring(0, index);
        }

        /// <summary>
        /// Determines whether the cast to the given type would succeed.
        /// If <paramref name="obj"/> is <see langword="null"/> and <typeparamref name="T"/>
        /// can be <see langword="null"/>, the cast succeeds just like the C# language feature.
        /// </summary>
        /// <param name="obj">The object to cast.</param>
        internal static bool CanCast<T>(object? obj)
        {
            return obj is T || (obj is null && default(T) is null);
        }

        /// <summary>
        /// Casts to a value of the given type if possible.
        /// If <paramref name="obj"/> is <see langword="null"/> and <typeparamref name="T"/>
        /// can be <see langword="null"/>, the cast succeeds just like the C# language feature.
        /// </summary>
        /// <param name="obj">The object to cast.</param>
        /// <param name="value">The value of the object, if the cast succeeded.</param>
        internal static bool TryCast<T>(object? obj, [NotNullWhen(true)] out T? value)
        {
            if (obj is T tObj)
            {
                value = tObj;
                return true;
            }

            value = default(T);
            return obj is null && default(T) is null;
        }

        private static readonly Type[] EqualsObjectParameterTypes = { typeof(object) };
    }
}
