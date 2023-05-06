// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;

namespace NUnit.TestData.AttributeInheritanceData
{
    // Sample Test from a post by Scott Bellware

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    internal class ConcernAttribute : TestFixtureAttribute
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly Type _typeOfConcern;
#pragma warning restore IDE0052 // Remove unread private members

        public ConcernAttribute( Type typeOfConcern )
        {
            _typeOfConcern = typeOfConcern;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    internal class SpecAttribute : TestAttribute
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

    internal class ClassUnderTest { }
}
