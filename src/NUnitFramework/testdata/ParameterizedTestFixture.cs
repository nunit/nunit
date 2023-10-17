// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;

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
}
