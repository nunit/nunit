// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public static class AttributeConstraintTests
    {
        [TestFixtureSource(typeof(AttributeConstraintTests), nameof(GetConstraints))]
        public class ConstraintValidation : ConstraintTestBaseNoData
        {
            public ConstraintValidation(Constraint constraint) : base()
            {
                TheConstraint = constraint;
            }

            protected override Constraint TheConstraint { get; }

            [SetUp]
            public void SetUp()
            {
                ExpectedDescription = "attribute NUnit.Framework.DescriptionAttribute not null";
                StringRepresentation = "<attribute NUnit.Framework.DescriptionAttribute <not <null>>>";
            }
        }

        private static IEnumerable<TestFixtureData> GetConstraints()
        {
            yield return new TestFixtureData(new AttributeConstraint(typeof(DescriptionAttribute), Is.Not.Null))
                .SetArgDisplayNames("non-generic");
            yield return new TestFixtureData(new AttributeConstraint<DescriptionAttribute>(Is.Not.Null))
                .SetArgDisplayNames("generic");
        }

        [Test]
        public static void NonAttributeThrowsException()
        {
            Assert.Throws<System.ArgumentException>(() => new AttributeConstraint(typeof(string), Is.Not.Null));
        }

        [Test, Description("my description")]
        public static void AttributeConstraintMatchesPropertyOnMethod()
        {
            Assert.That(
                typeof(AttributeConstraintTests).GetMethod(nameof(AttributeConstraintMatchesPropertyOnMethod)),
                Has.Attribute(typeof(DescriptionAttribute)).Property("Properties").Property("Keys").Contains("Description"));
        }

        [Test, Description("my description")]
        public static void GenericAttributeConstraintMatchesPropertyOnMethod()
        {
            Assert.That(
                typeof(AttributeConstraintTests).GetMethod(nameof(GenericAttributeConstraintMatchesPropertyOnMethod)),
                Has.Attribute<DescriptionAttribute>().Property("Properties").Property("Keys").Contains("Description"));
        }

        [Test]
        public static void GenericAttributeConstraintSucceedsWhenAttributeExists()
        {
            Assert.That(
                typeof(AttributeConstraintTests).GetMethod(nameof(GenericAttributeConstraintMatchesPropertyOnMethod)),
                new AttributeConstraint<DescriptionAttribute>(Is.Not.Null));
        }
    }
}
