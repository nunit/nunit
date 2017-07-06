// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
        [TestCase(typeof(int[]), typeof(IEnumerable<int>), ExpectedResult = typeof(IEnumerable<int>))]
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
        [TestCase(typeof(List<int>), ExpectedResult = "List<Int32>")]
        [TestCase(typeof(IList<string>), ExpectedResult = "IList<String>")]
        [TestCase(typeof(Dictionary<string, object>), ExpectedResult = "Dictionary<String,Object>")]
        [TestCase(typeof(C<string, long>), ExpectedResult = "TypeHelperTests+C<String,Int64>")]
        [TestCase(typeof(C<List<char[]>, long>), ExpectedResult = "TypeHelperTests+C<List<Char[]>,Int64>")]
        [TestCase(typeof(C<List<char[]>, long>.D<IDictionary<int, byte[]>, string>), ExpectedResult = "TypeHelperTests+C<List<Char[]>,Int64>+D<IDictionary<Int32,Byte[]>,String>")]
        [TestCase(typeof(List<>), ExpectedResult = "List<T>")]
        [TestCase(typeof(IList<>), ExpectedResult = "IList<T>")]
        [TestCase(typeof(Dictionary<,>), ExpectedResult = "Dictionary<TKey,TValue>")]
        [TestCase(typeof(C<,>), ExpectedResult = "TypeHelperTests+C<T1,T2>")]
        [TestCase(typeof(C<,>.D<,>), ExpectedResult = "TypeHelperTests+C<T1,T2>+D<T3,T4>")]
        public string GetDisplayNameTests(Type type)
        {
            return TypeHelper.GetDisplayName(type);
        }

        public class C<T1, T2>
        {
            public class D<T3, T4> { }
        }

        #endregion

        #region TryConvert method

        private class ConvertBaseType { }
        private class ConvertChildType : ConvertBaseType { }

        private static object FailedConversion = new object();
        private static ConvertBaseType ConvertBase = new ConvertBaseType();
        private static ConvertChildType ConvertChild = new ConvertChildType();
        private static IEnumerable<TestCaseData> TypeConversions = new List<TestCaseData>()
        {
            // Nulls (and DBNull.Value) should be converted to type's default value.
            new TestCaseData(null, typeof(string), null) { TestName = "Null: null -> null" },
            new TestCaseData(null, typeof(int), 0) { TestName = "Null: null -> default" },
            new TestCaseData(DBNull.Value, typeof(string), null) { TestName = "Null: DBNull -> null" },
            new TestCaseData(DBNull.Value, typeof(int), 0) { TestName = "Null: DBNull -> default" },

            // For equal or assignable types we just reuse the existing value.
            new TestCaseData(ConvertChild, typeof(ConvertChildType), ConvertChild) { TestName = "Assignable: child -> child" },
            new TestCaseData(ConvertChild, typeof(ConvertBaseType), ConvertChild) { TestName = "Assignable: child -> base" },
            new TestCaseData(ConvertBase, typeof(ConvertChildType), FailedConversion) { TestName = "Assignable: base -> child" },

            // When we're converting from Nullable<T> and/or to Nullable<T>, this
            // interface may be unwrapped and conversion made between inner type.
            new TestCaseData((short?)10, typeof(int), 10) { TestName = "INullable: short? -> int" },
            new TestCaseData((short)11, typeof(int?), 11) { TestName = "INullable: short -> int?" },
            new TestCaseData((short?)12, typeof(int?), 12) { TestName = "INullable: short? -> int?" },

            // Conversion between IConvertible types.
            new TestCaseData(123, typeof(short), (short)123),
            new TestCaseData(123, typeof(short), (byte)123),
            new TestCaseData(123, typeof(short), (sbyte)123),
            new TestCaseData(123, typeof(short), (double)123),
            new TestCaseData(123, typeof(short), (float)123),
            new TestCaseData(123, typeof(short), (decimal)123),
            new TestCaseData((short)123, typeof(int), 123),
            new TestCaseData((byte)123, typeof(int), 123),
            new TestCaseData((sbyte)123, typeof(int), 123),
            new TestCaseData((double)123, typeof(decimal), (decimal)123),
            new TestCaseData((float)123, typeof(decimal), (decimal)123),
            new TestCaseData("123", typeof(decimal), (decimal)123),
            new TestCaseData("2017-01-31", typeof(DateTime), new DateTime(2017, 01, 31)),

            // Conversion through custom parsing methods, not supported by IConvertible.
            new TestCaseData("01:23:45", typeof(TimeSpan), new TimeSpan(01, 23, 45))
        };

        [TestCaseSource(nameof(TypeConversions))]
        public void TryConvert_ProvidedCase_ExpectedResult(object value, Type targetType, object expected)
        {
            var success = TypeHelper.TryConvert(ref value, targetType);

            if (expected != FailedConversion)
            {
                Assert.That(success);
                Assert.That(value, Is.EqualTo(expected));
            }
            else
            {
                Assert.That(!success);
            }
        }

        #endregion

        #region ConvertData
        [TestCase(typeof(short))]
        [TestCase(typeof(byte))]
        [TestCase(typeof(sbyte))]
        [TestCase(typeof(decimal))]
        public void ConvertData_IntArray_ProperlyConverted(Type targetType)
        {
            var data = new object[] { 1, 2, 3 };

            IEnumerable result = TypeHelper.ConvertData(data, targetType);

            CollectionAssert.AllItemsAreInstancesOfType(result, targetType);
        }

        [Test]
        public void ConvertData_StringArray_ConvertedToDecimal()
        {
            var data = new object[] { "0.1", "1.0", "-1.0" };
            var expected = new decimal[] { 0.1M, 1.0M, -1.0M };

            IEnumerable result = TypeHelper.ConvertData(data, typeof(decimal));

            Assert.That(result, Is.EquivalentTo(expected).And.All.TypeOf<decimal>());
        }

        [Test]
        public void ConvertData_StringArray_ConvertedToDateTime()
        {
            var data = new object[] { "1970/01/01", "02/01/1980", "1999/12/31" };
            var expected = new DateTime[] { new DateTime(1970, 1, 1), new DateTime(1980, 2, 1), new DateTime(1999, 12, 31) };

            IEnumerable result = TypeHelper.ConvertData(data, typeof(DateTime));

            Assert.That(result, Is.EquivalentTo(expected).And.All.TypeOf<DateTime>());
        }

        [Test]
        public void ConvertData_EmptyArray_ConvertedToBooleanValues()
        {
            var data = new object[] { };
            var expected = new bool[] { true, false };

            IEnumerable result = TypeHelper.ConvertData(data, typeof(bool));

            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void ConvertData_EmptyArray_ConvertedToEnumValues()
        {
            var data = new object[] { };
            var expected = new ConvertDataEnum[] { ConvertDataEnum.Value1, ConvertDataEnum.Value2, ConvertDataEnum.Value3 };

            IEnumerable result = TypeHelper.ConvertData(data, typeof(ConvertDataEnum));

            Assert.That(result, Is.EquivalentTo(expected));
        }

        public enum ConvertDataEnum
        {
            Value1,
            Value2,
            Value3
        }
        #endregion
    }
}
