// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class ReusableConstraintTests
    {
        [Datapoints]
        static readonly ReusableConstraint[] constraints = new ReusableConstraint[] {
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
