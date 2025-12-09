// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;
using static NUnit.Framework.Tests.Constraints.AssignableToConstraintReferenceTypeTests;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class AssignableToConstraintValueTypeTests : AssignableToConstraintTests<int, long>
    {
    }

    [TestFixture]
    public class AssignableToConstraintReferenceTypeTests : AssignableToConstraintTests<D2, D1>
    {
        public class D1
        {
        }

        public class D2 : D1
        {
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
