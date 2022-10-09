// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading.Tasks;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Assertions
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
        public void ThrowsConstraintSucceedsWithDelegate()
        {
            // Without cast, delegate is ambiguous before C# 3.0.
            Assert.That((AsyncTestDelegate)delegate { throw new ArgumentException(); },
                    Throws.Exception.TypeOf<ArgumentException>());
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
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => { throw new ArgumentException(); }, TaskScheduler.Default); });

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void GenericThrowsAsyncReturnsCorrectException()
        {
            Assert.ThrowsAsync<ArgumentException>(
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => { throw new ArgumentException(); }, TaskScheduler.Default); });
            Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentExceptionAsync);

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void ThrowsConstraintReturnsCorrectException()
        {
            // Without cast, delegate is ambiguous before C# 3.0.
            Assert.That(
                (AsyncTestDelegate)delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => { throw new ArgumentException(); }, TaskScheduler.Default); },
                    Throws.Exception.TypeOf<ArgumentException>());

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void CorrectExceptionIsReturnedToMethod()
        {
            ArgumentException ex = Assert.ThrowsAsync(typeof(ArgumentException),
                new AsyncTestDelegate(AsyncTestDelegates.ThrowsArgumentException)) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));

            ex = Assert.ThrowsAsync<ArgumentException>(
                delegate { throw new ArgumentException("myMessage", "myParam"); });

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));

            ex = Assert.ThrowsAsync(typeof(ArgumentException),
                delegate { throw new ArgumentException("myMessage", "myParam"); }) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));

            ex = Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentException);

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
        }

        [Test]
        public void CorrectExceptionIsReturnedToMethodAsync()
        {
            ArgumentException ex = Assert.ThrowsAsync(typeof(ArgumentException),
                new AsyncTestDelegate(AsyncTestDelegates.ThrowsArgumentExceptionAsync)) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));

            ex = Assert.ThrowsAsync<ArgumentException>(
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => { throw new ArgumentException("myMessage", "myParam"); }, TaskScheduler.Default); });

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));

            ex = Assert.ThrowsAsync(typeof(ArgumentException),
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => { throw new ArgumentException("myMessage", "myParam"); }, TaskScheduler.Default); }) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));

            ex = Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentExceptionAsync);

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
        }


        [Test]
        public void NoExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsNothing));
            Assert.That(ex.Message, Is.EqualTo(
                "  Expected: <System.ArgumentException>" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine));

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void UnrelatedExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsNullReferenceException));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Environment.NewLine +
                "  But was:  <System.NullReferenceException: my message" + Environment.NewLine));

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void UnrelatedExceptionThrownAsync()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsNullReferenceExceptionAsync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Environment.NewLine +
                "  But was:  <System.NullReferenceException: my message" + Environment.NewLine));

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void BaseExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsSystemException));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Environment.NewLine +
                "  But was:  <System.Exception: my message" + Environment.NewLine));

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void BaseExceptionThrownAsync()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsSystemExceptionAsync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Environment.NewLine +
                "  But was:  <System.Exception: my message" + Environment.NewLine));
        }

        [Test, SetUICulture("en-US")]
        public void DerivedExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<Exception>(AsyncTestDelegates.ThrowsArgumentException));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.Exception>" + Environment.NewLine +
                "  But was:  <System.ArgumentException: myMessage"));

            CheckForSpuriousAssertionResults();
        }

        [Test, SetUICulture("en-US")]
        public void DerivedExceptionThrownAsync()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<Exception>(AsyncTestDelegates.ThrowsArgumentExceptionAsync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.Exception>" + Environment.NewLine +
                "  But was:  <System.ArgumentException: myMessage"));
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
            Assert.That(result.AssertionResults.Count, Is.EqualTo(0),
                "Spurious result left by Assert.Fail()");
        }

        private Exception CatchException(TestDelegate del)
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
