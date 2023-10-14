// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

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
        private static readonly object[] SuccessData = new object[] { typeof(AttributeExistsConstraintTests) };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData(typeof(D2), "<" + typeof(D2).FullName + ">")
        };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void NonAttributeThrowsException()
        {
            Assert.Throws<System.ArgumentException>(() => new AttributeExistsConstraint(typeof(string)));
        }

        [Test]
        public void AttributeExistsOnMethodInfo()
        {
            Assert.That(
                GetType().GetMethod("AttributeExistsOnMethodInfo"),
                new AttributeExistsConstraint(typeof(TestAttribute)));
        }

        [Test, Description("my description")]
        public void AttributeTestPropertyValueOnMethodInfo()
        {
            Assert.That(
                GetType().GetMethod("AttributeTestPropertyValueOnMethodInfo"),
                Has.Attribute(typeof(DescriptionAttribute)).Property("Properties").Property("Keys").Contains("Description"));
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
