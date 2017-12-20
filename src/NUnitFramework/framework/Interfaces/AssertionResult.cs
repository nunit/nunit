using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The AssertionResult class represents the result of a single assertion.
    /// </summary>
    public class AssertionResult
    {
        /// <summary>
        /// Construct an AssertionResult
        /// </summary>
        public AssertionResult(AssertionStatus status, string message, string stackTrace)
        {
            Status = status;
            Message = message;
            StackTrace = stackTrace;
        }

        /// <summary> The pass/fail status of the assertion</summary>
        public AssertionStatus Status { get; private set; }

        /// <summary>The message produced by the assertion, or null</summary>
        public string Message { get; private set; }

        /// <summary>The stack trace associated with the assertion, or null</summary>
        public string StackTrace { get; private set; }

        /// <summary>
        /// ToString Override
        /// </summary>
        public override string ToString()
        {
            return string.Format("Assert {0}: {1}", Status, Message) + Environment.NewLine + StackTrace;
        }

        /// <summary>
        /// Override GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Override Equals
        /// </summary>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as AssertionResult;

            return other != null && Status == other.Status && Message == other.Message && StackTrace == other.StackTrace;
        }
    }
}
