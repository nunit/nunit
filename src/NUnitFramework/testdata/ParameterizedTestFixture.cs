// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.TestData
{
    [TestFixture(1)]
    [TestFixture(2)]
    public class ParameterizedTestFixture
    {
        [Test]
        public void MethodWithoutParams()
        {
        }

        [TestCase(10, 20)]
        public void MethodWithParams(int x, int y)
        {
        }
    }

    [TestFixture(ParameterValue1)]
    [TestFixture(ParameterValue2)]
    public sealed class AnotherParameterizedTestFixture
    {
        public const string ParameterValue1 = "Hello World";
        public const string DisplayParameterValue1 = $"\"{ParameterValue1}\"";
        public const string ParameterValue2 = "X\nY\r\nZ";
        public const string DisplayParameterValue2 = "\"X\\nY\\r\\nZ\"";

        private readonly string _parameter;

        public AnotherParameterizedTestFixture(string parameter)
        {
            _parameter = parameter;
        }

        [TestCase(ParameterValue2)]
        public void TestCase(string parameter)
        {
            Assert.That(parameter, Is.EqualTo(_parameter));
        }
    }

    [TestFixture(Category = "XYZ")]
    public class TestFixtureWithSingleCategory
    {
    }

    [TestFixture(Category = "X,Y,Z")]
    public class TestFixtureWithMultipleCategories
    {
    }

    [TestFixture(null)]
    public class TestFixtureWithNullArgumentForOrdinaryValueTypeParameter
    {
        public TestFixtureWithNullArgumentForOrdinaryValueTypeParameter(OrdinaryValueType _)
        {
        }

        public struct OrdinaryValueType
        {
        }
    }

    [TestFixture(null)]
    public class TestFixtureWithNullArgumentForGenericParameter<T>
    {
        public TestFixtureWithNullArgumentForGenericParameter(T _)
        {
        }
    }

    [TestFixture(typeof(string))]
    [TestFixture(TypeArgs = [typeof(int)])]
    public class GenericClassWith<TOuter>
        where TOuter : notnull
    {
        public class NestedClassImplicitTextFixtureAttribute
        {
            [Test]
            public void Test()
            {
                Assert.That(typeof(TOuter).IsClass, Is.True);
            }
        }

        [TestFixture]
        public class NestedClassExplicitTestFixtureAttribute
        {
            [Test]
            public void Test()
            {
                Assert.That(typeof(TOuter).IsClass, Is.True);
            }
        }

        [TestFixture("42")]
        [TestFixture(42)]
        [TestFixture(42, TypeArgs = [typeof(long)])]
        public class NestedGenericClass<TInner>
            where TInner : notnull
        {
            private readonly TInner _inner;

            public NestedGenericClass(TInner argument)
            {
                _inner = argument;
            }

            [Test]
            public void Test()
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(_inner, Is.InstanceOf<TInner>());
                    Assert.That(_inner.ToString(), Is.EqualTo("42"));

                    Assert.That(typeof(TInner), Is.EqualTo(typeof(TOuter)));
                }
            }
        }
    }

    public static class NonGenericClassWith
    {
        [TestFixture]
        public class NestedClass
        {
            [Test]
            public void Test()
            {
                Assert.That(typeof(NestedClass).IsClass, Is.True);
            }
        }

        [TestFixtureSource(typeof(GenericTestFixtureCases), nameof(GenericTestFixtureCases.GetTestFixtureCases))]
        public class NestedGenericClass<TInner>
            where TInner : notnull
        {
            private readonly TInner _inner;

            public NestedGenericClass(TInner argument)
            {
                _inner = argument;
            }

            [Test]
            public void Test()
            {
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(_inner, Is.InstanceOf<TInner>());
                    Assert.That(_inner.ToString(), Is.EqualTo("42"));

                    Assert.That(typeof(TInner).IsClass, Is.True);
                }
            }
        }

        public static class GenericTestFixtureCases
        {
            public static IEnumerable<TestFixtureParameters> GetTestFixtureCases()
            {
                yield return new TestFixtureParameters("42");
                yield return new TestFixtureParameters(42);
                yield return new TestFixtureParameters(42)
                {
                    TypeArgs = [typeof(long)]
                };
            }
        }
    }
}
