// ***********************************************************************
// Copyright (c) 2007-2012 Charlie Poole, Rob Prouse
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
using System.ComponentModel;
using System.Reflection;
#if !NET20 && !NET35 && !NETSTANDARD1_6
using System.Runtime.ExceptionServices;
#endif
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;

#if NETSTANDARD1_6
using System.Linq;
#endif

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
        private static readonly BindingFlags AllMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        // A zero-length Type array - not provided by System.Type for all CLR versions we support.
        private static readonly Type[] EmptyTypes = new Type[0];

        #region Get Methods of a type

        /// <summary>
        /// Examine a fixture type and return an array of methods having a
        /// particular attribute. The array is order with base methods first.
        /// </summary>
        /// <param name="fixtureType">The type to examine</param>
        /// <param name="attributeType">The attribute Type to look for</param>
        /// <param name="inherit">Specifies whether to search the fixture type inheritance chain</param>
        /// <returns>The array of methods found</returns>
        public static MethodInfo[] GetMethodsWithAttribute(Type fixtureType, Type attributeType, bool inherit)
        {
            List<MethodInfo> list = new List<MethodInfo>();

            var flags = AllMembers | (inherit ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);
            foreach (MethodInfo method in fixtureType.GetMethods(flags))
            {
                if (method.IsDefined(attributeType, inherit))
                    list.Add(method);
            }

            list.Sort(new BaseTypesFirstComparer());

            return list.ToArray();
        }

        private class BaseTypesFirstComparer : IComparer<MethodInfo>
        {
            public int Compare(MethodInfo m1, MethodInfo m2)
            {
                if (m1 == null || m2 == null) return 0;

                Type m1Type = m1.DeclaringType;
                Type m2Type = m2.DeclaringType;

                if ( m1Type == m2Type ) return 0;
                if ( m1Type.IsAssignableFrom(m2Type) ) return -1;

                return 1;
            }
        }

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
            ConstructorInfo ctor = type.GetConstructor(EmptyTypes);
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
        public static object Construct(Type type, object[] arguments)
        {
            if (arguments == null) return Construct(type);

            Type[] argTypes = GetTypeArray(arguments);
            ITypeInfo typeInfo = new TypeWrapper(type);
            ConstructorInfo ctor = typeInfo.GetConstructor(argTypes);
            if (ctor == null)
                throw new InvalidTestFixtureException(type.FullName + " does not have a suitable constructor");

            return ctor.Invoke(arguments);
        }

        /// <summary>
        /// Returns an array of types from an array of objects.
        /// Differs from <see cref="M:System.Type.GetTypeArray(System.Object[])"/> by returning <see cref="NUnitNullType"/>
        /// for null elements rather than throwing <see cref="ArgumentNullException"/>.
        /// </summary>
        internal static Type[] GetTypeArray(object[] objects)
        {
            Type[] types = new Type[objects.Length];
            int index = 0;
            foreach (object o in objects)
            {
                // NUnitNullType is a marker to indicate null since we can't do typeof(null) or null.GetType()
                types[index++] = o == null ? typeof(NUnitNullType) : o.GetType();
            }
            return types;
        }

        #endregion

        #region Invoke Methods

        /// <summary>
        /// Invoke a parameterless method returning void on an object.
        /// </summary>
        /// <param name="method">A MethodInfo for the method to be invoked</param>
        /// <param name="fixture">The object on which to invoke the method</param>
        public static object InvokeMethod( MethodInfo method, object fixture )
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
#if !NET20 && !NET35 && !NETSTANDARD1_6
        [HandleProcessCorruptedStateExceptions] //put here to handle C++ exceptions.
#endif
        public static object InvokeMethod( MethodInfo method, object fixture, params object[] args )
        {
            if(method != null)
            {
                try
                {
                    return method.Invoke(fixture, args);
                }
#if !NETSTANDARD1_6
                catch (System.Threading.ThreadAbortException)
                {
                    // No need to wrap or rethrow ThreadAbortException
                    return null;
                }
#endif
                catch (TargetInvocationException e)
                {
                    throw new NUnitException("Rethrown", e.InnerException);
                }
                catch (Exception e)
                {
                    throw new NUnitException("Rethrown", e);
                }
            }

            return null;
        }

        #endregion

#if NETSTANDARD1_6
        /// <summary>
        /// <para>
        /// Selects the ultimate shadowing property just like <see langword="dynamic"/> would,
        /// rather than throwing <see cref="AmbiguousMatchException"/>
        /// for properties that shadow properties of a different property type
        /// which is what <see cref="TypeInfo.GetProperty(string, BindingFlags)"/> does.
        /// </para>
        /// <para>
        /// If you request both public and nonpublic properties, every public property is preferred
        /// over every nonpublic property. It would violate the principle of least surprise for a
        /// derived class’s implementation detail to be chosen over the public API for a type.
        /// </para>
        /// </summary>
        /// <param name="type">See <see cref="TypeInfo.GetProperty(string, BindingFlags)"/>.</param>
        /// <param name="name">See <see cref="TypeInfo.GetProperty(string, BindingFlags)"/>.</param>
        /// <param name="bindingFlags">See <see cref="TypeInfo.GetProperty(string, BindingFlags)"/>.</param>
#else
        /// <summary>
        /// <para>
        /// Selects the ultimate shadowing property just like <see langword="dynamic"/> would,
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
#endif
        public static PropertyInfo GetUltimateShadowingProperty(Type type, string name, BindingFlags bindingFlags)
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
    }
}
