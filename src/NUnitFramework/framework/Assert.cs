// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.ComponentModel;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
    using NUnit.Framework.Internal;

    /// <summary>
    /// Delegate used by tests that execute code and
    /// capture any thrown exception.
    /// </summary>
    public delegate void TestDelegate();

#if NET_4_0 || NET_4_5 || PORTABLE
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
    public partial class Assert
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Assert"/> class. 
        /// We don't actually want any instances of this object, but some people
        /// like to inherit from it to add other static methods. Hence, the
        /// protected constructor disallows any instances of this object. 
        /// </summary>
        protected Assert()
        {
        }

        #endregion

        #region Equals and ReferenceEquals

        /// <summary>
        /// The Equals method throws an InvalidOperationException. This is done 
        /// to make sure there is no mistake by calling this function.
        /// </summary>
        /// <param name="a">The left object</param>
        /// <param name="b">The right object</param>
        /// <returns>Not applicable</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new bool Equals(object a, object b)
        {
            throw new InvalidOperationException("Assert.Equals should not be used for Assertions");
        }

        /// <summary>
        /// override the default ReferenceEquals to throw an InvalidOperationException. This 
        /// implementation makes sure there is no mistake in calling this function 
        /// as part of Assert. 
        /// </summary>
        /// <param name="a">The left object</param>
        /// <param name="b">The right object</param>
        public static new void ReferenceEquals(object a, object b)
        {
            throw new InvalidOperationException("Assert.ReferenceEquals should not be used for Assertions");
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
        public static void Pass(string message, params object[] args)
        {
            Assert.Pass(BuildExceptionMessageFunc(message, args));
        }

        /// <summary>
        /// Throws a <see cref="SuccessException"/> with the message and arguments 
        /// that are passed in. This allows a test to be cut short, with a result
        /// of success returned to NUnit.
        /// </summary>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Pass(Func<string> getExceptionMessage)
        {
            throw new SuccessException(getExceptionMessage());
        }

        /// <summary>
        /// Throws a <see cref="SuccessException"/> with the message and arguments 
        /// that are passed in. This allows a test to be cut short, with a result
        /// of success returned to NUnit.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        public static void Pass(string message)
        {
            Assert.Pass(message, null);
        }

        /// <summary>
        /// Throws a <see cref="SuccessException"/> with the message and arguments 
        /// that are passed in. This allows a test to be cut short, with a result
        /// of success returned to NUnit.
        /// </summary>
        public static void Pass()
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
        public static void Fail(string message, params object[] args)
        {
            Assert.Fail(BuildExceptionMessageFunc(message, args));
        }

        /// <summary>
        /// Throws an <see cref="AssertionException"/> with the message and arguments 
        /// that are passed in. This is used by the other Assert functions. 
        /// </summary>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Fail(Func<string> getExceptionMessage)
        {
            throw new AssertionException(getExceptionMessage());
        }

        /// <summary>
        /// Throws an <see cref="AssertionException"/> with the message that is 
        /// passed in. This is used by the other Assert functions. 
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        public static void Fail(string message)
        {
            Assert.Fail(message, null);
        }

        /// <summary>
        /// Throws an <see cref="AssertionException"/>. 
        /// This is used by the other Assert functions. 
        /// </summary>
        public static void Fail()
        {
            Assert.Fail(string.Empty, null);
        }

        #endregion

        #region Ignore

        /// <summary>
        /// Throws an <see cref="IgnoreException"/> with the message and arguments 
        /// that are passed in.  This causes the test to be reported as ignored.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void Ignore(string message, params object[] args)
        {
            Assert.Ignore(BuildExceptionMessageFunc(message, args));
        }

        /// <summary>
        /// Throws an <see cref="IgnoreException"/> with the message and arguments 
        /// that are passed in.  This causes the test to be reported as ignored.
        /// </summary>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Ignore(Func<string> getExceptionMessage)
        {
            throw new IgnoreException(getExceptionMessage());
        }

        /// <summary>
        /// Throws an <see cref="IgnoreException"/> with the message that is 
        /// passed in. This causes the test to be reported as ignored. 
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="AssertionException"/> with.</param>
        public static void Ignore(string message)
        {
            Assert.Ignore(message, null);
        }

        /// <summary>
        /// Throws an <see cref="IgnoreException"/>. 
        /// This causes the test to be reported as ignored. 
        /// </summary>
        public static void Ignore()
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
        public static void Inconclusive(string message, params object[] args)
        {
            Assert.Inconclusive(BuildExceptionMessageFunc(message, args));
        }

        /// <summary>
        /// Throws an <see cref="InconclusiveException"/> with the message and arguments 
        /// that are passed in.  This causes the test to be reported as inconclusive.
        /// </summary>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Inconclusive(Func<string> getExceptionMessage)
        {
            throw new InconclusiveException(getExceptionMessage());
        }

        /// <summary>
        /// Throws an <see cref="InconclusiveException"/> with the message that is 
        /// passed in. This causes the test to be reported as inconclusive. 
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="InconclusiveException"/> with.</param>
        public static void Inconclusive(string message)
        {
            Assert.Inconclusive(message, null);
        }

        /// <summary>
        /// Throws an <see cref="InconclusiveException"/>. 
        /// This causes the test to be reported as Inconclusive. 
        /// </summary>
        public static void Inconclusive()
        {
            Assert.Inconclusive(string.Empty, null);
        }

        #endregion

        #region Contains

        /// <summary>
        /// Asserts that an object is contained in a list.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The list to be examined</param>
        /// <param name="message">The message to display in case of failure</param>
        /// <param name="args">Array of objects to be used in formatting the message</param>
        public static void Contains(object expected, ICollection actual, string message, params object[] args)
        {
            Assert.That(actual, new CollectionContainsConstraint(expected), message, args);
        }

        /// <summary>
        /// Asserts that an object is contained in a list.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The list to be examined</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Contains(object expected, ICollection actual, Func<ConstraintResult, string> getExceptionMessage)
        {
            Assert.That(actual, new CollectionContainsConstraint(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that an object is contained in a list.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The list to be examined</param>
        /// <param name="getExceptionMessage">A function to build the message included with the Exception</param>
        public static void Contains(object expected, ICollection actual, Func<string> getExceptionMessage)
        {
            Assert.That(actual, new CollectionContainsConstraint(expected), getExceptionMessage);
        }

        /// <summary>
        /// Asserts that an object is contained in a list.
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The list to be examined</param>
        public static void Contains(object expected, ICollection actual)
        {
            Assert.That(actual, new CollectionContainsConstraint(expected), null, null);
        }

        #endregion

        #region Multiple

        ///// <summary>
        ///// If an assert fails within this block, execution will continue and 
        ///// the errors will be reported at the end of the block.
        ///// </summary>
        ///// <param name="del">The test delegate</param>
        ////public static void Multiple(TestDelegate del)
        ////{
        ////    del();
        ////}

        #endregion

        #region Helper Functions

        /// <summary>
        /// Helper function that creates a lambda function passed to an Assert method that lazily builds the Exception message.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="Exception"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        /// <returns>A lambda function to lazily build the string</returns>
        public static Func<string> BuildExceptionMessageFunc(string message, object[] args)
        {
            return () =>
            {
                if (message == null)
                {
                    message = string.Empty;
                }
                else if (args != null && args.Length > 0)
                {
                    message = string.Format(message, args);
                }

                return message;
            };
        }

        /// <summary>
        /// Helper function that creates a lambda function passed to an Assert method that lazily builds the Exception message.  The function returned from this method ignores the <see cref="ConstraintResult"/> object returned from the <see cref="IConstraint.ApplyTo{TActual}(TActual)"/> method.  It is intended to be used when the client wants to pass a message and parameter arguments to the Assert method overload.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="Exception"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        /// <returns>A lambda function to lazily build the string</returns>
        public static Func<ConstraintResult, string> BuildExceptionMessageFuncIgnoringConstraintResult(
            string message,
            object[] args)
        {
            return result => BuildExceptionMessageFunc(message, args).Invoke();
        }

        /// <summary>
        /// Helper function that creates a lambda function passed to an Assert method that lazily builds the Exception message.  The function returned from this method ignores the <see cref="ConstraintResult"/> object returned from the <see cref="IConstraint.ApplyTo{TActual}(TActual)"/> method.  It is intended to be used when the client wants to pass a <see cref="Func{TResult}"/> where TResult is a string to the Assert method overload.
        /// </summary>
        /// <param name="getExceptionMessageFunc">A function to build the message included with the Exception</param>
        /// <returns>A lambda function to lazily build the string</returns>
        public static Func<ConstraintResult, string> BuildExceptionMessageFuncIgnoringConstraintResult(
            Func<string> getExceptionMessageFunc)
        {
            return result => getExceptionMessageFunc.Invoke();
        }

        /// <summary>
        /// Helper method to catch an exception thrown by a <see cref="TestDelegate"/>.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The <see cref="Exception"/> thrown (if any).</returns>
        public static Exception CatchException(TestDelegate code)
        {
            Exception caughtException = null;

#if NET_4_0 || NET_4_5 || PORTABLE
            if (AsyncInvocationRegion.IsAsyncOperation(code))
            {
                using (var region = AsyncInvocationRegion.Create(code))
                {
                    code();

                    try
                    {
                        region.WaitForPendingOperationsToComplete(null);
                    }
                    catch (Exception e)
                    {
                        caughtException = e;
                    }
                }
            }
            else
#endif
            {
                try
                {
                    code();
                }
                catch (Exception ex)
                {
                    caughtException = ex;
                }
            }

            return caughtException;
        }

        /// <summary>
        /// Helper function that creates a lambda function passed to an Assert method that lazily builds the Exception message.  The function returned from this method ignores the <see cref="ConstraintResult"/> object returned from the <see cref="IConstraint.ApplyTo{TActual}(TActual)"/> method.  It is intended to be used when the client wants to pass a message and parameter arguments to the Assert method overload.
        /// </summary>
        /// <param name="message">The message to initialize the <see cref="Exception"/> with.</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        /// <returns>A lambda function to lazily build the string</returns>
        public static Func<ConstraintResult, string> BuildDefaultExceptionMessageFunc(string message, object[] args)
        {
            return result =>
            {
                MessageWriter writer = new TextMessageWriter(message, args);
                result.WriteMessageTo(writer);
                var exceptionMessage = writer.ToString();
                return exceptionMessage;
            };
        }

        #endregion
    }
}