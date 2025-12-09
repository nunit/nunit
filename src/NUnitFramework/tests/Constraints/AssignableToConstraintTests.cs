// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public static class AssignableToConstraintTests
    {
        [TestFixture(TypeArgs = [typeof(int), typeof(long)])]
        [TestFixture(TypeArgs = [typeof(int), typeof(int)])]
        [TestFixture(TypeArgs = [typeof(float), typeof(double)])]
        [TestFixture(TypeArgs = [typeof(D2), typeof(D1)], Description = "Direct inheritance")]
        [TestFixture(TypeArgs = [typeof(D3), typeof(D1)], Description = "Implicit cast")]
        public class SuccessTests<TFrom, TTo> : AssignableToConstraintTests<TFrom, TTo>
            where TFrom : new()
            where TTo : new()
        {
        }

        public class D1
        {
        }

        public class D2 : D1
        {
        }

        public class D3
        {
            public static implicit operator D1(D3 _) => new D1();
        }
    }

    public abstract class AssignableToConstraintTests<TFrom, TTo> : ConstraintTestBase
        where TFrom : new()
        where TTo : new()
    {
        protected override Constraint TheConstraint { get; } = new AssignableToConstraint(typeof(TTo));

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = $"assignable to <{typeof(TTo)}>";
            StringRepresentation = $"<assignableto {typeof(TTo)}>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = [new TTo(), new TFrom()];
        private static readonly object[] FailureData = [new TestCaseData(new object(), "<" + typeof(object).FullName + ">")];
#pragma warning restore IDE0052 // Remove unread private members
    }
}
