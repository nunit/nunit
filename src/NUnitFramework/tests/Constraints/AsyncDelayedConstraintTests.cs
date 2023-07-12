// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class AsyncDelayedConstraintTests
    {
        [Test]
        public void ConstraintSuccess()
        {
            Assert.That(new DelayedConstraint(new EqualConstraint(1), 100)
                          .ApplyTo(async () => await One()).IsSuccess, Is.True);
        }

        [Test]
        public void ConstraintFailure()
        {
            Assert.That(new DelayedConstraint(new EqualConstraint(2), 100)
                           .ApplyTo(async () => await One()).IsSuccess, Is.False);
        }

        [Test]
        public void ConstraintError()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new DelayedConstraint(new EqualConstraint(1), 100).ApplyTo(new ActualValueDelegate<Task>(async () => await Throw())));
        }

        [Test]
        public void ConstraintVoidDelegateFailureAsDelegateIsNotCalled()
        {
            Assert.That(new DelayedConstraint(new EqualConstraint(1), 100)
                           .ApplyTo(new TestDelegate(async () => { await One(); })).IsSuccess, Is.False);
        }

        [Test]
        public void ConstraintVoidDelegateExceptionIsFailureAsDelegateIsNotCalled()
        {
            Assert.That(new DelayedConstraint(new EqualConstraint(1), 100)
                           .ApplyTo(new TestDelegate(async () => { await Throw(); })).IsSuccess, Is.False);
        }

        [Test]
        public void SyntaxSuccess()
        {
            Assert.That(async () => await One(), Is.EqualTo(1).After(100));
        }


        [Test]
        public void SyntaxFailure()
        {
            Assert.Throws<AssertionException>(() =>
                Assert.That(async () => await One(), Is.EqualTo(2).After(100)));
        }

        [Test]
        [Platform(Exclude = "Linux", Reason = "Intermittent failure under Linux")]
        public void SyntaxError()
        {
            Assert.Throws<InvalidOperationException>(() =>
                Assert.That(async () => await Throw(), Is.EqualTo(1).After(100)));
        }

        [Test]
        public void SyntaxVoidDelegateExceptionIsFailureAsCodeIsNotCalled()
        {
#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            Assert.Throws<AssertionException>(() =>
                Assert.That(new TestDelegate(async () => await Throw()), Is.EqualTo(1).After(100)));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
        }

        private static async Task<int> One()
        {
            return await Task.Run(() => 1);
        }

        private static async Task<int> Throw()
        {
            await One();
            throw new InvalidOperationException();
        }
    }
}
