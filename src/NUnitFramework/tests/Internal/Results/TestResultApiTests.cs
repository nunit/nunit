// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal.Results
{
    public sealed class TestResultApiTests : TestResultTests
    {
        private IEnumerable<TestResult> TestResults => new[]
        {
            TestResult,
            SuiteResult
        };

        private static IEnumerable<Action<TestResult, Exception>> RecordExceptionMethods => new Action<TestResult, Exception>[]
        {
            (result, exception) => result.RecordException(exception, FailureSite.SetUp),
            (result, exception) => result.RecordException(exception, FailureSite.Test),
            (result, exception) => result.RecordException(exception, FailureSite.TearDown)
        };

        [Test]
        public void ThrowsForNullException()
        {
            foreach (var method in RecordExceptionMethods)
            {
                foreach (var result in TestResults)
                {
                    Assert.That(
                        () => method.Invoke(result, null!),
                        Throws.ArgumentNullException.With.Property("ParamName").EqualTo("ex"));
                }
            }
        }

        [Test]
        public void DoesNotThrowForMissingInnerException()
        {
            var exceptions = new Exception[]
            {
                new NUnitException(),
                new TargetInvocationException(null),
                new AggregateException()
            };

            foreach (var method in RecordExceptionMethods)
            {
                foreach (var result in TestResults)
                {
                    foreach (var exception in exceptions)
                    {
                        method.Invoke(result, exception);
                    }
                }
            }
        }
    }
}
