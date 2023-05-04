// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Delegate used by tests that execute code and
    /// capture any thrown exception.
    /// </summary>
    public delegate void TestDelegate();

    /// <summary>
    /// Delegate used by tests that execute async code and
    /// capture any thrown exception.
    /// </summary>
    public delegate System.Threading.Tasks.Task AsyncTestDelegate();

    /// <summary>
    /// The Assert class contains a collection of static methods that
    /// implement the most common assertions used in NUnit.
    /// </summary>
    // Abstract because we support syntax extension by inheriting and declaring new static members.
    public abstract partial class Assert
    {
        #region Equals and ReferenceEquals

        /// <summary>
        /// DO NOT USE! Use Assert.AreEqual(...) instead.
        /// The Equals method throws an InvalidOperationException. This is done
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object a, object b)
        {
            throw new InvalidOperationException("Assert.Equals should not be used. Use Assert.AreEqual instead.");
        }

        /// <summary>
        /// DO NOT USE!
        /// The ReferenceEquals method throws an InvalidOperationException. This is done
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new void ReferenceEquals(object a, object b)
        {
            throw new InvalidOperationException("Assert.ReferenceEquals should not be used. Use Assert.AreSame instead.");
        }

        #endregion

        #region Charlie

        private const string CHARLIE_APPRECIATION = "Charlie Poole led NUnit for 20+ years, across at least 207 releases in 37 different repositories, authoring 4,898 commits across them. He participated in 2,990 issues, 1,305 PRs, and impacted 6,992,983 lines of code. NUnit was downloaded from NuGet 225+ million times during his tenure. And those numbers don't include at least 9 additional years of his work. This assertion attempts to pay homage to Charlie, who by virtue of his contributions has helped untold millions of tests pass.";

        /// <summary>
        /// An alias of the corresponding Assert.Pass() method. Charlie Poole was the lead of NUnit for 21 years,
        /// across at least 207 releases in 37 different repositories, authoring 4,898 commits across them.
        /// He participated in 2,990 issues, 1,305 PRs, and impacted 6,992,983 lines of code. NUnit was downloaded from NuGet 225+ million times during his tenure.
        /// And those are only the numbers ones we can easily find; our numbers are sourced from after NUnit moved the project to GitHub in 2011,
        /// which means there are at least 9 additional years of work not quantified above.
        ///
        /// This assertion attempts to pay homage to Charlie, who by virtue of his contributions has helped untold millions of tests pass.
        /// </summary>
        [DoesNotReturn]
        static public void Charlie()
        {
            Assert.Pass(CHARLIE_APPRECIATION);
        }

        #endregion

        #region Pass

        /// <summary>
        /// Throws a <see cref="SuccessException"/> with the message and arguments
        /// that are passed in. This allows a test to be cut short, with a result
        /// of success returned to NUnit.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        [DoesNotReturn]
        static public void Pass(string? message, params object?[]? args)
        {
            if (message is null) message = string.Empty;
            else if (args is not null && args.Length > 0)
                message = string.Format(message, args);

            // If we are in a multiple assert block, this is an error
            if (TestExecutionContext.CurrentContext.MultipleAssertLevel > 0)
                throw new Exception("Assert.Pass may not be used in a multiple assertion block.");

            throw new SuccessException(message);
        }

        /// <summary>
        /// Throws a <see cref="SuccessException"/> with the message and arguments
        /// that are passed in. This allows a test to be cut short, with a result
        /// of success returned to NUnit.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        [DoesNotReturn]
        static public void Pass(string? message)
        {
            Assert.Pass(message, null);
        }

        /// <summary>
        /// Throws a <see cref="SuccessException"/> with the message and arguments
        /// that are passed in. This allows a test to be cut short, with a result
        /// of success returned to NUnit.
        /// </summary>
        [DoesNotReturn]
        static public void Pass()
        {
            Assert.Pass(string.Empty, null);
        }

        #endregion

        #region Fail

        /// <summary>
        /// Marks the test as failed with the message and arguments that are passed in. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Fail(string? message, params object?[]? args)
        {
            if (message is null) message = string.Empty;
            else if (args is not null && args.Length > 0)
                message = string.Format(message, args);

            ReportFailure(message);
        }

        /// <summary>
        /// Marks the test as failed with the message that is passed in. Returns without throwing an exception when
        /// inside a multiple assert block.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        static public void Fail(string? message)
        {
            Assert.Fail(message, null);
        }

        /// <summary>
        /// Marks the test as failed. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        static public void Fail()
        {
            Assert.Fail(string.Empty, null);
        }

        #endregion

        #region Warn

        /// <summary>
        /// Issues a warning using the message and arguments provided.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Warn(string? message, params object?[]? args)
        {
            if (message is null) message = string.Empty;
            else if (args is not null && args.Length > 0)
                message = string.Format(message, args);

            IssueWarning(message);
        }

        /// <summary>
        /// Issues a warning using the message provided.
        /// </summary>
        /// <param name="message">The message to display.</param>
        static public void Warn(string? message)
        {
            IssueWarning(message);
        }

        #endregion

        #region Ignore

        /// <summary>
        /// Throws an <see cref="IgnoreException"/> with the message and arguments
        /// that are passed in.  This causes the test to be reported as ignored.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        [DoesNotReturn]
        static public void Ignore(string? message, params object?[]? args)
        {
            if (message is null) message = string.Empty;
            else if (args is not null && args.Length > 0)
                message = string.Format(message, args);

            // If we are in a multiple assert block, this is an error
            if (TestExecutionContext.CurrentContext.MultipleAssertLevel > 0)
                throw new Exception("Assert.Ignore may not be used in a multiple assertion block.");

            throw new IgnoreException(message);
        }

        /// <summary>
        /// Throws an <see cref="IgnoreException"/> with the message that is
        /// passed in. This causes the test to be reported as ignored.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        [DoesNotReturn]
        static public void Ignore(string? message)
        {
            Assert.Ignore(message, null);
        }

        /// <summary>
        /// Throws an <see cref="IgnoreException"/>.
        /// This causes the test to be reported as ignored.
        /// </summary>
        [DoesNotReturn]
        static public void Ignore()
        {
            Assert.Ignore(string.Empty, null);
        }

        #endregion

        #region Inconclusive

        /// <summary>
        /// Throws an <see cref="InconclusiveException"/> with the message and arguments
        /// that are passed in.  This causes the test to be reported as inconclusive.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="InconclusiveException"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        [DoesNotReturn]
        static public void Inconclusive(string? message, params object?[]? args)
        {
            if (message is null) message = string.Empty;
            else if (args is not null && args.Length > 0)
                message = string.Format(message, args);

            // If we are in a multiple assert block, this is an error
            if (TestExecutionContext.CurrentContext.MultipleAssertLevel > 0)
                throw new Exception("Assert.Inconclusive may not be used in a multiple assertion block.");

            throw new InconclusiveException(message);
        }

        /// <summary>
        /// Throws an <see cref="InconclusiveException"/> with the message that is
        /// passed in. This causes the test to be reported as inconclusive.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="InconclusiveException"/> with.</param>
        [DoesNotReturn]
        static public void Inconclusive(string? message)
        {
            Assert.Inconclusive(message, null);
        }

        /// <summary>
        /// Throws an <see cref="InconclusiveException"/>.
        /// This causes the test to be reported as Inconclusive.
        /// </summary>
        [DoesNotReturn]
        static public void Inconclusive()
        {
            Assert.Inconclusive(string.Empty, null);
        }

        #endregion

        #region Contains

        /// <summary>
        /// Asserts that an object is contained in a collection. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The collection to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Contains(object? expected, ICollection? actual, string? message, params object?[]? args)
        {
            Assert.That(actual, new SomeItemsConstraint(new EqualConstraint(expected)) ,message, args);
        }

        /// <summary>
        /// Asserts that an object is contained in a collection. Returns without throwing an exception when inside a
        /// multiple assert block.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The collection to be examined</param>
        public static void Contains(object? expected, ICollection? actual)
        {
            Assert.That(actual, new SomeItemsConstraint(new EqualConstraint(expected)) ,null, null);
        }

        #endregion

        #region Multiple

        /// <summary>
        /// Wraps code containing a series of assertions, which should all
        /// be executed, even if they fail. Failed results are saved and
        /// reported at the end of the code block.
        /// </summary>
        /// <param name="testDelegate">A TestDelegate to be executed in Multiple Assertion mode.</param>
        public static void Multiple(TestDelegate testDelegate)
        {
            TestExecutionContext context = TestExecutionContext.CurrentContext;
            Guard.OperationValid(context is not null, "There is no current test execution context.");

            var oldCount = context.CurrentResult.AssertionResults.Count;
            context.MultipleAssertLevel++;

            try
            {
                testDelegate();
            }
            finally
            {
                context.MultipleAssertLevel--;
            }

            if (context.MultipleAssertLevel == 0 && context.CurrentResult.PendingFailures > 0)
            {
                context.CurrentResult.RecordTestCompletion();
                if (context.CurrentResult.AssertionResults.Count > oldCount)
                {
                    throw new MultipleAssertException(context.CurrentResult);
                }
            }
        }

        /// <summary>
        /// Wraps code containing a series of assertions, which should all
        /// be executed, even if they fail. Failed results are saved and
        /// reported at the end of the code block.
        /// </summary>
        /// <param name="testDelegate">A TestDelegate to be executed in Multiple Assertion mode.</param>
        public static void Multiple(AsyncTestDelegate testDelegate)
        {
            TestExecutionContext context = TestExecutionContext.CurrentContext;
            Guard.OperationValid(context is not null, "There is no current test execution context.");

            var oldCount = context.CurrentResult.AssertionResults.Count;
            context.MultipleAssertLevel++;

            try
            {
                AsyncToSyncAdapter.Await(testDelegate.Invoke);
            }
            finally
            {
                context.MultipleAssertLevel--;
            }

            if (context.MultipleAssertLevel == 0 && context.CurrentResult.PendingFailures > 0)
            {
                context.CurrentResult.RecordTestCompletion();
                if (context.CurrentResult.AssertionResults.Count > oldCount)
                {
                    throw new MultipleAssertException(context.CurrentResult);
                }
            }
        }

        /// <summary>
        /// Wraps code containing a series of assertions, which should all
        /// be executed, even if they fail. Failed results are saved and
        /// reported at the end of the code block.
        /// </summary>
        /// <param name="testDelegate">An AsyncTestDelegate to be executed in Multiple Assertion mode.</param>
        public static async Task MultipleAsync(AsyncTestDelegate testDelegate)
        {
            TestExecutionContext context = TestExecutionContext.CurrentContext;
            Guard.OperationValid(context is not null, "There is no current test execution context.");

            context.MultipleAssertLevel++;

            try
            {
                await testDelegate();
            }
            finally
            {
                context.MultipleAssertLevel--;
            }

            if (context.MultipleAssertLevel == 0 && context.CurrentResult.PendingFailures > 0)
            {
                context.CurrentResult.RecordTestCompletion();
                throw new MultipleAssertException(context.CurrentResult);
            }
        }

        #endregion

        #region Helper Methods

        private static void ReportFailure(ConstraintResult result, string? message)
        {
            ReportFailure(result, message, null);
        }

        private static void ReportFailure(ConstraintResult result, string? message, params object?[]? args)
        {
            MessageWriter writer = new TextMessageWriter(message, args);
            result.WriteMessageTo(writer);

            ReportFailure(writer.ToString());
        }

        private static void ReportFailure(string? message)
        {
            // Record the failure in an <assertion> element
            var result = TestExecutionContext.CurrentContext.CurrentResult;
            result.RecordAssertion(AssertionStatus.Failed, message, GetStackTrace());
            result.RecordTestCompletion();

            // If we are outside any multiple assert block, then throw
            if (TestExecutionContext.CurrentContext.MultipleAssertLevel == 0)
                throw new AssertionException(result.Message);
        }

        private static void IssueWarning(string? message)
        {
            var result = TestExecutionContext.CurrentContext.CurrentResult;
            result.RecordAssertion(AssertionStatus.Warning, message, GetStackTrace());
        }

        // System.Environment.StackTrace puts extra entries on top of the stack, at least in some environments
        private static readonly StackFilter SystemEnvironmentFilter = new StackFilter(@" System\.Environment\.");

        private static string? GetStackTrace() =>
            StackFilter.DefaultFilter.Filter(SystemEnvironmentFilter.Filter(GetEnvironmentStackTraceWithoutThrowing()));

        /// <summary>
        /// If <see cref="Exception.StackTrace"/> throws, returns "SomeException was thrown by the
        /// Environment.StackTrace property." See also <see cref="ExceptionExtensions.GetStackTraceWithoutThrowing"/>.
        /// </summary>
        // https://github.com/dotnet/coreclr/issues/19698 is also currently present in .NET Framework 4.7 and 4.8. A
        // race condition between threads reading the same PDB file to obtain file and line info for a stack trace
        // results in AccessViolationException when the stack trace is accessed even indirectly e.g. Exception.ToString.
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        private static string GetEnvironmentStackTraceWithoutThrowing()
        {
            try
            {
                return Environment.StackTrace;
            }
            catch (Exception ex)
            {
                return ex.GetType().Name + " was thrown by the Environment.StackTrace property.";
            }
        }

        private static void IncrementAssertCount()
        {
            TestExecutionContext.CurrentContext.IncrementAssertCount();
        }

#endregion
    }
}
