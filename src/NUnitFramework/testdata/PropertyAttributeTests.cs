// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;

namespace NUnit.TestData.PropertyAttributeTests
{
    [TestFixture, Property("ClassUnderTest","SomeClass" )]
    public class FixtureWithProperties
    {
        [Test, Property("user","Charlie")]
        public void Test1() { }

        [Test, Property("X",10.0), Property("Y",17.0)]
        public void Test2() { }

        [Test, Priority(5)]
        public void Test3() { }

        [Test, CustomProperty]
        public void Test4() { }

        [Test, Property("A", "A"), Property("B", "B"), Property("C", "C"), Property("D", "D"), Property("E", "E"), Property("F", "F")]
        public void Test5WithManyProperties()
        { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class PriorityAttribute : PropertyAttribute
    {
        public PriorityAttribute( int level ) : base( level ) { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CustomPropertyAttribute : PropertyAttribute
    {
        public CustomPropertyAttribute() :base(new SomeClass())
        {
        }
    }
    public class SomeClass
    {
    }
}
