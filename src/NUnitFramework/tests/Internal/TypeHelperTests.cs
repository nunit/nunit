// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class TypeHelperTests
    {
        #region BestCommonType

        [TestCase(typeof(A), typeof(B), ExpectedResult = typeof(A))]
        [TestCase(typeof(B), typeof(A), ExpectedResult = typeof(A))]
        [TestCase(typeof(A), typeof(string), ExpectedResult = null)]
        [TestCase(typeof(int[]), typeof(IEnumerable<int>), ExpectedResult = typeof(IEnumerable<int>))]
        public Type? BestCommonTypeTest(Type type1, Type type2)
        {
            return TypeHelper.TryGetBestCommonType(type1, type2, out var result) ? result : null;
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
            public class D<T3, T4>
            {
            }
        }

        #endregion

        #region FullName

        [Test]
        public void TestValidFullName()
        {
            Type type = typeof(TypeHelperTests);
            Assert.That(type.FullName(), Is.EqualTo(type.FullName));
        }

        [Test]
        public void TestInvalidFullName()
        {
            Type type = typeof(Generic<>).GetGenericArguments()[0];
            Assert.That(type.FullName, Is.Null);
            Assert.That(() => type.FullName(), Throws.InvalidOperationException);
        }

        private class Generic<T>
        {
            public Type TypeParameter => typeof(T);
        }

        #endregion

        #region IsRecord

        [TestCase(typeof(RecordClass), ExpectedResult = true)]
        [TestCase(typeof(RecordStruct), ExpectedResult = true)]
        [TestCase(typeof(RecordWithProperties), ExpectedResult = true)]
        [TestCase(typeof(int), ExpectedResult = false)]
        [TestCase(typeof(int[]), ExpectedResult = false)]
        [TestCase(typeof(DtoClass), ExpectedResult = false)]
        [TestCase(typeof(ClassWithPrimaryConstructor), ExpectedResult = false)]
        [TestCase(typeof(ClassWithOverriddenEquals), ExpectedResult = false)]
        public bool IsRecordTests(Type type) => TypeHelper.IsRecord(type);

        private class DtoClass
        {
            public string? Name { get; set; }
        }

        private class ClassWithPrimaryConstructor(string name)
        {
            public string Name => name;
        }

        private class ClassWithOverriddenEquals
        {
            public string? Name { get; set; }

            public override bool Equals(object? obj)
            {
                return obj is ClassWithOverriddenEquals other && other.Name == Name;
            }

            public override int GetHashCode()
            {
                return 539060726 + EqualityComparer<string?>.Default.GetHashCode(Name ?? string.Empty);
            }
        }

        private record class RecordClass(string Name);

        private record struct RecordStruct(string Name);

        private record RecordWithProperties
        {
            public string? Name { get; set; }
        }

        #endregion
    }
}
