// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class BinarySerializableTest : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new BinarySerializableConstraint();

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "binary serializable";
            StringRepresentation = "<binaryserializable>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { 1, "a", new List<int>(), new InternalWithSerializableAttributeClass() };
        private static readonly object[] FailureData = new object[] { new TestCaseData(new InternalClass(), "<NUnit.Framework.Tests.Constraints.InternalClass>") };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void NullArgumentThrowsException()
        {
            object? o = default(object);
            Assert.Throws<ArgumentNullException>(() => TheConstraint.ApplyTo(o));
        }
    }
}
