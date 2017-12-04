// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Linq;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class PredicateConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new PredicateConstraint<int>((x) => x < 5);
            expectedDescription = @"value matching lambda expression";
            stringRepresentation = "<predicate>";
        }

        static object[] SuccessData = new object[]
        {
            0,
            -5
        };

        static object[] FailureData = new object[]
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
            var methodInfo = typeof(PredicateConstraintTests).GetTypeInfo()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Single(method => method.Name == nameof(ActualMayBeNullForNullableTypes) && method.GetParameters().Length == 0)
                .MakeGenericMethod(type);

            ((Action)methodInfo.CreateDelegate(typeof(Action))).Invoke();
        }

        private static void ActualMayBeNullForNullableTypes<T>()
        {
            Assert.That(null, new ConstraintExpression().Matches<T>(actual => true));
        }

        [TestCase(typeof(int))]
        [TestCase(typeof(AttributeTargets))]
        public static void ActualMustNotBeNullForNonNullableTypes(Type type)
        {
            // https://github.com/nunit/nunit/issues/1215
            var methodInfo = typeof(PredicateConstraintTests).GetTypeInfo()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Single(method => method.Name == nameof(ActualMustNotBeNullForNonNullableTypes) && method.GetParameters().Length == 0)
                .MakeGenericMethod(type);

            ((Action)methodInfo.CreateDelegate(typeof(Action))).Invoke();
        }

        private static void ActualMustNotBeNullForNonNullableTypes<T>()
        {
            Assert.That(() =>
            {
                Assert.That(null, new ConstraintExpression().Matches<T>(actual => true));
            }, Throws.ArgumentException);
        }
    }
}
