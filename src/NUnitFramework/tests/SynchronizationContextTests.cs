// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using System.Threading.Tasks;
using NUnit.TestData;

namespace NUnit.Framework
{
    public static class SynchronizationContextTests
    {
        [Test]
        public static void SynchronizationContextIsRestoredBetweenTestCases()
        {
            using (TestUtils.RestoreSynchronizationContext()) // Restore the synchronization context so as not to affect other tests if this test fails
            {
                var result = TestBuilder.RunParameterizedMethodSuite(
                    typeof(SynchronizationContextFixture),
                    nameof(SynchronizationContextFixture.TestCasesThatSetSynchronizationContext));

                result.AssertPassed();
            }
        }

        [Test]
        public static void SynchronizationContextIsRestoredBetweenTestCaseSources()
        {
            using (TestUtils.RestoreSynchronizationContext()) // Restore the synchronization context so as not to affect other tests if this test fails
            {
                var fixture = TestBuilder.MakeFixture(typeof(SynchronizationContextFixture));

                foreach (var name in new[]
                {
                    nameof(SynchronizationContextFixture.TestMethodWithSourceThatSetsSynchronizationContext1),
                    nameof(SynchronizationContextFixture.TestMethodWithSourceThatSetsSynchronizationContext2)
                })
                {
                    var parameterizedSuite = fixture.Tests.Single(t => t.Method.Name == name);
                    Assert.That(parameterizedSuite.Tests.Single().Arguments.Single(), Is.True);
                }
            }
        }

        public static IEnumerable<AsyncExecutionApiAdapter> ApiAdapters => AsyncExecutionApiAdapter.All;

#if NETCOREAPP
        [Platform(Include = "Win, Mono")]
#endif
        [Apartment(ApartmentState.STA)]
        [TestCaseSource(nameof(ApiAdapters))]
        public static void ContinuationStaysOnStaThread(AsyncExecutionApiAdapter apiAdapter)
        {
            var thread = Thread.CurrentThread;

            apiAdapter.Execute(async () =>
            {
                await Task.Yield();
                Assert.That(Thread.CurrentThread, Is.SameAs(thread));
            });
        }

#if NETCOREAPP
        [Platform(Include = "Win, Mono")]
#endif
        [Apartment(ApartmentState.STA)]
        [TestCaseSource(nameof(ApiAdapters))]
        public static void AsyncDelegatesAreExecutedOnStaThread(AsyncExecutionApiAdapter apiAdapter)
        {
            var thread = Thread.CurrentThread;

            apiAdapter.Execute(() =>
            {
                Assert.That(Thread.CurrentThread, Is.SameAs(thread));
                return Task.FromResult<object>(null);
            });
        }

#if UseWindowsFormsAndWPF
        // TODO: test a custom awaitable type whose awaiter executes continuations on a brand new thread
        // to ensure that the message pump is shut down on the correct thread.

        public static readonly IEnumerable<Type> KnownSynchronizationContextTypes = new[]
        {
            typeof(System.Windows.Forms.WindowsFormsSynchronizationContext),
            typeof(System.Windows.Threading.DispatcherSynchronizationContext)
        };

        private static SynchronizationContext CreateSynchronizationContext(Type knownSynchronizationContextType)
        {
            if (new PlatformHelper().IsPlatformSupported("Mono"))
            {
                if (knownSynchronizationContextType == typeof(System.Windows.Threading.DispatcherSynchronizationContext))
                {
                    Assert.Ignore("DispatcherSynchronizationContext throws NotImplementedException on Mono.");
                }
                else if (knownSynchronizationContextType == typeof(System.Windows.Forms.WindowsFormsSynchronizationContext))
                {
                    if (!Environment.UserInteractive)
                    {
                        Assert.Inconclusive("WindowsFormsSynchronizationContext throws ArgumentNullException on Mono when not running interactively.");
                    }
                }
            }

            return (SynchronizationContext)Activator.CreateInstance(knownSynchronizationContextType);
        }

        [Test, Timeout(10_000)]
        public static void ContinuationDoesNotDeadlockOnKnownSynchronizationContext(
            [ValueSource(nameof(KnownSynchronizationContextTypes))] Type knownSynchronizationContextType,
            [ValueSource(nameof(ApiAdapters))] AsyncExecutionApiAdapter apiAdapter)
        {
            var createdOnThisThread = CreateSynchronizationContext(knownSynchronizationContextType);

            using (TestUtils.TemporarySynchronizationContext(createdOnThisThread))
            {
                apiAdapter.Execute(async () => await TaskEx.Yield());
            }
        }

        [Test]
        public static void AsyncDelegatesAreExecutedUnderTheCurrentSynchronizationContext(
            [ValueSource(nameof(KnownSynchronizationContextTypes))] Type knownSynchronizationContextType,
            [ValueSource(nameof(ApiAdapters))] AsyncExecutionApiAdapter apiAdapter)
        {
            var createdOnThisThread = CreateSynchronizationContext(knownSynchronizationContextType);

            using (TestUtils.TemporarySynchronizationContext(createdOnThisThread))
            {
                apiAdapter.Execute(() =>
                {
                    Assert.That(SynchronizationContext.Current, Is.TypeOf(knownSynchronizationContextType));
                    return TaskEx.FromResult<object>(null);
                });
            }
        }

        [Test, Timeout(10_000)]
        public static void AwaitingContinuationDoesNotAlterSynchronizationContext(
            [ValueSource(nameof(KnownSynchronizationContextTypes))] Type knownSynchronizationContextType,
            [ValueSource(nameof(ApiAdapters))] AsyncExecutionApiAdapter apiAdapter)
        {
            var createdOnThisThread = CreateSynchronizationContext(knownSynchronizationContextType);

            using (TestUtils.TemporarySynchronizationContext(createdOnThisThread))
            {
                apiAdapter.Execute(async () => await TaskEx.Yield());

                Assert.That(SynchronizationContext.Current, Is.TypeOf(knownSynchronizationContextType));
            }
        }
#endif
    }
}
