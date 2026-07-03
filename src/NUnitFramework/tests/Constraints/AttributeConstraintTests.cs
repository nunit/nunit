// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Constraints;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class AttributeExistsConstraintTests : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new AttributeExistsConstraint(typeof(TestFixtureAttribute));

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "type with attribute <NUnit.Framework.TestFixtureAttribute>";
            StringRepresentation = "<attributeexists NUnit.Framework.TestFixtureAttribute>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = [typeof(AttributeExistsConstraintTests)];
        private static readonly object[] FailureData = [new TestCaseData(typeof(D2), $"<{typeof(D2).FullName}>")];
#pragma warning restore IDE0052 // Remove unread private members
    }

    [TestFixture]
    public class AttributeConstraintTests : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new AttributeConstraint(typeof(TestFixtureAttribute), DummyConstraint.Instance);

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "attribute NUnit.Framework.TestFixtureAttribute ";
            StringRepresentation = "<attribute NUnit.Framework.TestFixtureAttribute <dummy>>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = [typeof(AttributeConstraintTests)];
        private static readonly object[] FailureData = [
            new TestCaseData(typeof(D2), $"<{typeof(D2).FullName}>"),
            new TestCaseData(typeof(D2), $"Attribute {typeof(TestFixtureAttribute)} was not found"),
        ];
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void AttributeExistsOnAssembly()
        {
            var asm = GetType().Assembly;

            Assert.That(asm, Has.Attribute(typeof(ParallelizableAttribute)));
            Assert.That(asm, Has.Attribute<ParallelizableAttribute>());
        }

        [Test]
        public void AttributeExistsOnType()
        {
            var type = GetType();

            Assert.That(type, Has.Attribute(typeof(TestFixtureAttribute)));
            Assert.That(type, Has.Attribute<TestFixtureAttribute>());
        }

        [Test]
        public void AttributeExistsOnMethodInfo()
        {
            var method = GetType().GetMethod(nameof(AttributeExistsOnMethodInfo));

            Assert.That(method, Has.Attribute(typeof(TestAttribute)));
            Assert.That(method, Has.Attribute<TestAttribute>());
        }

        [Test(Description = "my description")]
        public void AttributeTestPropertyValueOnMethodInfo()
        {
            var method = GetType().GetMethod(nameof(AttributeTestPropertyValueOnMethodInfo));

            Assert.That(method, Has.Attribute(typeof(TestAttribute)).Property("Description").EqualTo("my description"));
            Assert.That(method, Has.Attribute<TestAttribute>().Property("Description").EqualTo("my description"));
        }

        [Test]
        public void AttributeDoesntExist()
        {
            var type = GetType();

            Assert.That(type, Has.No.Attribute(typeof(ObsoleteAttribute)));
            Assert.That(type, Has.No.Attribute<ObsoleteAttribute>());
        }

        [Test]
        public void NonAttributeThrowsRuntimeException()
        {
            Assert.Throws<System.ArgumentException>(() => new AttributeConstraint(typeof(string), DummyConstraint.Instance));
            Assert.Throws<System.ArgumentException>(() => new AttributeExistsConstraint(typeof(string)));
        }

        [Test]
        public void NonAttributeThrowsCompileTimeException_WithGenericApi()
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
    }

    file class B
    {
    }

    file class D1 : B
    {
    }

    file class D2 : D1
    {
    }
}
