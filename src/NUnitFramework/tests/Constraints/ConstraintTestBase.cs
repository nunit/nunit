// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
    public abstract class ConstraintTestBaseNoData
    {
        protected Constraint theConstraint;
        protected string expectedDescription = "<NOT SET>";
        protected string stringRepresentation = "<NOT SET>";

        [Test]
        public void ProvidesProperDescription()
        {
            Assert.That(theConstraint.Description, Is.EqualTo(expectedDescription));
        }

        [Test]
        public void ProvidesProperStringRepresentation()
        {
            Assert.That(theConstraint.ToString(), Is.EqualTo(stringRepresentation));
        }
    }

    public abstract class ConstraintTestBase : ConstraintTestBaseNoData
    {
        [Test, TestCaseSource("SuccessData")]
        public void SucceedsWithGoodValues(object value)
        {
            var constraintResult = theConstraint.ApplyTo(value);
            if (!constraintResult.IsSuccess)
            {
                MessageWriter writer = new TextMessageWriter();
                constraintResult.WriteMessageTo(writer);
                Assert.Fail(writer.ToString());
            }
        }

        [Test, TestCaseSource("FailureData")]
        public void FailsWithBadValues(object badValue, string message)
        {
            string NL = Env.NewLine;

            var constraintResult = theConstraint.ApplyTo(badValue);
            Assert.IsFalse(constraintResult.IsSuccess);

            TextMessageWriter writer = new TextMessageWriter();
            constraintResult.WriteMessageTo(writer);
            Assert.That( writer.ToString(), Is.EqualTo(
                TextMessageWriter.Pfx_Expected + expectedDescription + NL +
                TextMessageWriter.Pfx_Actual + message + NL ));
        }
    }
}