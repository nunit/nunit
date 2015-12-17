#if NET_4_5 || PORTABLE
using System;
using System.Threading.Tasks;
using NUnit.TestUtilities;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class AssertThrowsAsyncTests
    {
        [Test]
        public async Task CorrectExceptionThrownSync()
        {
            await Assert.ThrowsAsync(typeof(ArgumentException), AsyncTestDelegates.ThrowsArgumentExceptionSync);
            await Assert.ThrowsAsync(typeof(ArgumentException), delegate { throw new ArgumentException(); });

            await Assert.ThrowsAsync<ArgumentException>(delegate { throw new ArgumentException(); });
            await Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentExceptionSync);
        }

        [Test]
        public async Task CorrectExceptionThrownAsync()
        {
            await Assert.ThrowsAsync(typeof(ArgumentException), AsyncTestDelegates.ThrowsArgumentExceptionAsync);
            await Assert.ThrowsAsync(typeof(ArgumentException), async () =>
            {
                await Task.Yield();
                throw new ArgumentException();
            });

            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await Task.Yield();
                throw new ArgumentException();
            });
            await Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentExceptionAsync);
        }

        [Test]
        public async Task CorrectExceptionIsReturnedToMethodSync()
        {
            ArgumentException ex = await Assert.ThrowsAsync(typeof (ArgumentException), AsyncTestDelegates.ThrowsArgumentExceptionSync) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = await Assert.ThrowsAsync<ArgumentException>(delegate { throw new ArgumentException("myMessage", "myParam"); });

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = await Assert.ThrowsAsync(typeof(ArgumentException), delegate { throw new ArgumentException("myMessage", "myParam"); }) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = await Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentExceptionSync);

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif
        }

        [Test]
        public async Task CorrectExceptionIsReturnedToMethodAsync()
        {
            ArgumentException ex = await Assert.ThrowsAsync(typeof(ArgumentException), AsyncTestDelegates.ThrowsArgumentExceptionAsync) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await Task.Yield();
                throw new ArgumentException("myMessage", "myParam");
            });

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = await Assert.ThrowsAsync(typeof(ArgumentException), async () =>
            {
                await Task.Yield();
                throw new ArgumentException("myMessage", "myParam");
            }) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif

            ex = await Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsArgumentExceptionAsync);

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
#if !NETCF && !SILVERLIGHT && !PORTABLE
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));
#endif
        }

        [Test]
        public async Task NoExceptionThrown()
        {
            var ex = await CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsNothing));
            Assert.That(ex.Message, Is.EqualTo(
                "  Expected: <System.ArgumentException>" + Env.NewLine +
                "  But was:  null" + Env.NewLine));
        }

        [Test]
        public async Task UnrelatedExceptionThrownSync()
        {
            var ex = await CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsNullReferenceExceptionSync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Env.NewLine +
                "  But was:  <System.NullReferenceException: my message" + Env.NewLine));
        }

        [Test]
        public async Task UnrelatedExceptionThrownAsync()
        {
            var ex = await CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsNullReferenceExceptionAsync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Env.NewLine +
                "  But was:  <System.NullReferenceException: my message" + Env.NewLine));
        }

        [Test]
        public async Task BaseExceptionThrownSync()
        {
            var ex = await CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsSystemExceptionSync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Env.NewLine +
                "  But was:  <System.Exception: my message" + Env.NewLine));
        }

        [Test]
        public async Task BaseExceptionThrownAsync()
        {
            var ex = await CatchException(() => Assert.ThrowsAsync<ArgumentException>(AsyncTestDelegates.ThrowsSystemExceptionAsync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Env.NewLine +
                "  But was:  <System.Exception: my message" + Env.NewLine));
        }

        [Test]
        public async Task DerivedExceptionThrownSync()
        {
            var ex = await CatchException(() => Assert.ThrowsAsync<Exception>(AsyncTestDelegates.ThrowsArgumentExceptionSync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.Exception>" + Env.NewLine +
                "  But was:  <System.ArgumentException: myMessage" + Env.NewLine + "Parameter name: myParam" + Env.NewLine));
        }

        [Test]
        public async Task DerivedExceptionThrownAsync()
        {
            var ex = await CatchException(() => Assert.ThrowsAsync<Exception>(AsyncTestDelegates.ThrowsArgumentExceptionAsync));
            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.Exception>" + Env.NewLine +
                "  But was:  <System.ArgumentException: myMessage" + Env.NewLine + "Parameter name: myParam" + Env.NewLine));
        }

        [Test]
        public async Task DoesNotThrowSuceeds()
        {
            await Assert.DoesNotThrowAsync(AsyncTestDelegates.ThrowsNothing);
        }

        [Test]
        public async Task DoesNotThrowFailsSync()
        {
            var ex = await CatchException(() => Assert.DoesNotThrowAsync(AsyncTestDelegates.ThrowsArgumentExceptionSync));
            Assert.That(ex, Is.Not.Null.With.TypeOf<AssertionException>());
        }

        [Test]
        public async Task DoesNotThrowFailsAsync()
        {
            var ex = await CatchException(() => Assert.DoesNotThrowAsync(AsyncTestDelegates.ThrowsArgumentExceptionAsync));
            Assert.That(ex, Is.Not.Null.With.TypeOf<AssertionException>());
        }

        private async Task<Exception> CatchException(AsyncTestDelegate del)
        {
            try
            {
                await del();
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
