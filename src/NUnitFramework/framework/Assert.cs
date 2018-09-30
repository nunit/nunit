// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

#if ASYNC
    /// <summary>
    /// Delegate used by tests that execute async code and
    /// capture any thrown exception.
    /// </summary>
    public delegate System.Threading.Tasks.Task AsyncTestDelegate();
#endif

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
            throw new InvalidOperationException("Assert.Equals should not be used for Assertions, use Assert.AreEqual(...) instead.");
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
            throw new InvalidOperationException("Assert.ReferenceEquals should not be used for Assertions, use Assert.AreSame(...) instead.");
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
        static public void Pass(string message, params object[] args)
        {
            if (message == null) message = string.Empty;
            else if (args != null && args.Length > 0)
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
        static public void Pass(string message)
        {
            Assert.Pass(message, null);
        }

        /// <summary>
        /// Throws a <see cref="SuccessException"/> with the message and arguments
        /// that are passed in. This allows a test to be cut short, with a result
        /// of success returned to NUnit.
        /// </summary>
        static public void Pass()
        {
            Assert.Pass(string.Empty, null);
        }

        #endregion

        #region Fail

        /// <summary>
        /// Throws an <see cref="AssertionException"/> with the message and arguments
        /// that are passed in. This is used by the other Assert functions.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Fail(string message, params object[] args)
        {
            if (message == null) message = string.Empty;
            else if (args != null && args.Length > 0)
                message = string.Format(message, args);

            ReportFailure(message);
        }

        /// <summary>
        /// Throws an <see cref="AssertionException"/> with the message that is
        /// passed in. This is used by the other Assert functions.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        static public void Fail(string message)
        {
            Assert.Fail(message, null);
        }

        /// <summary>
        /// Throws an <see cref="AssertionException"/>.
        /// This is used by the other Assert functions.
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
        static public void Warn(string message, params object[] args)
        {
            if (message == null) message = string.Empty;
            else if (args != null && args.Length > 0)
                message = string.Format(message, args);

            IssueWarning(message);
        }

        /// <summary>
        /// Issues a warning using the message provided.
        /// </summary>
        /// <param name="message">The message to display.</param>
        static public void Warn(string message)
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
        static public void Ignore(string message, params object[] args)
        {
            if (message == null) message = string.Empty;
            else if (args != null && args.Length > 0)
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
        static public void Ignore(string message)
        {
            Assert.Ignore(message, null);
        }

        /// <summary>
        /// Throws an <see cref="IgnoreException"/>.
        /// This causes the test to be reported as ignored.
        /// </summary>
        static public void Ignore()
        {
            Assert.Ignore(string.Empty, null);
        }

        #endregion

        #region InConclusive

        /// <summary>
        /// Throws an <see cref="InconclusiveException"/> with the message and arguments
        /// that are passed in.  This causes the test to be reported as inconclusive.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="InconclusiveException"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        static public void Inconclusive(string message, params object[] args)
        {
            if (message == null) message = string.Empty;
            else if (args != null && args.Length > 0)
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
        static public void Inconclusive(string message)
        {
            Assert.Inconclusive(message, null);
        }

        /// <summary>
        /// Throws an <see cref="InconclusiveException"/>.
        /// This causes the test to be reported as Inconclusive.
        /// </summary>
        static public void Inconclusive()
        {
            Assert.Inconclusive(string.Empty, null);
        }

        #endregion

        #region Contains

        /// <summary>
        /// Asserts that an object is contained in a collection.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The collection to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Contains(object expected, ICollection actual, string message, params object[] args)
        {
            Assert.That(actual, new SomeItemsConstraint(new EqualConstraint(expected)) ,message, args);
        }

        /// <summary>
        /// Asserts that an object is contained in a collection.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The collection to be examined</param>
        public static void Contains(object expected, ICollection actual)
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
            Guard.OperationValid(context != null, "Assert.Multiple called outside of a valid TestExecutionContext");

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
                throw new MultipleAssertException(context.CurrentResult);
            }
        }

#if ASYNC
        /// <summary>
        /// Wraps code containing a series of assertions, which should all
        /// be executed, even if they fail. Failed results are saved and
        /// reported at the end of the code block.
        /// </summary>
        /// <param name="testDelegate">A TestDelegate to be executed in Multiple Assertion mode.</param>
        public static void Multiple(AsyncTestDelegate testDelegate)
        {
            TestExecutionContext context = TestExecutionContext.CurrentContext;
            Guard.OperationValid(context != null, "Assert.Multiple called outside of a valid TestExecutionContext");

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
                throw new MultipleAssertException(context.CurrentResult);
            }
        }
#endif

#endregion

#region Helper Methods

        private static void ReportFailure(ConstraintResult result, string message)
        {
            ReportFailure(result, message, null);
        }

        private static void ReportFailure(ConstraintResult result, string message, params object[] args)
        {
            MessageWriter writer = new TextMessageWriter(message, args);
            result.WriteMessageTo(writer);

            ReportFailure(writer.ToString());
        }

        private static void ReportFailure(string message)
        {
            // Record the failure in an <assertion> element
            var result = TestExecutionContext.CurrentContext.CurrentResult;
            result.RecordAssertion(AssertionStatus.Failed, message, GetStackTrace());
            result.RecordTestCompletion();

            // If we are outside any multiple assert block, then throw
            if (TestExecutionContext.CurrentContext.MultipleAssertLevel == 0)
                throw new AssertionException(result.Message);
        }

        private static void IssueWarning(string message)
        {
            var result = TestExecutionContext.CurrentContext.CurrentResult;
            result.RecordAssertion(AssertionStatus.Warning, message, GetStackTrace());
        }

        // System.Environment.StackTrace puts extra entries on top of the stack, at least in some environments
        private static readonly StackFilter SystemEnvironmentFilter = new StackFilter(@" System\.Environment\.");

        private static string GetStackTrace() =>
            StackFilter.DefaultFilter.Filter(SystemEnvironmentFilter.Filter(Environment.StackTrace));

        private static void IncrementAssertCount()
        {
            TestExecutionContext.CurrentContext.IncrementAssertCount();
        }

#endregion
    }
}
