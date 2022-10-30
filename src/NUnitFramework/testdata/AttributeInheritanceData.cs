// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestData.AttributeInheritanceData
{
    // Sample Test from a post by Scott Bellware

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    class ConcernAttribute : TestFixtureAttribute
    {
#pragma warning disable 414
        private Type typeOfConcern;
#pragma warning restore 414

        public ConcernAttribute( Type typeOfConcern )
        {
            this.typeOfConcern = typeOfConcern;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    class SpecAttribute : TestAttribute
    {
    }

    /// <summary>
    /// Summary description for AttributeInheritance.
    /// </summary>
    [Concern(typeof(ClassUnderTest))]
    public class When_collecting_test_fixtures
    {
        [Spec]
        public void Should_include_classes_with_an_attribute_derived_from_TestFixtureAttribute()
        {
        }
    }

    class ClassUnderTest { }
}
