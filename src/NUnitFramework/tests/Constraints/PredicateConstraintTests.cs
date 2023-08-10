// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class PredicateConstraintTests : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new PredicateConstraint<int>((x) => x < 5);

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = @"value matching lambda expression";
            StringRepresentation = "<predicate>";
        }

        private static readonly object[] SuccessData = new object[]
        {
            0,
            -5
        };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData(123, "123")
        };

        [Test]
        public void CanUseConstraintExpressionSyntax()
        {
            Assert.That(123, Is.TypeOf<int>().And.Matches<int>((int x) => x > 100));
        }

        [TestCase(typeof(object))]
        [TestCase(typeof(string))]
        [TestCase(typeof(int?))]
        [TestCase(typeof(AttributeTargets?))]
        public static void ActualMayBeNullForNullableTypes(Type type)
        {
            // https://github.com/nunit/nunit/issues/1215
            var methodInfo = typeof(PredicateConstraintTests)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Single(method => method.Name == nameof(ActualMayBeNullForNullableTypes) && method.GetParameters().Length == 0)
                .MakeGenericMethod(type);

            ((Action)methodInfo.CreateDelegate(typeof(Action))).Invoke();
        }

        private static void ActualMayBeNullForNullableTypes<T>()
        {
            Assert.That(default(object), new ConstraintExpression().Matches<T>(actual => true));
        }

        [TestCase(typeof(int))]
        [TestCase(typeof(AttributeTargets))]
        public static void ActualMustNotBeNullForNonNullableTypes(Type type)
        {
            // https://github.com/nunit/nunit/issues/1215
            var methodInfo = typeof(PredicateConstraintTests)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Single(method => method.Name == nameof(ActualMustNotBeNullForNonNullableTypes) && method.GetParameters().Length == 0)
                .MakeGenericMethod(type);

            ((Action)methodInfo.CreateDelegate(typeof(Action))).Invoke();
        }

        private static void ActualMustNotBeNullForNonNullableTypes<T>()
        {
            Assert.That(() =>
            {
                Assert.That(default(object), new ConstraintExpression().Matches<T>(actual => true));
            }, Throws.ArgumentException);
        }
    }
}
