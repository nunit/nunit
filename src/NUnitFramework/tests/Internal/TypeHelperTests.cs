// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class TypeHelperTests
    {
        #region BestCommonType

        [TestCase(typeof(TypeHelper.NonmatchingTypeClass), typeof(object), ExpectedResult = typeof(TypeHelper.NonmatchingTypeClass))]
        [TestCase(typeof(object), typeof(TypeHelper.NonmatchingTypeClass), ExpectedResult = typeof(TypeHelper.NonmatchingTypeClass))]
        [TestCase(typeof(A), typeof(B), ExpectedResult = typeof(A))]
        [TestCase(typeof(B), typeof(A), ExpectedResult = typeof(A))]
        [TestCase(typeof(A), typeof(string), ExpectedResult = typeof(TypeHelper.NonmatchingTypeClass))]
        [TestCase(typeof(int[]), typeof(IEnumerable<int>), ExpectedResult=typeof(IEnumerable<int>))]
        public Type BestCommonTypeTest(Type type1, Type type2)
        {
            return TypeHelper.BestCommonType(type1, type2);
        }

        public class A
        {
        }

        public class B : A
        {
        }

        #endregion

        #region GetDisplayName

        [TestCase(typeof(int), ExpectedResult = "Int32")]
        [TestCase(typeof(TypeHelperTests), ExpectedResult = "TypeHelperTests")]
        [TestCase(typeof(A), ExpectedResult = "TypeHelperTests+A")]
        [TestCase(typeof(int[]), ExpectedResult = "Int32[]")]
        [TestCase(typeof(List<int>), ExpectedResult="List<Int32>")]
        [TestCase(typeof(IList<string>), ExpectedResult = "IList<String>")]
        [TestCase(typeof(Dictionary<string, object>), ExpectedResult = "Dictionary<String,Object>")]
        [TestCase(typeof(C<string, long>), ExpectedResult = "TypeHelperTests+C<String,Int64>")]
        [TestCase(typeof(C<List<char[]>, long>), ExpectedResult = "TypeHelperTests+C<List<Char[]>,Int64>")]
        [TestCase(typeof(C<List<char[]>, long>.D<IDictionary<int,byte[]>,string>), ExpectedResult = "TypeHelperTests+C<List<Char[]>,Int64>+D<IDictionary<Int32,Byte[]>,String>")]
#if !NETCF // No Open Generics in CF
        [TestCase(typeof(List<>), ExpectedResult = "List<T>")]
        [TestCase(typeof(IList<>), ExpectedResult = "IList<T>")]
        [TestCase(typeof(Dictionary<,>), ExpectedResult = "Dictionary<TKey,TValue>")]
        [TestCase(typeof(C<,>), ExpectedResult = "TypeHelperTests+C<T1,T2>")]
        [TestCase(typeof(C<,>.D<,>), ExpectedResult = "TypeHelperTests+C<T1,T2>+D<T3,T4>")]
#endif
        public string GetDisplayNameTests(Type type)
        {
            return TypeHelper.GetDisplayName(type);
        }

        public class C<T1,T2>
        {
            public class D<T3, T4>
            {
            }
        }

        #endregion
    }
}
