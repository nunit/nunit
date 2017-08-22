// ***********************************************************************
// Copyright (c) 2008-2016 Charlie Poole, Rob Prouse
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

#if ASYNC
using System;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

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

        [Test]
        public void DerivedExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<Exception>(AsyncTestDelegates.ThrowsArgumentException));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.Exception>" + Environment.NewLine +
                "  But was:  <System.ArgumentException: myMessage" + Environment.NewLine + "Parameter name: myParam" + Environment.NewLine));

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void DerivedExceptionThrownAsync()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<Exception>(AsyncTestDelegates.ThrowsArgumentExceptionAsync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.Exception>" + Environment.NewLine +
                "  But was:  <System.ArgumentException: myMessage" + Environment.NewLine + "Parameter name: myParam" + Environment.NewLine));
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
#endif
