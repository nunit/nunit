// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// Unhandled exceptions can be caught but regardless will always terminate the application
// See for a proposal to change this:
// https://github.com/dotnet/runtime/issues/42275
// https://github.com/dotnet/runtime/issues/101560
//
// NET10 and later have a new API for allowing an exception to be marked as handled.
// https://learn.microsoft.com/en-us/dotnet/api/system.runtime.exceptionservices.exceptionhandling.setunhandledexceptionhandler?view=net-10.0
// This requires the NUnit Framework itself to be built with .NET 10.0 or later.
// But even then we have the problem that the handler runs on a different thread
// and thus cannot find the current test to mark it as failed or ignored.

// Limit tests to .NET Framework for now, later enable .NET 10.0+ if we can handle this.
#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    [Explicit("When running in Nunit-Lite it seems to crash the process.")]
    internal class UnhandledExceptionHandlingAttributeTests
    {
        // +/-1 because TestExceptionThrownInTask does not find the current test and thus does not cause an error.
#if THREAD_ABORT
        private const int ExpectedPassCount = 5;
        private const int ExpectedFailCount = 4;
#else
        private const int ExpectedPassCount = 2;
        private const int ExpectedFailCount = 3;
#endif

        private static readonly Dictionary<string, ResultState> ExpectedResults = new()
        {
            [nameof(UnhandledExceptionFixture.TestExceptionThrownInSpawnedThread)] = ResultState.Error,
            [nameof(UnhandledExceptionFixture.TestExceptionThrownInSpawnedThreadAllDirectedToBeIgnored)] = ResultState.Success,
            [nameof(UnhandledExceptionFixture.TestExceptionThrownInSpawnedThreadDirectedToBeIgnored)] = ResultState.Success,
            [nameof(UnhandledExceptionFixture.TestExceptionThrownInSpawnedThreadNotToBeIgnored)] = ResultState.Error,
            [nameof(UnhandledExceptionFixture.TestExceptionThrownInSpawnedThreadNotSupportedToBeIgnored)] = ResultState.Error,
#if TASK_EXCEPTION_FINDS_CONTEXT
            // This test does not find the current test and thus does not cause an error.
            [nameof(UnhandledExceptionFixture.TestExceptionThrownInTask)] = ResultState.Error,
            [nameof(UnhandledExceptionFixture.TestExceptionThrownInTaskDirectedToBeIgnored)] = ResultState.Success,
#endif
#if THREAD_ABORT
            [nameof(UnhandledExceptionFixture.TestThreadAbort)] = ResultState.Error,
            [nameof(UnhandledExceptionFixture.TestThreadAbortDirectedToBeIgnored)] = ResultState.Success,
            [nameof(UnhandledExceptionFixture.TestThreadAbortCaughtAndReset)] = ResultState.Success,
            [nameof(UnhandledExceptionFixture.TestThreadAbortCaughtAndResetDirectedToBeIgnored)] = ResultState.Success,
#endif
        };

        [Test]
        public void UnhandledExceptionsCauseFailuresUnlessDirectedToIgnore()
        {
            ITestResult result = TestBuilder.RunTestFixture(typeof(UnhandledExceptionFixture));

            AssertChildResults(result);

            Assert.That(result.FailCount, Is.EqualTo(ExpectedFailCount));
            Assert.That(result.PassCount, Is.EqualTo(ExpectedPassCount));
        }

        [Test]
        public void UnhandledExceptionsAreIgnored()
        {
            var result = TestBuilder.RunTestFixture(typeof(IgnoringUnhandledExceptionFixture));
            AssertChildResults(result, ResultState.Success);
        }

        [Test]
        public void AttributeOnMethodOverridesAttributeOnClass()
        {
            var result = TestBuilder.RunTestFixture(typeof(FailUnhandledExceptionFixture));
            AssertChildResults(result);
        }

        private static void AssertChildResults(ITestResult result, ResultState? overriddenResult = null)
        {
            Assert.That(result.InitiatedCount, Is.EqualTo(ExpectedPassCount + ExpectedFailCount));

            using (Assert.EnterMultipleScope())
            {
                foreach (var pair in ExpectedResults)
                {
                    ITestResult childResult = result.Children.Single(t => t.Name == pair.Key);
                    Assert.That(childResult.ResultState, Is.EqualTo(pair.Value).Or.EqualTo(overriddenResult),
                                $"{pair.Key}{Environment.NewLine}{childResult.Message}");
                }
            }
        }
    }

    [TestFixture]
    [Explicit]
    public class UnhandledExceptionHandlingAttributeTestsDirectRun : UnhandledExceptionFixture
    {
        // This is so we can run the test fixture to check the behaviour.
    }
}

#endif
