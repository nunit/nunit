// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The AssertionResult class represents the result of a single assertion.
    /// </summary>
    public class AssertionResult : IEquatable<AssertionResult>
    {
        /// <summary>
        /// Construct an AssertionResult
        /// </summary>
        public AssertionResult(AssertionStatus status, string message, string? stackTrace)
            : this(FailureSite.Test, status, message, stackTrace)
        {
        }

        /// <summary>
        /// Construct an AssertionResult
        /// </summary>
        public AssertionResult(FailureSite site, AssertionStatus status, string message, string? stackTrace)
        {
            Site = site;
            Status = status;
            Message = message;
            StackTrace = stackTrace;
        }

        /// <summary> Where did the failure occur.</summary>
        public FailureSite Site { get; }

        /// <summary> The pass/fail status of the assertion</summary>
        public AssertionStatus Status { get; }

        /// <summary>The message produced by the assertion, or null</summary>
        public string Message { get; }

        /// <summary>The stack trace associated with the assertion, or null</summary>
        public string? StackTrace { get; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(object? obj)
        {
            return Equals(obj as AssertionResult);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(AssertionResult? other)
        {
            return other is not null &&
                Status == other.Status &&
                Message == other.Message &&
                StackTrace == other.StackTrace;
        }

        /// <summary>Serves as the default hash function.</summary>
        public override int GetHashCode()
        {
            var hashCode = -783279553;
            hashCode = hashCode * -1521134295 + Status.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            hashCode = hashCode * -1521134295 + EqualityComparer<string?>.Default.GetHashCode(StackTrace ?? string.Empty);
            return hashCode;
        }

        /// <summary>
        /// ToString Override
        /// </summary>
        public override string ToString()
        {
            return $"Assert {Status}: {Message}" + Environment.NewLine + StackTrace;
        }
    }
}
