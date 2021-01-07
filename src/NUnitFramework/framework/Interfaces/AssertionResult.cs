// ***********************************************************************
// Copyright (c) 2016 Charlie Poole, Rob Prouse
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Text;

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
        public AssertionResult(AssertionStatus status, string? message, string? stackTrace)
        {
            Status = status;
            Message = message;
            StackTrace = stackTrace;
        }

        /// <summary> The pass/fail status of the assertion</summary>
        public AssertionStatus Status { get; }

        /// <summary>The message produced by the assertion, or null</summary>
        public string? Message { get; }

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
            return other != null &&
                Status == other.Status &&
                Message == other.Message &&
                StackTrace == other.StackTrace;
        }

        /// <summary>Serves as the default hash function.</summary>
        public override int GetHashCode()
        {
            var hashCode = -783279553;
            hashCode = hashCode * -1521134295 + Status.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string?>.Default.GetHashCode(Message);
            hashCode = hashCode * -1521134295 + EqualityComparer<string?>.Default.GetHashCode(StackTrace);
            return hashCode;
        }

        /// <summary>
        /// ToString Override
        /// </summary>
        public override string ToString()
        {
            return string.Format("Assert {0}: {1}", Status, Message) + Environment.NewLine + StackTrace;
        }
    }
}
