// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Immutable;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    public class DefaultConstraintTests : ConstraintTestBaseNoData
    {
        public DefaultConstraintTests()
        {
            TheConstraint = new DefaultConstraint();
            ExpectedDescription = "default";
            StringRepresentation = "<default>";
        }

        // Cannot use parametrized tests here, as we can't allow boxing to happen.

        [Test] public void Success_Null() => AssertSuccess<string>(null);
        [Test] public void Success_Int() => AssertSuccess(default(int));
        [Test] public void Success_NullableInt() => AssertSuccess(default(int?));
        [Test] public void Success_Long() => AssertSuccess(default(long));
        [Test] public void Success_DefaultStructWithOverriddenEquals() => AssertSuccess(default(StructWithStrangeEquals));
        [Test] public void Success_ImmutableArray() => AssertSuccess(default(StructWithStrangeEquals));

        [Test] public void Failure_NewObject() => AssertFailure(new object());
        [Test] public void Failure_EmptyString() => AssertFailure(string.Empty);
        [Test] public void Failure_NullableInt_Zero() => AssertFailure((int?)0);
        [Test] public void Failure_BoxedInt_Zero() => AssertFailure((object)0);
        [Test] public void Failure_Int_NonZero() => AssertFailure(1);
        [Test] public void Failure_Double_NaN() => AssertFailure(double.NaN);
        [Test] public void Failure_NonDefaultStructWithOverriddenEquals() => AssertFailure(new StructWithStrangeEquals { SomeField = 1, EqualsResult = true });
        [Test] public void Failure_ImmutableArray_Empty() => AssertFailure(ImmutableArray.Create<int>());

        private void AssertSuccess<T>(T actual)
        {
            var constraintResult = TheConstraint.ApplyTo(actual);
            if (!constraintResult.IsSuccess)
            {
                MessageWriter writer = new TextMessageWriter();
                constraintResult.WriteMessageTo(writer);
                Assert.Fail(writer.ToString());
            }
        }

        private void AssertFailure<T>(T actual)
        {
            var constraintResult = TheConstraint.ApplyTo(actual);
            Assert.That(!constraintResult.IsSuccess);
        }

        private struct StructWithStrangeEquals
        {
            public int SomeField { get; set; }
            public bool EqualsResult { get; set; }

            public override bool Equals(object obj) => EqualsResult;

            public override int GetHashCode() => 0;
        }
    }
}
