// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

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

        [TestCase(10,20)]
        public void MethodWithParams(int x, int y)
        {
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
