// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Reflection;
using System.Collections;
#if CLR_2_0
using System.Collections.Generic;
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
	public class Reflect
	{
        private static readonly BindingFlags AllMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        // A zero-length Type array - not provided by System.Type for all CLR versions we support.
        private static readonly Type[] EmptyTypes = new Type[0];

        #region Attributes

        /// <summary>
        /// Get attribute of a given type on a member. If multiple attributes
        /// of a type are present, the first one found is returned.
        /// </summary>
        /// <param name="member">The member to examine</param>
        /// <param name="attributeType">The attribute Type to look for</param>
        /// <param name="inherit">True to include inherited attributes</param>
        /// <returns>The attribute or null</returns>
        public static System.Attribute GetAttribute(ICustomAttributeProvider member, Type attributeType, bool inherit)
        {
            object[] attrs = member.GetCustomAttributes(attributeType, inherit);
            return attrs.Length > 0 ? (Attribute)attrs[0] : null;
        }

        /// <summary>
		/// Get all attributes of a given type on a member.
		/// </summary>
		/// <param name="member">The member to examine</param>
		/// <param name="attribueType">The attribute Type to look for</param>
		/// <param name="inherit">True to include inherited attributes</param>
		/// <returns>The attribute or null</returns>
        public static System.Attribute[] GetAttributes(
            ICustomAttributeProvider member, Type attributeType, bool inherit)
        {
            return (System.Attribute[])member.GetCustomAttributes(attributeType, inherit);
        }

        /// <summary>
        /// Get all attributes on a member.
        /// </summary>
        /// <param name="member">The member to examine</param>
        /// <param name="inherit">True to include inherited attributes</param>
        /// <returns>The attribute or null</returns>
        public static System.Attribute[] GetAttributes(
            ICustomAttributeProvider member, bool inherit)
        {
            object[] attributes = member.GetCustomAttributes(inherit);
            System.Attribute[] result = new System.Attribute[attributes.Length];
            int n = 0;
            foreach (Attribute attribute in attributes)
                result[n++] = attribute;

            return result;
        }

        #endregion

		#region Interfaces

		/// <summary>
		/// Check to see if a type implements a named interface.
		/// </summary>
		/// <param name="fixtureType">The type to examine</param>
		/// <param name="interfaceType">The Type of the interface to check for</param>
		/// <returns>True if the interface is implemented by the type</returns>
        public static bool HasInterface(Type fixtureType, Type interfaceType)
        {
            foreach (Type type in fixtureType.GetInterfaces())
                if (type == interfaceType)
                    return true;
            return false;
        }

		#endregion

        #region Get Constructors for a Type

        /// <summary>
        /// Determines whether the specified type has a constructor that takes the specified arg types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="argTypes">The arg types.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type has such a constructor; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasConstructor(Type type, params Type[] argTypes)
        {
            return GetConstructor(type, argTypes) != null;
        }

        /// <summary>
		/// Find the default constructor on a type
		/// </summary>
		/// <param name="fixtureType"></param>
		/// <returns></returns>
		public static ConstructorInfo GetConstructor( Type fixtureType )
		{
			return fixtureType.GetConstructor( Reflect.EmptyTypes );
		}

		/// <summary>
		/// Find the default constructor on a type
		/// </summary>
		/// <param name="fixtureType"></param>
		/// <returns></returns>
		public static ConstructorInfo GetConstructor( Type fixtureType, Type[] types )
		{
			return fixtureType.GetConstructor( types );
		}

        #endregion

        #region Get Methods of a type

        /// <summary>
        /// Gets the MethodInfo for a named method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The methd name.</param>
        /// <param name="argTypes">The arg types.</param>
        /// <returns>A MethodInfo</returns>
        public static MethodInfo GetMethod(Type type, string name, params Type[] argTypes)
        {
            if (argTypes == null) argTypes = Reflect.EmptyTypes;
            return type.GetMethod(name, argTypes);
        }

        /// <summary>
        /// Gets a method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="argTypes">The arg types.</param>
        /// <returns></returns>
        public static MethodInfo GetMethod(Type type, string name, BindingFlags flags, params Type[] argTypes)
        {
            if (argTypes == null) argTypes = Reflect.EmptyTypes;
            return type.GetMethod(name, flags, null, argTypes, null);
        }

        /// <summary>
        /// Examine a fixture type and return an array of methods having a 
        /// particular attribute. The array is order with base methods first.
        /// </summary>
        /// <param name="fixtureType">The type to examine</param>
        /// <param name="attributeType">The attribute Type to look for</param>
        /// <returns>The array of methods found</returns>
        public static MethodInfo[] GetMethodsWithAttribute(Type fixtureType, Type attributeType, bool inherit)
        {
#if CLR_2_0
            List<MethodInfo> list = new List<MethodInfo>();
#else
            ArrayList list = new ArrayList();
#endif

            foreach (MethodInfo method in GetMethods(fixtureType))
            {
                if (method.IsDefined(attributeType, inherit))
                    list.Add(method);
            }

            list.Sort(new BaseTypesFirstComparer());

            return (MethodInfo[])list.ToArray();
        }

        private static MethodInfo[] GetMethods(Type fixtureType)
        {
            return fixtureType.GetMethods(AllMembers);
        }

#if CLR_2_0
        private class BaseTypesFirstComparer : System.Collections.Generic.IComparer<MethodInfo>
#else
        private class BaseTypesFirstComparer : IComparer
#endif
        {
            #region IComparer Members
#if CLR_2_0
            public int Compare(MethodInfo x, MethodInfo y)
#else
            public int Compare(object x, object y)
#endif
            {
                MethodInfo m1 = x as MethodInfo;
                MethodInfo m2 = y as MethodInfo;

                if (m1 == null || m2 == null) return 0;

                Type m1Type = m1.DeclaringType;
                Type m2Type = m2.DeclaringType;

                if ( m1Type == m2Type ) return 0;
                if ( m1Type.IsAssignableFrom(m2Type) ) return -1;

                return 1;
            }

            #endregion
        }

        /// <summary>
        /// Examine a fixture type and return true if it has a method with
        /// a particular attribute. 
        /// </summary>
        /// <param name="fixtureType">The type to examine</param>
        /// <param name="attributeType">The attribute Type to look for</param>
        /// <returns>True if found, otherwise false</returns>
        public static bool HasMethodWithAttribute(Type fixtureType, Type attributeType, bool inherit)
        {
            foreach (MethodInfo method in GetMethods( fixtureType ))
            {
                if (method.IsDefined(attributeType, inherit))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Examine a fixture type and get a method with a particular name.
        /// In the case of overloads, the first one found is returned.
        /// </summary>
        /// <param name="fixtureType">The type to examine</param>
        /// <param name="methodName">The name of the method</param>
        /// <returns>A MethodInfo or null</returns>
        public static MethodInfo GetNamedMethod(Type fixtureType, string methodName)
        {
            foreach (MethodInfo method in GetMethods( fixtureType ))
            {
                if (method.Name == methodName)
                    return method;
            }

            return null;
        }

        /// <summary>
        /// Examine a fixture type and get a method with a particular name and list
        /// of arguments. In the case of overloads, the first one found is returned.
        /// </summary>
        /// <param name="fixtureType">The type to examine</param>
        /// <param name="methodName">The name of the method</param>
        /// <param name="argTypes">The full names of the argument types to search for</param>
        /// <returns>A MethodInfo or null</returns>
        public static MethodInfo GetNamedMethod(Type fixtureType, string methodName, 
            string[] argTypes)
        {
            foreach (MethodInfo method in GetMethods(fixtureType) )
            {
                if (method.Name == methodName)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == argTypes.Length)
                    {
                        bool match = true;
                        for (int i = 0; i < argTypes.Length; i++)
                            if (parameters[i].ParameterType.FullName != argTypes[i])
                            {
                                match = false;
                                break;
                            }

                        if (match)
                            return method;
                    }
                }
            }

            return null;
        }

        #endregion

		#region Get Properties of a type
		/// <summary>
		/// Examine a type and return a property having a particular attribute.
		/// In the case of multiple methods, the first one found is returned.
		/// </summary>
		/// <param name="fixtureType">The type to examine</param>
		/// <param name="attributeType">The attribute Type to look for</param>
		/// <returns>A PropertyInfo or null</returns>
        public static PropertyInfo GetPropertyWithAttribute(Type fixtureType, Type attributeType)
        {
            foreach (PropertyInfo property in fixtureType.GetProperties(AllMembers))
            {
                if (property.IsDefined(attributeType, true))
                    return property;
            }

            return null;
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
            ConstructorInfo ctor = GetConstructor(type);
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
            ConstructorInfo ctor = GetConstructor(type, argTypes);
            if (ctor == null)
                throw new InvalidTestFixtureException(type.FullName + " does not have a suitable constructor");

            return ctor.Invoke(arguments);
        }

        /// <summary>
        /// Returns an array of types from an array of objects.
        /// Used because the compact framework doesn't support
        /// Type.GetTypeArray()
        /// </summary>
        /// <param name="objects">An array of objects</param>
        /// <returns>An array of Types</returns>
        private static Type[] GetTypeArray(object[] objects)
        {
            Type[] types = new Type[objects.Length];
            int index = 0;
            foreach (object o in objects)
                types[index++] = o.GetType();
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
			return InvokeMethod( method, fixture, null );
		}

		/// <summary>
		/// Invoke a method returning void, converting any TargetInvocationException
		/// to an NUnitException
		/// </summary>
		/// <param name="method">A MethodInfo for the method to be invoked</param>
		/// <param name="fixture">The object on which to invoke the method</param>
		public static object InvokeMethod( MethodInfo method, object fixture, params object[] args )
		{
			if(method != null)
			{
				try
				{
					return method.Invoke( fixture, args );
				}
				catch(Exception e)
				{
                    if (e is TargetInvocationException)
                        throw new NUnitException("Rethrown", e.InnerException);
                    else
                        throw new NUnitException("Rethrown", e);
                }
			}

		    return null;
		}

		#endregion

		#region Private Constructor for static-only class

		private Reflect() { }

		#endregion
	}
}
