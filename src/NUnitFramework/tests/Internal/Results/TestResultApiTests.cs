// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Results
{
    public sealed class TestResultApiTests : TestResultTests
    {
        private IEnumerable<TestResult> TestResults => new[]
        {
            _testResult,
            _suiteResult
        };

        private static IEnumerable<Action<TestResult, Exception>> RecordExceptionMethods => new Action<TestResult, Exception>[]
        {
            (result, exception) => result.RecordException(exception),
            (result, exception) => result.RecordException(exception, FailureSite.Test),
            (result, exception) => result.RecordTearDownException(exception)
        };

        [Test]
        public void ThrowsForNullException()
        {
            foreach (var method in RecordExceptionMethods)
            foreach (var result in TestResults)
            {
                Assert.That(
                    () => method.Invoke(result, null),
                    Throws.ArgumentNullException.With.Property("ParamName").EqualTo("ex"));
            }
        }


        [Test]
        public void DoesNotThrowForMissingInnerException()
        {
            var exceptions = new Exception[]
            {
                new NUnitException(),
                new TargetInvocationException(null),
#if ASYNC
                new AggregateException()
#endif
            };

            foreach (var method in RecordExceptionMethods)
            foreach (var result in TestResults)
            foreach (var exception in exceptions)
            {
                method.Invoke(result, exception);
            }
        }
    }
}
