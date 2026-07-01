// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    internal class AttributeContraintTests
    {
        [Test]
        public void ValidateSpecificAttributeExists()
        {
            Assert.That(typeof(TestClassWithAttribute), Has.Attribute<CustomAttribute>());
            Assert.That(typeof(TestClassWithAttribute), Has.No.Attribute<DerivedCustomAttribute>());
        }

        [Test]
        public void ValidateDerivedAttributeExists()
        {
            Assert.That(typeof(TestClassWithDerivedAttribute), Has.Attribute<CustomAttribute>());
            Assert.That(typeof(TestClassWithDerivedAttribute), Has.Attribute<DerivedCustomAttribute>());
        }

        [Test]
        public void ValidateNoAttributeExists()
        {
            Assert.That(typeof(TestClassWithoutAttribute), Has.No.Attribute<CustomAttribute>());
        }

        [Test]
        public void ValidateAttributeProperty()
        {
            Assert.That(typeof(TestClassWithAttribute), Has.Attribute<CustomAttribute>()
                .Property("Description").EqualTo("Foo"));
            Assert.That(typeof(TestClassWithDerivedAttribute), Has.Attribute<DerivedCustomAttribute>()
                .Property("Description").EqualTo("Foo"));
        }

        [Test]
        public void ShouldNotCompile_WhenAssertingNonAttributeType()
        {
            const string code =
@"using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

class SomeClass
{
    [Test]
    void SomeMethod()
    {
        Assert.That(new object(), Has.Attribute<string>());
    }
}";
            var compiler = new TestCompiler();
            var results = compiler.CompileCode(code);

            Assert.That(results.Success, Is.False, "Code fragment with Has.Attribute<string>() should not compile but it did.");

            var expectedFailure = results.Diagnostics.FirstOrDefault(x => x.Id == "CS0311");

            Assert.That(expectedFailure, Is.Not.Null, "Expected compiler error 'CS0311' for violating generic type constraint.");
            Assert.That(expectedFailure.GetMessage(), Does.Contain("There is no implicit reference conversion from 'string' to 'System.Attribute'."));
        }

        [Custom]
        private class TestClassWithAttribute
        {
        }

        private class TestClassWithoutAttribute
        {
        }

        [DerivedCustom]
        private class TestClassWithDerivedAttribute
        {
        }

        private class CustomAttribute : Attribute
        {
            public string Description { get; set; } = "Foo";
        }
        private class DerivedCustomAttribute : CustomAttribute
        {
        }
    }
}
