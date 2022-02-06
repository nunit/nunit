using System;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework
{
    internal static class TypeGetMemberExtension
    {
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
            var members = new List<MemberInfo>();
            do
            {
                members.AddRange(type.GetMember(name, flags));
            }
            while (members.Count == 0 && (type = type.BaseType) != null);
            return members.ToArray();
        }
    }
}
