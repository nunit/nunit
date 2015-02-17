﻿#if NET_4_0 || NET_4_5
using System;
using System.Threading.Tasks;

namespace NUnit.Framework.Constraints.Tests
{
	[TestFixture]
	public class AsyncDelayedConstraintTests
	{
		[Test]
		public void ConstraintSuccess()
		{
            Assert.IsTrue(new DelayedConstraint(Is.EqualTo(1), 100)
				.ApplyTo(async () => await One()).IsSuccess);
		}

		[Test]
		public void ConstraintFailure()
		{
            Assert.IsFalse(new DelayedConstraint(Is.EqualTo(2), 100)
				.ApplyTo(async () => await One()).IsSuccess);
		}

		[Test]
		public void ConstraintError()
		{
			Assert.Throws<InvalidOperationException>(() =>
                new DelayedConstraint(Is.EqualTo(1), 100).ApplyTo(new ActualValueDelegate<Task>(async () => await Throw())));
		}

		[Test]
		public void ConstraintVoidDelegateFailureAsDelegateIsNotCalled()
		{
            Assert.IsFalse(new DelayedConstraint(Is.EqualTo(1), 100)
				.ApplyTo(new TestDelegate(async () => { await One(); })).IsSuccess);
		}

		[Test]
		public void ConstraintVoidDelegateExceptionIsFailureAsDelegateIsNotCalled()
		{
            Assert.IsFalse(new DelayedConstraint(Is.EqualTo(1), 100)
				.ApplyTo(new TestDelegate(async () => { await Throw(); })).IsSuccess);
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

		[Test, Platform(Exclude="Linux", Reason="Intermittent failure under Linux")]
		public void SyntaxError()
		{
			Assert.Throws<InvalidOperationException>(() =>
				Assert.That(async () => await Throw(), Is.EqualTo(1).After(100)));
		}

		[Test]
		public void SyntaxVoidDelegateExceptionIsFailureAsCodeIsNotCalled()
		{
			Assert.Throws<AssertionException>(() =>
				Assert.That(new TestDelegate(async () => await Throw()), Is.EqualTo(1).After(100)));
		}

		private static async Task<int> One()
		{
#if NET_4_5
			return await Task.Run(() => 1);
#elif NET_4_0
			return await TaskEx.Run(() => 1);
#endif
		}

		private static async Task Throw()
		{
			await One();
			throw new InvalidOperationException();
		}
	}
}
#endif