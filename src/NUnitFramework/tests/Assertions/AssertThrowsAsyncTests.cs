#if NET_4_0 || NET_4_5 || PORTABLE
using System;
using System.Threading.Tasks;
using NUnit.TestUtilities;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class AssertThrowsAsyncTests
    {
        [Test]
        public void CorrectExceptionThrown()
        {
            Assert.ThrowsAsync(typeof(ArgumentException), AsyncTestDelegates.ThrowsArgumentException);
            Assert.ThrowsAsync(typeof(ArgumentException),
                delegate { throw new ArgumentException(); });

            Assert.ThrowsAsync<ArgumentException>(
                delegate { throw new ArgumentException(); });
            Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentException);

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
        public void CorrectExceptionThrownAsync()
        {
            Assert.ThrowsAsync(typeof(ArgumentException), AsyncTestDelegates.ThrowsArgumentExceptionAsync);
            Assert.ThrowsAsync(typeof(ArgumentException),
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => { throw new ArgumentException(); }, TaskScheduler.Default); });

            Assert.ThrowsAsync<ArgumentException>(
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => { throw new ArgumentException(); }, TaskScheduler.Default); });
            Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentExceptionAsync);

            // Without cast, delegate is ambiguous before C# 3.0.
            Assert.That(
                (AsyncTestDelegate)delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => { throw new ArgumentException(); }, TaskScheduler.Default); },
                    Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void CorrectExceptionIsReturnedToMethod()
        {
            ArgumentException ex = Assert.ThrowsAsync(typeof(ArgumentException),
                new AsyncTestDelegate(AsyncTestDelegates.ThrowsArgumentException)) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = Assert.ThrowsAsync<ArgumentException>(
                delegate { throw new ArgumentException("myMessage", "myParam"); });

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = Assert.ThrowsAsync(typeof(ArgumentException),
                delegate { throw new ArgumentException("myMessage", "myParam"); }) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentException);

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif
        }

        [Test]
        public void CorrectExceptionIsReturnedToMethodAsync()
        {
            ArgumentException ex = Assert.ThrowsAsync(typeof(ArgumentException),
                new AsyncTestDelegate(AsyncTestDelegates.ThrowsArgumentExceptionAsync)) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = Assert.ThrowsAsync<ArgumentException>(
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => { throw new ArgumentException("myMessage", "myParam"); }, TaskScheduler.Default); });

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = Assert.ThrowsAsync(typeof(ArgumentException),
                delegate { return AsyncTestDelegates.Delay(5).ContinueWith(t => { throw new ArgumentException("myMessage", "myParam"); }, TaskScheduler.Default); }) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentExceptionAsync);

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif
        }


        [Test]
        public void NoExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsNothing));
            Assert.That(ex.Message, Is.EqualTo(
                "  Expected: <System.ArgumentException>" + Env.NewLine +
                "  But was:  null" + Env.NewLine));
        }

        [Test]
        public void UnrelatedExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsNullReferenceException));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Env.NewLine +
                "  But was:  <System.NullReferenceException: my message" + Env.NewLine));
        }

        [Test]
        public void UnrelatedExceptionThrownAsync()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsNullReferenceExceptionAsync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Env.NewLine +
                "  But was:  <System.NullReferenceException: my message" + Env.NewLine));
        }

        [Test]
        public void BaseExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsSystemException));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Env.NewLine +
                "  But was:  <System.Exception: my message" + Env.NewLine));
        }

        [Test]
        public void BaseExceptionThrownAsync()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsSystemExceptionAsync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Env.NewLine +
                "  But was:  <System.Exception: my message" + Env.NewLine));
        }

        [Test]
        public void DerivedExceptionThrown()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<Exception>(AsyncTestDelegates.ThrowsArgumentException));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.Exception>" + Env.NewLine +
                "  But was:  <System.ArgumentException: myMessage" + Env.NewLine + "Parameter name: myParam" + Env.NewLine));
        }

        [Test]
        public void DerivedExceptionThrownAsync()
        {
            var ex = CatchException(() => Assert.ThrowsAsync<Exception>(AsyncTestDelegates.ThrowsArgumentExceptionAsync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.Exception>" + Env.NewLine +
                "  But was:  <System.ArgumentException: myMessage" + Env.NewLine + "Parameter name: myParam" + Env.NewLine));
        }

        [Test]
        public void DoesNotThrowSuceeds()
        {
            Assert.DoesNotThrowAsync(AsyncTestDelegates.ThrowsNothing);
        }

        [Test]
        public void DoesNotThrowFails()
        {
            var ex = CatchException(() => Assert.DoesNotThrowAsync(AsyncTestDelegates.ThrowsArgumentException));
            Assert.That(ex, Is.Not.Null.With.TypeOf<AssertionException>());
        }

        [Test]
        public void DoesNotThrowFailsAsync()
        {
            var ex = CatchException(() => Assert.DoesNotThrowAsync(AsyncTestDelegates.ThrowsArgumentExceptionAsync));
            Assert.That(ex, Is.Not.Null.With.TypeOf<AssertionException>());
        }

        private Exception CatchException(TestDelegate del)
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
#endif
