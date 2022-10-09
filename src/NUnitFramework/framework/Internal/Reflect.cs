// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Helper methods for inspecting a type by reflection.
    ///
    /// Many of these methods take ICustomAttributeProvider as an
    /// argument to avoid duplication, even though certain attributes can
    /// only appear on specific types of members, like MethodInfo or Type.
    ///
    /// In the case where a type is being examined for the presence of
    /// an attribute, interface or named member, the Reflect methods
    /// operate with the full name of the member being sought. This
    /// removes the necessity of the caller having a reference to the
    /// assembly that defines the item being sought and allows the
    /// NUnit core to inspect assemblies that reference an older
    /// version of the NUnit framework.
    /// </summary>
    public static class Reflect
    {
        internal static readonly BindingFlags AllMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        #region Get Methods of a type

        /// <summary>
        /// Examine a fixture type and return true if it has a method with
        /// a particular attribute.
        /// </summary>
        /// <param name="fixtureType">The type to examine</param>
        /// <param name="attributeType">The attribute Type to look for</param>
        /// <returns>True if found, otherwise false</returns>
        public static bool HasMethodWithAttribute(Type fixtureType, Type attributeType)
        {
            foreach (MethodInfo method in fixtureType.GetMethods(AllMembers | BindingFlags.FlattenHierarchy))
            {
                if (method.IsDefined(attributeType, false))
                    return true;
            }
            return false;
        }

        #endregion

        #region Invoke Constructors

        /// <summary>
        /// Invoke the default constructor on a Type
        /// </summary>
        /// <param name="type">The Type to be constructed</param>
        /// <returns>An instance of the Type</returns>
        public static object Construct(Type type)
        {
            ConstructorInfo? ctor = type.GetConstructor(Array.Empty<Type>());
            if (ctor == null)
                throw new InvalidTestFixtureException(type.FullName + " does not have a default constructor");

            return ctor.Invoke(null);
        }

        /// <summary>
        /// Invoke a constructor on a Type with arguments
        /// </summary>
        /// <param name="type">The Type to be constructed</param>
        /// <param name="arguments">Arguments to the constructor</param>
        /// <returns>An instance of the Type</returns>
        public static object Construct(Type type, object?[]? arguments)
        {
            if (arguments == null) return Construct(type);

            Type?[] argTypes = GetTypeArray(arguments);
            ConstructorInfo? ctor = GetConstructors(type, argTypes).FirstOrDefault();
            if (ctor == null)
                throw new InvalidTestFixtureException(type.FullName + " does not have a suitable constructor");

            return ctor.Invoke(arguments);
        }

        /// <summary>
        /// Returns an array of types from an array of objects.
        /// Differs from <see cref="M:System.Type.GetTypeArray(System.Object[])"/> by returning <see langword="null"/>
        /// for null elements rather than throwing <see cref="ArgumentNullException"/>.
        /// </summary>
        internal static Type?[] GetTypeArray(object?[] objects)
        {
            Type?[] types = new Type?[objects.Length];
            int index = 0;
            foreach (object? o in objects)
            {
                types[index++] = o?.GetType();
            }
            return types;
        }

        /// <summary>
        /// Gets the constructors to which the specified argument types can be coerced.
        /// </summary>
        internal static IEnumerable<ConstructorInfo> GetConstructors(Type type, Type?[] matchingTypes)
        {
            return type
                .GetConstructors()
                .Where(c => c.GetParameters().ParametersMatch(matchingTypes));
        }

        /// <summary>
        /// Determines if the given types can be coerced to match the given parameters.
        /// </summary>
        internal static bool ParametersMatch(this ParameterInfo[] pinfos, Type?[] ptypes)
        {
            if (pinfos.Length != ptypes.Length)
                return false;

            for (int i = 0; i < pinfos.Length; i++)
            {
                if (!ptypes[i].CanImplicitlyConvertTo(pinfos[i].ParameterType))
                    return false;
            }
            return true;
        }

        // §6.1.2 (Implicit numeric conversions) of the specification
        private static readonly Dictionary<Type, List<Type>> convertibleValueTypes = new Dictionary<Type, List<Type>>() {
            { typeof(decimal), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char) } },
            { typeof(double), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char), typeof(float) } },
            { typeof(float), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char), typeof(float) } },
            { typeof(ulong), new List<Type> { typeof(byte), typeof(ushort), typeof(uint), typeof(char) } },
            { typeof(long), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(char) } },
            { typeof(uint), new List<Type> { typeof(byte), typeof(ushort), typeof(char) } },
            { typeof(int), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(char) } },
            { typeof(ushort), new List<Type> { typeof(byte), typeof(char) } },
            { typeof(short), new List<Type> { typeof(byte) } }
        };

        /// <summary>
        /// Determines whether the current type can be implicitly converted to the specified type.
        /// </summary>
        internal static bool CanImplicitlyConvertTo(this Type? from, Type to)
        {
            if (to.IsAssignableFrom(from))
                return true;

            // Look for the marker that indicates from was null
            if (from == null)
            {
                // Look for the marker that indicates from was null
                return to.GetTypeInfo().IsClass || to.FullName.StartsWith("System.Nullable", StringComparison.Ordinal);
            }

            if (convertibleValueTypes.TryGetValue(to, out var types) && types.Contains(from))
                return true;

            return from
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Any(m => m.ReturnType == to && m.Name == "op_Implicit");
        }

        #endregion

        #region Invoke Methods

        /// <summary>
        /// Invoke a parameterless method returning void on an object.
        /// </summary>
        /// <param name="method">A MethodInfo for the method to be invoked</param>
        /// <param name="fixture">The object on which to invoke the method</param>
        public static object? InvokeMethod(MethodInfo method, object? fixture)
        {
            return InvokeMethod(method, fixture, null);
        }

        /// <summary>
        /// Invoke a method, converting any TargetInvocationException to an NUnitException.
        /// </summary>
        /// <param name="method">A MethodInfo for the method to be invoked</param>
        /// <param name="fixture">The object on which to invoke the method</param>
        /// <param name="args">The argument list for the method</param>
        /// <returns>The return value from the invoked method</returns>
        [HandleProcessCorruptedStateExceptions] //put here to handle C++ exceptions.
        public static object? InvokeMethod(MethodInfo method, object? fixture, params object?[]? args)
        {
            try
            {
                return method.Invoke(fixture, args);
            }
            catch (TargetInvocationException e)
            {
                throw new NUnitException("Rethrown", e.InnerException);
            }
            catch (Exception e)
#if THREAD_ABORT
                // If ThreadAbortException is caught, it must be rethrown or else Mono 5.18.1
                // will not rethrow at the end of the catch block. Instead, it will resurrect
                // the ThreadAbortException at the end of the next unrelated catch block that
                // executes on the same thread after handling an unrelated exception.
                // The end result is that an unrelated test will error with the message "Test
                // cancelled by user."

                // This is just cleaner than catching and rethrowing:
                when (e is not System.Threading.ThreadAbortException)
#endif
            {
                throw new NUnitException("Rethrown", e);
            }
        }

        #endregion

        /// <summary>
        /// <para>
        /// Selects the ultimate shadowing property just like <c>dynamic</c> would,
        /// rather than throwing <see cref="AmbiguousMatchException"/>
        /// for properties that shadow properties of a different property type
        /// which is what <see cref="Type.GetProperty(string, BindingFlags)"/> does.
        /// </para>
        /// <para>
        /// If you request both public and nonpublic properties, every public property is preferred
        /// over every nonpublic property. It would violate the principle of least surprise for a
        /// derived class’s implementation detail to be chosen over the public API for a type.
        /// </para>
        /// </summary>
        /// <param name="type">See <see cref="Type.GetProperty(string, BindingFlags)"/>.</param>
        /// <param name="name">See <see cref="Type.GetProperty(string, BindingFlags)"/>.</param>
        /// <param name="bindingFlags">See <see cref="Type.GetProperty(string, BindingFlags)"/>.</param>
        public static PropertyInfo? GetUltimateShadowingProperty(Type type, string name, BindingFlags bindingFlags)
        {
            Guard.ArgumentNotNull(type, nameof(type));
            Guard.ArgumentNotNull(name, nameof(name));

            if ((bindingFlags & BindingFlags.DeclaredOnly) != 0)
            {
                // If you're asking us to search a hierarchy but only want properties declared in the given type,
                // you're in the wrong place but okay:
                return type.GetProperty(name, bindingFlags);
            }

            if ((bindingFlags & (BindingFlags.Public | BindingFlags.NonPublic)) == (BindingFlags.Public | BindingFlags.NonPublic))
            {
                // If we're searching for both public and nonpublic properties, search for only public first
                // because chances are if there is a public property, it would be very surprising to detect the private shadowing property.

                for (var publicSearchType = type; publicSearchType != null; publicSearchType = publicSearchType.GetTypeInfo().BaseType)
                {
                    var property = publicSearchType.GetProperty(name, (bindingFlags | BindingFlags.DeclaredOnly) & ~BindingFlags.NonPublic);
                    if (property != null) return property;
                }

                // There is no public property, so may as well not ask to include them during the second search.
                bindingFlags &= ~BindingFlags.Public;
            }

            for (var searchType = type; searchType != null; searchType = searchType.GetTypeInfo().BaseType)
            {
                var property = searchType.GetProperty(name, bindingFlags | BindingFlags.DeclaredOnly);
                if (property != null) return property;
            }

            return null;
        }

        internal static bool IsAssignableFromNull(Type type)
        {
            Guard.ArgumentNotNull(type, nameof(type));
            return !type.GetTypeInfo().IsValueType || IsNullable(type);
        }

        private static bool IsNullable(Type type)
        {
            // Compare with https://github.com/dotnet/coreclr/blob/bb01fb0d954c957a36f3f8c7aad19657afc2ceda/src/mscorlib/src/System/Nullable.cs#L152-L157
            return type.GetTypeInfo().IsGenericType
                && !type.GetTypeInfo().IsGenericTypeDefinition
                && ReferenceEquals(type.GetGenericTypeDefinition(), typeof(Nullable<>));
        }

        internal static IEnumerable<Type> TypeAndBaseTypes(this Type type)
        {
            for (; type != null; type = type.GetTypeInfo().BaseType)
            {
                yield return type;
            }
        }

        /// <summary>
        /// Same as <c>GetMethod(<paramref name="name"/>, <see cref="BindingFlags.Public"/> |
        /// <see cref="BindingFlags.Instance"/>, <see langword="null"/>, <paramref name="parameterTypes"/>,
        /// <see langword="null"/>)</c> except that it also chooses only non-generic methods.
        /// Useful for avoiding the <see cref="AmbiguousMatchException"/> you can have with <c>GetMethod</c>.
        /// </summary>
        internal static MethodInfo? GetNonGenericPublicInstanceMethod(this Type type, string name, Type[] parameterTypes)
        {
            foreach (var currentType in type.TypeAndBaseTypes())
            {
                var method = currentType
                   .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                   .SingleOrDefault(candidate =>
                   {
                       if (candidate.Name != name || candidate.GetGenericArguments().Length != 0) return false;

                       var parameters = candidate.GetParameters();
                       if (parameters.Length != parameterTypes.Length) return false;

                       for (var i = 0; i < parameterTypes.Length; i++)
                           if (parameters[i].ParameterType != parameterTypes[i])
                               return false;

                       return true;
                   });

                if (method != null) return method;
            }

            return null;
        }

        internal static PropertyInfo? GetPublicInstanceProperty(this Type type, string name, Type[] indexParameterTypes)
        {
            for (var currentType = type; currentType != null; currentType = currentType.GetTypeInfo().BaseType)
            {
                var property = currentType
                     .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .SingleOrDefault(candidate =>
                     {
                         if (candidate.Name != name) return false;

                         var indexParameters = candidate.GetIndexParameters();
                         if (indexParameters.Length != indexParameterTypes.Length) return false;

                         for (var i = 0; i < indexParameterTypes.Length; i++)
                             if (indexParameters[i].ParameterType != indexParameterTypes[i]) return false;

                         return true;
                     });

                if (property != null) return property;
            }

            return null;
        }

        internal static object? InvokeWithTransparentExceptions(this MethodBase methodBase, object? instance)
        {
            // If we ever target .NET Core 2.1, we can keep from mucking with the exception stack trace
            // using BindingFlags.DoNotWrapExceptions rather than try…catch.

            try
            {
                return methodBase.Invoke(instance, null);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionHelper.Rethrow(ex.InnerException!);

                // If this line is reached, ExceptionHelper.Rethrow is very broken.
                throw new InvalidOperationException("ExceptionHelper.Rethrow failed to throw an exception.");
            }
        }

        internal static object? DynamicInvokeWithTransparentExceptions(this Delegate @delegate)
        {
            try
            {
                return @delegate.DynamicInvoke();
            }
            catch (TargetInvocationException ex)
            {
                ExceptionHelper.Rethrow(ex.InnerException!);

                // If this line is reached, ExceptionHelper.Rethrow is very broken.
                throw new InvalidOperationException("ExceptionHelper.Rethrow failed to throw an exception.");
            }
        }

        internal static bool IsFSharpOption(this Type type, [NotNullWhen(true)] out Type? someType)
        {
            Guard.ArgumentNotNull(type, nameof(type));

            if (type.GetTypeInfo().IsGenericType
                && type.GetGenericTypeDefinition().FullName == "Microsoft.FSharp.Core.FSharpOption`1")
            {
                someType = type.GetGenericArguments()[0];
                return true;
            }

            someType = null;
            return false;
        }

        internal static bool IsVoidOrUnit(Type type)
        {
            Guard.ArgumentNotNull(type, nameof(type));

            return type == typeof(void) || type.FullName == "Microsoft.FSharp.Core.Unit";
        }

        /// <summary>
        /// Returns the get accessor for the indexer.
        /// </summary>
        /// <param name="type">Type to reflect on for the indexer.</param>
        /// <param name="indexerTypes">List of indexer types that matches the indexer type order.</param>
        /// <returns>The Get accessor</returns>
        public static MethodInfo? GetDefaultIndexer(Type type, Type[] indexerTypes)
        {
            const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            var indexerName = GetIndexerName(type);
            var indexer = type.GetProperty(indexerName, bindingFlags, null, null, indexerTypes, null);

            return indexer?.GetGetMethod(true);
        }

        private static string GetIndexerName(Type type)
        {
            const string defaultFrameworkIndexerName = "Item";
            var defaultMemberAttribute = type.GetAttributes<DefaultMemberAttribute>(true).FirstOrDefault();

            return defaultMemberAttribute?.MemberName ?? defaultFrameworkIndexerName;
        }

        /// <summary>
        /// Searches for the specified members, using the specified binding constraints.
        /// </summary>
        /// <remarks>
        /// Similar to <see cref="Type.GetMember(string, BindingFlags)"/> but also finds private static members from the base class.
        /// </remarks>
        /// <param name="type">The type to find the members from.</param>
        /// <param name="name">The string containing the name of the members to get.</param>
        /// <param name="flags">A bitwise combination of the enumeration values that specify how the search is conducted.</param>
        /// <returns></returns>
        public static MemberInfo[] GetMemberIncludingFromBase(this Type type, string name, BindingFlags flags)
        {
            Type? currentType = type;
            MemberInfo[] members;
            do
            {
                members = currentType.GetMember(name, flags);
            }
            while (members.Length == 0 && (currentType = currentType.BaseType) != null);
            return members;
        }
    }
}
