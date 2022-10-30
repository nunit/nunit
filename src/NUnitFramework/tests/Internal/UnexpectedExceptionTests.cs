// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.TestData.UnexpectedExceptionFixture;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class UnexpectedExceptionTests
    {
        private static ITestResult RunDataTestCase(string methodName)
        {
            return TestBuilder.RunTestCase(typeof(UnexpectedExceptionFixture), methodName);
        }

        [Test]
        public void FailRecordsInnerException()
        {
            string expectedMessage =
                "System.Exception : Outer Exception" + Environment.NewLine + "  ----> System.Exception : Inner Exception";

            ITestResult result = RunDataTestCase(nameof(UnexpectedExceptionFixture.ThrowsWithInnerException));

            Assert.AreEqual(ResultState.Error, result.ResultState);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [Test]
        public void FailRecordsNestedInnerException()
        {
            string expectedMessage =
                "System.Exception : Outer Exception" + Environment.NewLine +
                "  ----> System.Exception : Inner Exception" + Environment.NewLine +
                "  ----> System.Exception : Inner Inner Exception";

            ITestResult result = RunDataTestCase(nameof(UnexpectedExceptionFixture.ThrowsWithNestedInnerException));

            Assert.AreEqual(ResultState.Error, result.ResultState);
            Assert.AreEqual(expectedMessage, result.Message);
        }

        [Test]
        public void FailRecordsInnerExceptionsAsPartOfAggregateException()
        {
            string expectedStartOfMessage = "System.AggregateException : Outer Aggregate Exception";
            string expectedEndOfMessage =
                "  ----> System.Exception : Inner Exception 1 of 2" + Environment.NewLine +
                "  ----> System.Exception : Inner Exception 2 of 2";

            ITestResult result = RunDataTestCase(nameof(UnexpectedExceptionFixture.ThrowsWithAggregateException));

            Assert.AreEqual(ResultState.Error, result.ResultState);
            Assert.That(result.Message, Does.StartWith(expectedStartOfMessage));
            Assert.That(result.Message, Does.EndWith(expectedEndOfMessage));
        }

        [Test]
        public void FailRecordsNestedInnerExceptionAsPartOfAggregateException()
        {
            string expectedStartOfMessage = "System.AggregateException : Outer Aggregate Exception";
            string expectedEndOfMessage =
                "  ----> System.Exception : Inner Exception" + Environment.NewLine +
                "  ----> System.Exception : Inner Inner Exception";

            ITestResult result = RunDataTestCase(nameof(UnexpectedExceptionFixture.ThrowsWithAggregateExceptionContainingNestedInnerException));

            Assert.AreEqual(ResultState.Error, result.ResultState);
            Assert.That(result.Message, Does.StartWith(expectedStartOfMessage));
            Assert.That(result.Message, Does.EndWith(expectedEndOfMessage));
        }

        [Test]
        public void BadStackTraceIsHandled()
        {
            ITestResult result = RunDataTestCase(nameof(UnexpectedExceptionFixture.ThrowsWithBadStackTrace));

            Assert.AreEqual(ResultState.Error, result.ResultState);
            Assert.AreEqual("NUnit.TestData.UnexpectedExceptionFixture.ExceptionWithBadStackTrace : thrown by me", result.Message);
            Assert.AreEqual("InvalidOperationException was thrown by the Exception.StackTrace property.", result.StackTrace);
        }

        [Test]
        public void CustomExceptionIsHandled()
        {
            ITestResult result = RunDataTestCase(nameof(UnexpectedExceptionFixture.ThrowsCustomException));

            Assert.AreEqual(ResultState.Error, result.ResultState);
            Assert.AreEqual("NUnit.TestData.UnexpectedExceptionFixture.CustomException : message", result.Message);
        }

        [Test]
        public void AssertThatCanHandleRecursivelyThrowingExceptionAsActual()
        {
            var result = (ITestResult)null;

            var ex = RecordPossiblyDangerousException(() =>
                result = RunDataTestCase(nameof(UnexpectedExceptionFixture.AssertThatWithRecursivelyThrowingExceptionAsActual)));

            Assert.That(ex is null); // Careful not to pass ex to Assert.That and crash the test run rather than failing

            Assert.That(result, Has.Property("Message").StartWith(
                "  Expected: null" + Environment.NewLine
                + "  But was:  <! RecursivelyThrowingException was thrown by RecursivelyThrowingException.ToString() !>"));
        }

        [Test]
        public void AssertThatCanHandleRecursivelyThrowingExceptionAsExpected()
        {
            var result = (ITestResult)null;

            var ex = RecordPossiblyDangerousException(() =>
                result = RunDataTestCase(nameof(UnexpectedExceptionFixture.AssertThatWithRecursivelyThrowingExceptionAsExpected)));

            Assert.That(ex is null); // Careful not to pass ex to Assert.That and crash the test run rather than failing

            Assert.That(result, Has.Property("Message").StartWith(
                "  Expected: <! RecursivelyThrowingException was thrown by RecursivelyThrowingException.ToString() !>" + Environment.NewLine
                + "  But was:  null"));
        }

        [Test]
        public void RecordExceptionCanHandleRecursivelyThrowingException()
        {
            var result = new TestCaseResult(new TestMethod(new MethodWrapper(typeof(UnexpectedExceptionTests), nameof(DummyMethod))));

            var ex = RecordPossiblyDangerousException(() =>
                result.RecordException(new RecursivelyThrowingException()));

            Assert.That(ex is null); // Careful not to pass ex to Assert.That and crash the test run rather than failing

            Assert.That(result, Has.Property("Message").StartWith(
                "NUnit.TestData.UnexpectedExceptionFixture.RecursivelyThrowingException : RecursivelyThrowingException was thrown by the Exception.Message property." + Environment.NewLine
                + "RecursivelyThrowingException was thrown by the Exception.Data property."));

            Assert.That(result, Has.Property("StackTrace").EqualTo(
                "RecursivelyThrowingException was thrown by the Exception.StackTrace property."));
        }

        [Test]
        public void RecordExceptionWithSiteCanHandleRecursivelyThrowingException()
        {
            var result = new TestCaseResult(new TestMethod(new MethodWrapper(typeof(UnexpectedExceptionTests), nameof(DummyMethod))));

            var ex = RecordPossiblyDangerousException(() =>
                result.RecordException(new RecursivelyThrowingException(), FailureSite.Test));

            Assert.That(ex is null); // Careful not to pass ex to Assert.That and crash the test run rather than failing

            Assert.That(result, Has.Property("Message").StartWith(
                "NUnit.TestData.UnexpectedExceptionFixture.RecursivelyThrowingException : RecursivelyThrowingException was thrown by the Exception.Message property." + Environment.NewLine
                + "RecursivelyThrowingException was thrown by the Exception.Data property."));

            Assert.That(result, Has.Property("StackTrace").EqualTo(
                "RecursivelyThrowingException was thrown by the Exception.StackTrace property."));
        }

        [Test]
        public void RecordTearDownExceptionCanHandleRecursivelyThrowingException()
        {
            var result = new TestCaseResult(new TestMethod(new MethodWrapper(typeof(UnexpectedExceptionTests), nameof(DummyMethod))));

            var ex = RecordPossiblyDangerousException(() =>
                result.RecordTearDownException(new RecursivelyThrowingException()));

            Assert.That(ex is null); // Careful not to pass ex to Assert.That and crash the test run rather than failing

            Assert.That(result, Has.Property("Message").StartWith(
                "TearDown : NUnit.TestData.UnexpectedExceptionFixture.RecursivelyThrowingException : RecursivelyThrowingException was thrown by the Exception.Message property." + Environment.NewLine
                + "RecursivelyThrowingException was thrown by the Exception.Data property."));

            Assert.That(result, Has.Property("StackTrace").EqualTo(
                "--TearDown" + Environment.NewLine
                + "RecursivelyThrowingException was thrown by the Exception.StackTrace property."));
        }

        public void DummyMethod()
        {
        }

#pragma warning disable SYSLIB0032
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
#pragma warning restore SYSLIB0032
        private static Exception RecordPossiblyDangerousException(Action action)
        {
            try
            {
                action.Invoke();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}
