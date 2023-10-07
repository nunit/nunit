// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework
{
    /// <summary>
    /// A class to allow postponing the actual formatting of interpolated strings.
    /// </summary>
    /// <remarks>
    /// This class is needed as the compiler prefers to call a <see cref="string"/> overload
    /// vs a <see cref="FormattableString"/> overload.
    /// https://www.damirscorner.com/blog/posts/20180921-FormattableStringAsMethodParameter.html
    /// </remarks>
    public readonly struct NUnitString
    {
        // Keep as nullable, as default(NUnitString) will set this to null.
        private readonly string? _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitString"/> class.
        /// </summary>
        /// <param name="message">The message formattable to hold.</param>
        public NUnitString(string message)
        {
            _message = message;
        }

        /// <summary>
        /// Implicit conversion from a <see cref="FormattableString"/> to a <see cref="NUnitString"/>.
        /// </summary>
        /// <remarks>
        /// Should never be called. It only exists for the compiler.
        /// </remarks>
        /// <param name="formattableMessage">The message formattable to hold.</param>
        [Obsolete("This only exists for the compiler")]
        public static implicit operator NUnitString(FormattableString formattableMessage)
            => throw new NotImplementedException();

        /// <summary>
        /// Implicit conversion from a <see cref="string"/> to a <see cref="NUnitString"/>.
        /// </summary>
        /// <param name="message">The message formattable to hold.</param>
        public static implicit operator NUnitString(string message) => new(message);

        /// <inheritdoc/>
        public override string ToString() => _message ?? string.Empty;
    }
}
