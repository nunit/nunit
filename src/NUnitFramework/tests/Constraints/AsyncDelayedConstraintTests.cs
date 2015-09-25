// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

#if ( NET_4_0 || NET_4_5 ) && !PORTABLE
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
			Assert.IsTrue(new DelayedConstraint(new EqualConstraint(1), 100)
				.ApplyTo(async () => await One()).IsSuccess);
		}

		[Test]
		public void ConstraintFailure()
		{
			Assert.IsFalse(new DelayedConstraint(new EqualConstraint(2), 100)
				.ApplyTo(async () => await One()).IsSuccess);
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
			Assert.IsFalse(new DelayedConstraint(new EqualConstraint(1), 100)
				.ApplyTo(new TestDelegate(async () => { await One(); })).IsSuccess);
		}

		[Test]
		public void ConstraintVoidDelegateExceptionIsFailureAsDelegateIsNotCalled()
		{
			Assert.IsFalse(new DelayedConstraint(new EqualConstraint(1), 100)
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