// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class XmlSerializableTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new XmlSerializableConstraint();
            ExpectedDescription = "XML serializable";
            StringRepresentation = "<xmlserializable>";
        }

        static object[] SuccessData = new object[] { 1, "a", new List<string>() };

        static object[] FailureData = new object[] {
            new TestCaseData( new Dictionary<string, string>(), "<System.Collections.Generic.Dictionary`2[System.String,System.String]>" ),
            new TestCaseData( new InternalClass(), "<" + typeof(InternalClass).FullName + ">" ),
            new TestCaseData( new InternalWithSerializableAttributeClass(), "<" + typeof(InternalWithSerializableAttributeClass).FullName + ">" )
        };

        [Test]
        public void NullArgumentThrowsException()
        {
            object o = null;
            Assert.Throws<ArgumentNullException>(() => TheConstraint.ApplyTo(o));
        }
    }

    internal class InternalClass
    { }

    [Serializable]
    internal class InternalWithSerializableAttributeClass
    { }
}
