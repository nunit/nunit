// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Interfaces;
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
            return type.GetDisplayName();
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

        #region HasCompilerGeneratedEquals

        [TestCase(typeof(RecordClass), ExpectedResult = true)]
        [TestCase(typeof(RecordStruct), ExpectedResult = true)]
        [TestCase(typeof(RecordWithProperties), ExpectedResult = true)]
        [TestCase(typeof(RecordWithOverriddenEquals), ExpectedResult = false)]
        [TestCase(typeof(int), ExpectedResult = false)]
        [TestCase(typeof(int[]), ExpectedResult = false)]
        [TestCase(typeof(DtoClass), ExpectedResult = false)]
        [TestCase(typeof(ClassWithPrimaryConstructor), ExpectedResult = false)]
        [TestCase(typeof(ClassWithOverriddenEquals), ExpectedResult = false)]
        public bool HasCompilerGeneratedEqualsTests(Type type) => type.HasCompilerGeneratedEquals();

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

        private record RecordWithOverriddenEquals(string Name)
        {
            public virtual bool Equals(RecordWithOverriddenEquals? other)
            {
                return string.Equals(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
            }

            public override int GetHashCode()
            {
                return Name.ToUpperInvariant().GetHashCode();
            }
        }

        #endregion

        #region ConvertArgumentList

        [Test]
        public void ConvertArgumentList_WithMultipleTestMethodsUsingSameTestCaseData()
        {
            // Regression test for Issue #3125
            // When the same TestCaseData is used by multiple test methods with different parameter types,
            // the argument conversion should not mutate the original array, which would cause subsequent
            // test methods to receive already-converted values.

            // Create test parameters using reflection from real methods
            var methodA = typeof(TestMethods).GetMethod(nameof(TestMethods.TestA));
            var methodB = typeof(TestMethods).GetMethod(nameof(TestMethods.TestB));

            var paramA = methodA!.GetParameters()
                .Select(p => new RuntimeParameterInfo(p))
                .ToArray();
            var paramB = methodB!.GetParameters()
                .Select(p => new RuntimeParameterInfo(p))
                .ToArray();

            // Original arguments from TestCaseData
            object?[] originalArgs = new object?[] { string.Empty, 0 };

            // Convert for TestA (should convert int to float)
            object?[] argsForTestA = (object?[])originalArgs.Clone();
            argsForTestA = TypeHelper.ConvertArgumentList(argsForTestA, paramA);
            using (Assert.EnterMultipleScope())
            {
                // Verify TestA received the float conversion
                Assert.That(argsForTestA[1], Is.TypeOf<float>(), "TestA argument should be float type");
                Assert.That((float)argsForTestA[1]!, Is.EqualTo(0.0f), "TestA should receive converted float");

                // Convert for TestB using fresh original args (should NOT convert, int is already int)
                object?[] argsForTestB = (object?[])originalArgs.Clone();
                argsForTestB = TypeHelper.ConvertArgumentList(argsForTestB, paramB);

                // Verify TestB receives int (not float from previous conversion)
                Assert.That(argsForTestB[1], Is.TypeOf<int>(), "TestB argument should be int type");
                Assert.That((int)argsForTestB[1]!, Is.EqualTo(0), "TestB should receive original int value");
            }
        }

        private static class TestMethods
        {
            public static void TestA(string a, float b)
            {
            }

            public static void TestB(string a, int b)
            {
            }
        }

        private class RuntimeParameterInfo(ParameterInfo parameterInfo) : IParameterInfo
        {
            public Type ParameterType => parameterInfo.ParameterType;

            public string Name => parameterInfo.Name ?? "param";

            public bool IsOptional => parameterInfo.IsOptional;

            public IMethodInfo Method => throw new NotImplementedException();

            public System.Reflection.ParameterInfo ParameterInfo => parameterInfo;

            T[] IReflectionInfo.GetCustomAttributes<T>(bool inherit) =>
                parameterInfo.GetCustomAttributes(typeof(T), inherit).OfType<T>().ToArray();

            bool IReflectionInfo.IsDefined<T>(bool inherit) => parameterInfo.IsDefined(typeof(T), inherit);
        }

        private class FloatComparer : IComparer<float>
        {
            public static readonly FloatComparer Instance = new FloatComparer();

            public int Compare(float x, float y)
            {
                return x.CompareTo(y);
            }
        }

        #endregion
    }
}
