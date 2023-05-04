// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class AttributeExistsConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new AttributeExistsConstraint(typeof(TestFixtureAttribute));
            ExpectedDescription = "type with attribute <NUnit.Framework.TestFixtureAttribute>";
            StringRepresentation = "<attributeexists NUnit.Framework.TestFixtureAttribute>";
        }

        private static object[] SuccessData = new object[] { typeof(AttributeExistsConstraintTests) };
        private static object[] FailureData = new object[] { 
            new TestCaseData( typeof(D2), "<" + typeof(D2).FullName + ">" ) };

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

        private class B { }

        private class D1 : B { }

        private class D2 : D1 { }
    }
}
