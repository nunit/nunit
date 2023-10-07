// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class XmlSerializableTest : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new XmlSerializableConstraint();

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "XML serializable";
            StringRepresentation = "<xmlserializable>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { 1, "a", new List<string>() };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData( new Dictionary<string, string>(), "<System.Collections.Generic.Dictionary`2[System.String,System.String]>" ),
            new TestCaseData( new InternalClass(), "<" + typeof(InternalClass).FullName + ">" ),
            new TestCaseData( new InternalWithSerializableAttributeClass(), "<" + typeof(InternalWithSerializableAttributeClass).FullName + ">" )
        };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void NullArgumentThrowsException()
        {
            object? o = null;
            Assert.Throws<ArgumentNullException>(() => TheConstraint.ApplyTo(o));
        }
    }

    internal class InternalClass
    { }

    [Serializable]
    internal class InternalWithSerializableAttributeClass
    { }
}
