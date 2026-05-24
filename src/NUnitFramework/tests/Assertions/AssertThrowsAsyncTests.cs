// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssertThrowsAsyncTests
    {
        [Test]
        public void ThrowsAsyncSucceedsWithDelegate()
        {
            Assert.ThrowsAsync(typeof(ArgumentException), AsyncTestDelegates.ThrowsArgumentException);
            Assert.ThrowsAsync(typeof(ArgumentException),
                delegate { throw new ArgumentException(); });
        }

        [Test]
        public void GenericThrowsAsyncSucceedsWithDelegate()
        {
            Assert.ThrowsAsync<ArgumentException>(
                delegate { throw new ArgumentException(); });
            Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentException);
        }

        [Test]
        public async Task AsyncRegionThrowsDoesNotDisposeAsyncRegion()
        {
            await AsyncTestDelegates.Delay(100);
            Assert.ThrowsAsync<ArgumentException>(async () => await AsyncTestDelegates.ThrowsArgumentExceptionAsync());
            Assert.That(async () => await AsyncTestDelegates.ThrowsArgumentExceptionAsync(), Throws.ArgumentException);
        }

        [Test]
        public void ThrowsAsyncReturnsCorrectException()
        {
            Assert.ThrowsAsync(typeof(ArgumentException), AsyncTestDelegates.ThrowsArgumentExceptionAsync);
            Assert.ThrowsAsync(typeof(ArgumentException),
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => throw new ArgumentException(), TaskScheduler.Default); });

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void GenericThrowsAsyncReturnsCorrectException()
        {
            Assert.ThrowsAsync<ArgumentException>(
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => throw new ArgumentException(), TaskScheduler.Default); });
            Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentExceptionAsync);

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void ThrowsConstraintReturnsCorrectException()
        {
            Assert.That(
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => throw new ArgumentException(), TaskScheduler.Default); },
                Throws.Exception.TypeOf<ArgumentException>());

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void ThrowsAsyncIsNotAffectedByAssertionsInDelegate()
        {
            Assert.ThrowsAsync<AssertionException>(
                () => Assert.ThatAsync(AsyncTestDelegates.ThrowsArgumentExceptionAsync, Throws.InvalidOperationException));
        }

        [Test]
        public void CorrectExceptionIsReturnedToMethod()
        {
            var ex = Assert.ThrowsAsync(typeof(ArgumentException),
                AsyncTestDelegates.ThrowsArgumentException) as ArgumentException;

            VerifyMessageAndParam(ex);

            ex = Assert.ThrowsAsync<ArgumentException>(
                delegate { throw new ArgumentException("myMessage", "myParam"); });

            VerifyMessageAndParam(ex);

            ex = Assert.ThrowsAsync(typeof(ArgumentException),
                delegate { throw new ArgumentException("myMessage", "myParam"); }) as ArgumentException;

            VerifyMessageAndParam(ex);

            ex = Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentException);

            VerifyMessageAndParam(ex);
        }

        private void VerifyMessageAndParam(ArgumentException? ex)
        {
            Assert.That(ex, Is.Not.Null, "No ArgumentException thrown");
            using (Assert.EnterMultipleScope())
            {
                Assert.That(ex!.Message, Does.StartWith("myMessage"));
                Assert.That(ex.ParamName, Is.EqualTo("myParam"));
            }
        }

        [Test]
        public void CorrectExceptionIsReturnedToMethodAsync()
        {
            var ex = Assert.ThrowsAsync(typeof(ArgumentException),
                AsyncTestDelegates.ThrowsArgumentExceptionAsync) as ArgumentException;

            VerifyMessageAndParam(ex);

            ex = Assert.ThrowsAsync<ArgumentException>(
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => throw new ArgumentException("myMessage", "myParam"), TaskScheduler.Default); });

            VerifyMessageAndParam(ex);

            ex = Assert.ThrowsAsync(typeof(ArgumentException),
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => throw new ArgumentException("myMessage", "myParam"), TaskScheduler.Default); }) as ArgumentException;

            VerifyMessageAndParam(ex);

            ex = Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentExceptionAsync);

            VerifyMessageAndParam(ex);
        }

        [Test]
        public void NoExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsNothing));
            VerifyArgumentExceptionWithNullMessage(ex);

            CheckForSpuriousAssertionResults();
        }

        private static void VerifyArgumentExceptionWithNullMessage(Exception? ex)
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain(
                "  Expected: <System.ArgumentException>" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine));
        }

        private static void VerifyArgumentExceptionWithNullRefExceptionAndMyMessage(Exception? ex)
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain(
                "  Expected: <System.ArgumentException>" + Environment.NewLine +
                "  But was:  <System.NullReferenceException: my message" + Environment.NewLine));
        }
        private static void VerifyArgumentExceptionWithSystemExceptionAndMyMessage(Exception? ex)
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain(
                "  Expected: <System.ArgumentException>" + Environment.NewLine +
                "  But was:  <System.Exception: my message" + Environment.NewLine));
        }

        [Test]
        public void UnrelatedExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsNullReferenceException));
            VerifyArgumentExceptionWithNullRefExceptionAndMyMessage(ex);

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void UnrelatedExceptionThrownAsync()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsNullReferenceExceptionAsync));
            VerifyArgumentExceptionWithNullRefExceptionAndMyMessage(ex);

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void BaseExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsSystemException));
            VerifyArgumentExceptionWithSystemExceptionAndMyMessage(ex);

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void BaseExceptionThrownAsync()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsSystemExceptionAsync));
            VerifyArgumentExceptionWithSystemExceptionAndMyMessage(ex);
        }

        [Test, SetUICulture("en-US")]
        public void DerivedExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<Exception>(AsyncTestDelegates.ThrowsArgumentException));
            VerifyArgumentExceptionWithArgumentExceptionAndMyMessage(ex);

            CheckForSpuriousAssertionResults();
        }

        private static void VerifyArgumentExceptionWithArgumentExceptionAndMyMessage(Exception? ex)
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain(
                "  Expected: <System.Exception>" + Environment.NewLine +
                "  But was:  <System.ArgumentException: myMessage"));
        }

        [Test, SetUICulture("en-US")]
        public void DerivedExceptionThrownAsync()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<Exception>(AsyncTestDelegates.ThrowsArgumentExceptionAsync));
            //Assert.That(ex, Is.Not.Null);
            //Assert.That(ex!.Message, Does.Contain(
            //    "  Expected: <System.Exception>" + Environment.NewLine +
            //    "  But was:  <System.ArgumentException: myMessage"));
            VerifyArgumentExceptionWithArgumentExceptionAndMyMessage(ex);
        }

        [Test]
        public void DoesNotThrowSucceeds()
        {
            Assert.DoesNotThrowAsync(AsyncTestDelegates.ThrowsNothing);
        }

        [Test]
        public void DoesNotThrowFails()
        {
            var ex = CatchException(() => Assert.DoesNotThrowAsync(AsyncTestDelegates.ThrowsArgumentException));
            Assert.That(ex, Is.Not.Null.With.TypeOf<AssertionException>());

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void DoesNotThrowFailsAsync()
        {
            var ex = CatchException(() => Assert.DoesNotThrowAsync(AsyncTestDelegates.ThrowsArgumentExceptionAsync));
            Assert.That(ex, Is.Not.Null.With.TypeOf<AssertionException>());

            CheckForSpuriousAssertionResults();
        }

        private static void CheckForSpuriousAssertionResults()
        {
            var result = TestExecutionContext.CurrentContext.CurrentResult;
            Assert.That(result.AssertionResultCount, Is.Zero,
                "Spurious result left by Assert.Fail()");
        }

        private Exception? CatchException(Action del)
        {
            using (new TestExecutionContext.IsolatedContext())
            {
                try
                {
                    del();
                    return null;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }
    }
}
