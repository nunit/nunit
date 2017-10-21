// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{ 
    /// <summary>
    /// UniqueItemsConstraint tests whether all the items in a 
    /// collection are unique.
    /// </summary>
    public class UniqueItemsConstraint : CollectionItemsEqualConstraint
    {
        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "all items unique"; }
        }

        /// <summary>
        /// Check that all items are unique.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool Matches(IEnumerable actual)
        {
            return TryFastAlgorithm(actual) ?? OriginalAlgorithm(actual);
        }

        private bool OriginalAlgorithm(IEnumerable actual)
        {
            var list = new List<object>();

            foreach (object o1 in actual)
            {
                foreach (object o2 in list)
                    if (ItemsEqual(o1, o2))
                        return false;
                list.Add(o1);
            }

            return true;
        }

        private bool? TryFastAlgorithm(IEnumerable actual)
        {
            // If the user specified any external comparer with Using, exit
            if (UsingExternalComparer)
                return null;

            // If IEnumerable<T> is not implemented exit,
            // Otherwise return value is the Type of T
            Type memberType = GetGenericTypeArgument(actual);
            if (memberType == null)
                return null;

            string methodName = nameof(ItemsUnique);
            bool makeGeneric = true;

            // Special handling for ignore case with strings and chars
            if (IgnoringCase)
            {
                if (memberType == typeof(string))
                {
                    methodName = nameof(StringsUniqueIgnoringCase);
                    makeGeneric = false;
                }
                else
                if (memberType == typeof(char))
                {
                    methodName = nameof(CharsUniqueIgnoringCase);
                    makeGeneric = false;
                }
        }

        MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            if (method == null)
                throw new InvalidOperationException("Internal error - Method not found: " + methodName);
            
            if (makeGeneric)
                method = method.MakeGenericMethod(memberType);

            return (bool)method.Invoke(null, new object[] { actual });
        }

        private static bool ItemsUnique<T>(IEnumerable<T> actual)
        {
            var hash = new HashSet<T>();

            foreach (T item in actual)
            {
                if (hash.Contains(item))
                    return false;

                hash.Add(item);
            }

            return true;
        }

        private static bool StringsUniqueIgnoringCase(IEnumerable<string> actual)
        {
            var hash = new HashSet<string>();

            foreach (string item in actual)
            {
                string s = item.ToLower();
                if (hash.Contains(s))
                    return false;

                hash.Add(s);
            }

            return true;
        }

        private static bool CharsUniqueIgnoringCase(IEnumerable<char> actual)
        {
            var hash = new HashSet<char>();

            foreach (char item in actual)
            {
                char ch = char.ToLower(item);
                if (hash.Contains(ch))
                    return false;

                hash.Add(ch);
            }

            return true;
        }

        private Type GetGenericTypeArgument(IEnumerable actual)
        {
            foreach (var type in actual.GetType().GetInterfaces())
            {
                if (type.FullName.StartsWith("System.Collections.Generic.IEnumerable`1"))
                {
#if NET_2_0 || NET_3_5 || NET_4_0
                    return type.GetGenericArguments()[0];
#else
                    return type.GenericTypeArguments[0];
#endif
                }
            }

            return null;
        }
    }
}