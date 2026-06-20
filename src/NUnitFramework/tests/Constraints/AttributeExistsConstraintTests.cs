// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public static class AttributeExistsConstraintTests
    {
        [TestFixtureSource(typeof(AttributeExistsConstraintTests), nameof(GetConstraints))]
        public class ConstraintValidation : ConstraintTestBase
        {
            public ConstraintValidation(Constraint constraint) : base()
            {
                TheConstraint = constraint;
            }

            protected override Constraint TheConstraint { get; }

            [SetUp]
            public void SetUp()
            {
                ExpectedDescription = "type with attribute <NUnit.Framework.TestFixtureAttribute>";
                StringRepresentation = "<attributeexists NUnit.Framework.TestFixtureAttribute>";
            }

#pragma warning disable IDE0052 // Remove unread private members
            private static readonly object[] SuccessData = [typeof(AttributeExistsConstraintTests)];
            private static readonly object[] FailureData =
            [
                new TestCaseData(typeof(D2), "<" + typeof(D2).FullName + ">")
            ];
#pragma warning restore IDE0052 // Remove unread private members
        }

        private static IEnumerable<TestFixtureData> GetConstraints()
        {
            yield return new TestFixtureData(new AttributeExistsConstraint(typeof(TestFixtureAttribute)))
                .SetArgDisplayNames("non-generic");
            yield return new TestFixtureData(new AttributeExistsConstraint<TestFixtureAttribute>())
                .SetArgDisplayNames("generic");
        }

        [Test]
        public static void NonAttributeThrowsException()
        {
            Assert.Throws<System.ArgumentException>(() => new AttributeExistsConstraint(typeof(string)));
        }

        [Test]
        public static void AttributeExistsOnMethodInfo()
        {
            Assert.That(
                typeof(AttributeExistsConstraintTests).GetMethod(nameof(AttributeExistsOnMethodInfo)),
                new AttributeExistsConstraint(typeof(TestAttribute)));
        }

        [Test]
        public static void GenericAttributeExistsOnMethodInfo()
        {
            Assert.That(
                typeof(AttributeExistsConstraintTests).GetMethod(nameof(GenericAttributeExistsOnMethodInfo)),
                new AttributeExistsConstraint<TestAttribute>());
        }

        [Test, Description("my description")]
        public static void AttributeTestPropertyValueOnMethodInfo()
        {
            Assert.That(
                typeof(AttributeExistsConstraintTests).GetMethod(nameof(AttributeTestPropertyValueOnMethodInfo)),
                Has.Attribute(typeof(DescriptionAttribute)).Property("Properties").Property("Keys").Contains("Description"));
        }

        [Test, Description("my description")]
        public static void GenericAttributeTestPropertyValueOnMethodInfo()
        {
            Assert.That(
                typeof(AttributeExistsConstraintTests).GetMethod(nameof(GenericAttributeTestPropertyValueOnMethodInfo)),
                Has.Attribute<DescriptionAttribute>().Property("Properties").Property("Keys").Contains("Description"));
        }

        private class B
        {
        }

        private class D1 : B
        {
        }

        private class D2 : D1
        {
        }
    }
}
