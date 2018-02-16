// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;
namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class AnyOfConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new AnyOfConstraint(new object[] { 1, 2, 3 });
            expectedDescription = "any of < 1, 2, 3 >";
            stringRepresentation = "<anyof 1 2 3>";
        }

        private static object[] SuccessData = new object[] { 1, 2, 3 };
        private static object[] FailureData = new object[] { new object[] { 4, "4" }, new object[] { "A", "\"A\"" } };

        [Test]
        public void ItemIsPresent_IgnoreCase()
        {
            var anyOf = new AnyOfConstraint(new[] { "a", "B", "ab" }).IgnoreCase;
            Assert.That(anyOf.ApplyTo("AB").Status, Is.EqualTo(ConstraintStatus.Success));
        }

        [Test]
        public void ItemIsPresent_WithEqualityComparer()
        {
            Func<string, string, bool> comparer = (expected, actual) => actual.Contains(expected);
            var anyOf = new AnyOfConstraint(new[] { "A", "B", "C" }).Using(comparer);
            Assert.That(anyOf.ApplyTo("1. A").Status, Is.EqualTo(ConstraintStatus.Success));
        }
    }
}
