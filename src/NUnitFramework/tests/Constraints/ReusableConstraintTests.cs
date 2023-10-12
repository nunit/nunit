// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class ReusableConstraintTests
    {
        [Datapoints]
        public static readonly ReusableConstraint[] Constraints = new ReusableConstraint[]
        {
            Is.Not.Empty,
            Is.Not.Null,
            Has.Length.GreaterThan(3),
            Has.Property("Length").EqualTo(4).And.StartsWith("te")
        };

        [Theory]
        public void CanReuseReusableConstraintMultipleTimes(ReusableConstraint c)
        {
            string s = "test";

            Assume.That(s, c);

            Assert.That(s, c, "Should pass first time");
            Assert.That(s, c, "Should pass second time");
            Assert.That(s, c, "Should pass third time");
        }

        [Test]
        public void CanCreateReusableConstraintByImplicitConversion()
        {
            ReusableConstraint c = Is.Not.Null;

            string s = "test";
            Assert.That(s, c, "Should pass first time");
            Assert.That(s, c, "Should pass second time");
            Assert.That(s, c, "Should pass third time");
        }
    }
}
