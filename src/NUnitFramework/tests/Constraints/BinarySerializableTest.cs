// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class BinarySerializableTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new BinarySerializableConstraint();
            ExpectedDescription = "binary serializable";
            StringRepresentation = "<binaryserializable>";
        }

        private static object[] SuccessData = new object[] { 1, "a", new List<int>(), new InternalWithSerializableAttributeClass() };
        private static object[] FailureData = new object[] { new TestCaseData( new InternalClass(), "<NUnit.Framework.Constraints.InternalClass>" ) };

        [Test]
        public void NullArgumentThrowsException()
        {
            object o = null;
            Assert.Throws<ArgumentNullException>(() => TheConstraint.ApplyTo(o));
        }
    }
}
