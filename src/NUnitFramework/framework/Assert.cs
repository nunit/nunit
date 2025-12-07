// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
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
        public static void Pass(string message)
        {
            // If we are in a multiple assert block, this is an error
            if (TestExecutionContext.CurrentContext.IsInsideMultipleAssert)
                throw new Exception("Assert.Pass may not be used in a multiple assertion block.");
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
        public static void Fail(string message)
        {
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
        public static void Warn(string message)
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
        [DoesNotReturn]
        public static void Ignore(string message)
        {
            // If we are in a multiple assert block, this is an error
            if (TestExecutionContext.CurrentContext.IsInsideMultipleAssert)
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
        public static void Inconclusive(string message)
        {
            // If we are in a multiple assert block, this is an error
            if (TestExecutionContext.CurrentContext.IsInsideMultipleAssert)
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
            using (EnterMultipleScope())
            {
                testDelegate();
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
            using (EnterMultipleScope())
            {
                AsyncToSyncAdapter.Await(TestExecutionContext.CurrentContext, testDelegate.Invoke);
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
            using (EnterMultipleScope())
            {
                await testDelegate();
            }
        }

        /// <summary>
        /// Enters a multiple assert scope.
        /// Wraps code containing a series of assertions, which should all
        /// be executed, even if they fail. Failed results are saved and
        /// reported when the returned IDisposable is disposed.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> which when disposed leaves the multiple assertion scope.</returns>
        public static IDisposable EnterMultipleScope()
        {
            return new AssertionScope();
        }
        private sealed class AssertionScope : IAssertionScope
        {
            private readonly TestExecutionContext _context;
            private readonly int _assertionCountWhenEnteringScope;

            private int _isDisposed;

            public AssertionScope()
            {
                _context = TestExecutionContext.CurrentContext;
                Guard.OperationValid(_context is not null, "There is no current test execution context.");

                lock (_context)
                {
                    _assertionCountWhenEnteringScope = _context.CurrentResult.AssertionResultCount;
                    ++_context.MultipleAssertLevel;
                }
            }

            /// <summary>
            /// Gets a count of pending failures (from Multiple Assert)
            /// </summary>
            public bool HasFailuresInScope => _context.CurrentResult.AssertionResultCount > _assertionCountWhenEnteringScope;

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _isDisposed, 1) == 1)
                    return; // Already disposed.

                Guard.OperationValid(TestExecutionContext.CurrentContext == _context, "The assertion scope does not belong to this test.");

                int multipleAssertLevel;
                int assertionCount;
                int pendingFailures;

                lock (_context)
                {
                    multipleAssertLevel = --_context.MultipleAssertLevel;
                    assertionCount = _context.CurrentResult.AssertionResultCount;
                    pendingFailures = _context.CurrentResult.PendingFailures;
                }

                if (multipleAssertLevel == 0 && pendingFailures > 0)
                {
                    _context.CurrentResult.RecordTestCompletion();
                    if (assertionCount > _assertionCountWhenEnteringScope)
                    {
                        // We are at the end of the outermost multiple assert scope and there were failures recorded.
                        // Throw MultipleAssertException to exit current test
                        // unless we are in the middle of handling another exception we don't want to loose.
                        if (!IsExceptionActive())
                            throw new MultipleAssertException(_context.CurrentResult);
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        private static bool _exceptionActiveCheckingPossible = true;

        private static bool IsExceptionActive()
        {
            if (!_exceptionActiveCheckingPossible)
                return false;

            try
            {
#pragma warning disable CS0618 // Type or member is obsolete
                return System.Runtime.InteropServices.Marshal.GetExceptionCode() != 0;
#pragma warning restore CS0618 // Type or member is obsolete
            }
            catch (PlatformNotSupportedException)
            {
                _exceptionActiveCheckingPossible = false;
                return false;
            }
        }

        internal static string ExtendedMessage(string methodName, string message, string actualExpression, string constraintExpression)
        {
            string context = $"{methodName}({actualExpression}, {constraintExpression})";
            string extendedMessage = string.IsNullOrEmpty(message) ? context : $"{message}\n{context}";

            return extendedMessage;
        }

        private static void ReportFailure(string message)
        {
            // Record the failure in an <assertion> element
            var result = TestExecutionContext.CurrentContext.CurrentResult;
            result.RecordAssertion(AssertionStatus.Failed, message, GetStackTrace());
            result.RecordTestCompletion();

            // If multiple asserts disabled, then throw
            if (TestExecutionContext.CurrentContext.IsInsideMultipleAssert is false)
            {
                throw new AssertionException(result.Message);
            }
            else if (TestExecutionContext.CurrentContext.ThrowOnEachFailureUnderDebugger && Debugger.IsAttached)
            {
                try
                {
                    throw new AssertionException(result.Message);
                }
                catch (AssertionException)
                {
                    // we catch exception for multiple assert block to not change observed behavior but still allow user to break into debugger
                }
            }
        }

        private static void IssueWarning(string message)
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
