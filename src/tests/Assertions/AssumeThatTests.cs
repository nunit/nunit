// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using NUnit.Framework;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class AssumeThatTests
    {
        [Test]
        public void AssumptionPasses_Boolean()
        {
            Assume.That(2 + 2 == 4);
        }

        [Test]
        public void AssumptionPasses_BooleanWithMessage()
        {
            Assume.That(2 + 2 == 4, "Not Equal");
        }

        [Test]
        public void AssumptionPasses_BooleanWithMessageAndArgs()
        {
            Assume.That(2 + 2 == 4, "Not Equal to {0}", 4);
        }

        [Test]
        public void AssumptionPasses_ActualAndConstraint()
        {
            Assume.That(2 + 2, Is.EqualTo(4));
        }

        [Test]
        public void AssumptionPasses_ActualAndConstraintWithMessage()
        {
            Assume.That(2 + 2, Is.EqualTo(4), "Should be 4");
        }

        [Test]
        public void AssumptionPasses_ActualAndConstraintWithMessageAndArgs()
        {
            Assume.That(2 + 2, Is.EqualTo(4), "Should be {0}", 4);
        }

        [Test]
        public void AssumptionPasses_ReferenceAndConstraint()
        {
            bool value = true;
            Assume.That(ref value, Is.True);
        }

        [Test]
        public void AssumptionPasses_ReferenceAndConstraintWithMessage()
        {
            bool value = true;
            Assume.That(ref value, Is.True, "Message");
        }

        [Test]
        public void AssumptionPasses_ReferenceAndConstraintWithMessageAndArgs()
        {
            bool value = true;
            Assume.That(ref value, Is.True, "Message", 42);
        }

        [Test]
        public void AssumptionPasses_DelegateAndConstraint()
        {
            Assume.That(new Constraints.ActualValueDelegate(ReturnsFour), Is.EqualTo(4));
        }

        [Test]
        public void AssumptionPasses_DelegateAndConstraintWithMessage()
        {
            Assume.That(new Constraints.ActualValueDelegate(ReturnsFour), Is.EqualTo(4), "Message");
        }

        [Test]
        public void AssumptionPasses_DelegateAndConstraintWithMessageAndArgs()
        {
            Assume.That(new Constraints.ActualValueDelegate(ReturnsFour), Is.EqualTo(4), "Should be {0}", 4);
        }

        private object ReturnsFour()
        {
            return 4;
        }

        [Test, ExpectedException(typeof(InconclusiveException))]
        public void FailureThrowsInconclusiveException_Boolean()
        {
            Assume.That(2 + 2 == 5);
        }

        [Test, ExpectedException(typeof(InconclusiveException))]
        public void FailureThrowsInconclusiveException_BooleanWithMessage()
        {
            Assume.That(2 + 2 == 5, "message");
        }

        [Test, ExpectedException(typeof(InconclusiveException))]
        public void FailureThrowsInconclusiveException_BooleanWithMessageAndArgs()
        {
            Assume.That(2 + 2 == 5, "message", 5);
        }

        [Test, ExpectedException(typeof(InconclusiveException))]
        public void FailureThrowsInconclusiveException_ActualAndConstraint()
        {
            Assume.That(2 + 2, Is.EqualTo(5));
        }

        [Test, ExpectedException(typeof(InconclusiveException))]
        public void FailureThrowsInconclusiveException_ActualAndConstraintWithMessage()
        {
            Assume.That(2 + 2, Is.EqualTo(5), "Error");
        }

        [Test, ExpectedException(typeof(InconclusiveException))]
        public void FailureThrowsInconclusiveException_ActualAndConstraintWithMessageAndArgs()
        {
            Assume.That(2 + 2, Is.EqualTo(5), "Should be {0}", 5);
        }

        [Test, ExpectedException(typeof(InconclusiveException))]
        public void FailureThrowsInconclusiveException_ReferenceAndConstraint()
        {
            bool value = false;
            Assume.That(ref value, Is.True);
        }

        [Test, ExpectedException(typeof(InconclusiveException))]
        public void FailureThrowsInconclusiveException_ReferenceAndConstraintWithMessage()
        {
            bool value = false;
            Assume.That(ref value, Is.True, "message");
        }

        [Test, ExpectedException(typeof(InconclusiveException))]
        public void FailureThrowsInconclusiveException_ReferenceAndConstraintWithMessageAndArgs()
        {
            bool value = false;
            Assume.That(ref value, Is.True, "message", 42);
        }

        [Test, ExpectedException(typeof(InconclusiveException))]
        public void FailureThrowsInconclusiveException_DelegateAndConstraint()
        {
            Assume.That(new Constraints.ActualValueDelegate(ReturnsFive), Is.EqualTo(4));
        }

        [Test, ExpectedException(typeof(InconclusiveException))]
        public void FailureThrowsInconclusiveException_DelegateAndConstraintWithMessage()
        {
            Assume.That(new Constraints.ActualValueDelegate(ReturnsFive), Is.EqualTo(4), "Error");
        }

        [Test, ExpectedException(typeof(InconclusiveException))]
        public void FailureThrowsInconclusiveException_DelegateAndConstraintWithMessageAndArgs()
        {
            Assume.That(new Constraints.ActualValueDelegate(ReturnsFive), Is.EqualTo(4), "Should be {0}", 4);
        }

        private object ReturnsFive()
        {
            return 5;
        }
    }
}
