// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
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
    public delegate Task AsyncTestDelegate();

    /// <summary>
    /// The Assert class contains a collection of static methods that
    /// implement the most common assertions used in NUnit.
    /// </summary>
    // Abstract because we support syntax extension by inheriting and declaring new static members.
    public abstract partial class Assert : AssertBase
    {
        #region Equals and ReferenceEquals

        /// <summary>
        /// DO NOT USE! Use Assert.That(x,Is.EqualTo) instead.
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

        private const string CharlieAppreciation = "Charlie Poole led NUnit for 20+ years, across at least 207 releases in 37 different repositories, authoring 4,898 commits across them. He participated in 2,990 issues, 1,305 PRs, and impacted 6,992,983 lines of code. NUnit was downloaded from NuGet 225+ million times during his tenure. And those numbers don't include at least 9 additional years of his work. This assertion attempts to pay homage to Charlie, who by virtue of his contributions has helped untold millions of tests pass.";

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
        public static void Charlie()
        {
            Pass(CharlieAppreciation);
        }

        #endregion

        #region Pass

        /// <summary>
        /// Throws a <see cref="SuccessException"/> with the message and arguments
        /// that are passed in. This allows a test to be cut short, with a result
        /// of success returned to NUnit.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        [DoesNotReturn]
        public static void Pass(string? message)
        {
            // If we are in a multiple assert block, this is an error
            if (TestExecutionContext.CurrentContext.MultipleAssertLevel > 0)
                throw new Exception("Assert.Pass may not be used in a multiple assertion block.");
            message ??= string.Empty;
            throw new SuccessException(message);
        }

        /// <summary>
        /// Throws a <see cref="SuccessException"/> with the message and arguments
        /// that are passed in. This allows a test to be cut short, with a result
        /// of success returned to NUnit.
        /// </summary>
        [DoesNotReturn]
        public static void Pass()
        {
            Pass(string.Empty);
        }

        #endregion

        #region Fail

        /// <summary>
        /// Marks the test as failed with the message and arguments that are passed in. Returns without throwing an
        /// exception when inside a multiple assert block.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        public static void Fail(string? message)
        {
            message ??= string.Empty;
            ReportFailure(message);
        }

        /// <summary>
        /// Marks the test as failed. Returns without throwing an exception when inside a multiple assert block.
        /// </summary>
        public static void Fail()
        {
            Fail(string.Empty);
        }

        #endregion

        #region Warn

        /// <summary>
        /// Issues a warning using the message and arguments provided.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static void Warn(string? message)
        {
            message ??= string.Empty;
            IssueWarning(message);
        }

        #endregion

        #region Ignore

        /// <summary>
        /// Throws an <see cref="IgnoreException"/> with the message and arguments
        /// that are passed in.  This causes the test to be reported as ignored.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        [DoesNotReturn]
        public static void Ignore(string? message)
        {
            // If we are in a multiple assert block, this is an error
            if (TestExecutionContext.CurrentContext.MultipleAssertLevel > 0)
                throw new Exception("Assert.Ignore may not be used in a multiple assertion block.");

            throw new IgnoreException(message);
        }

        /// <summary>
        /// Throws an <see cref="IgnoreException"/>.
        /// This causes the test to be reported as ignored.
        /// </summary>
        [DoesNotReturn]
        public static void Ignore()
        {
            Ignore(string.Empty);
        }

        #endregion

        #region Inconclusive

        /// <summary>
        /// Throws an <see cref="InconclusiveException"/> with the message and arguments
        /// that are passed in.  This causes the test to be reported as inconclusive.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="InconclusiveException"/> with.</param>
        [DoesNotReturn]
        public static void Inconclusive(string? message)
        {
            // If we are in a multiple assert block, this is an error
            if (TestExecutionContext.CurrentContext.MultipleAssertLevel > 0)
                throw new Exception("Assert.Inconclusive may not be used in a multiple assertion block.");

            throw new InconclusiveException(message);
        }

        /// <summary>
        /// Throws an <see cref="InconclusiveException"/>.
        /// This causes the test to be reported as Inconclusive.
        /// </summary>
        [DoesNotReturn]
        public static void Inconclusive()
        {
            Inconclusive(string.Empty);
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

            if (context is { MultipleAssertLevel: 0, CurrentResult: { PendingFailures: > 0 } })
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

            if (context is { MultipleAssertLevel: 0, CurrentResult: { PendingFailures: > 0 } })
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

            if (context is { MultipleAssertLevel: 0, CurrentResult: { PendingFailures: > 0 } })
            {
                context.CurrentResult.RecordTestCompletion();
                throw new MultipleAssertException(context.CurrentResult);
            }
        }

        #endregion

        #region Helper Methods
        private static void ReportFailure(ConstraintResult result, string? message)
        {
            MessageWriter writer = new TextMessageWriter(message);
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
        private static readonly StackFilter SystemEnvironmentFilter = new(@" System\.Environment\.");

        private static string? GetStackTrace() =>
            StackFilter.DefaultFilter.Filter(SystemEnvironmentFilter.Filter(GetEnvironmentStackTraceWithoutThrowing()));

        /// <summary>
        /// If <see cref="Exception.StackTrace"/> throws, returns "SomeException was thrown by the
        /// Environment.StackTrace property." See also <see cref="ExceptionExtensions.GetStackTraceWithoutThrowing"/>.
        /// </summary>
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
            => TestExecutionContext.CurrentContext.IncrementAssertCount();

        #endregion
    }
}
