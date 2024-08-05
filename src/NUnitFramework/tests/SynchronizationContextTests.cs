// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests
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
                    var parameterizedSuite = fixture.Tests.Single(t => t.Method?.Name == name);
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
                return Task.FromResult<object?>(null);
            });
        }
    }
}
