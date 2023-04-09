// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Allows enumerating sub-strings from a string based on separator char.
    /// Basically a less allocating version of string.Split.
    /// </summary>
    internal struct StringTokenEnumerator : IEnumerator<string>, IEnumerable
    {
        private int _currentStartIndex;
        private readonly string _target;
        private readonly char _separator;
        private readonly bool _returnEmptyTokens;

        /// <summary>
        /// Constructs a new enumerator against given target with given separator.
        /// </summary>
        /// <param name="target">Target string to enumerate.</param>
        /// <param name="separator">Separator between tokens to return.</param>
        /// <param name="returnEmptyTokens">Whether empty tokens should be returned</param>
        public StringTokenEnumerator(string target, char separator, bool returnEmptyTokens)
        {
            _currentStartIndex = 0;
            _target = target;
            _separator = separator;
            _returnEmptyTokens = returnEmptyTokens;
            Current = null;
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (!_returnEmptyTokens && _target.Length == 0)
            {
                return false;
            }

            while (FindNextToken())
            {
                if (!_returnEmptyTokens && Current!.Length == 0)
                {
                    // check for the next one
                    continue;
                }

                break;
            }

            return Current != null;
        }

        private bool FindNextToken()
        {
            int start = _currentStartIndex;

            if (start > _target.Length)
            {
                // we've jumped to the end
                Current = null;
                return false;
            }

            var endIndex = _target.IndexOf(_separator, start);
            if (endIndex == -1)
            {
                Current = _target.Substring(start);
                // signal that we have no more to give, next loop will return false
                _currentStartIndex = _target.Length + 1;
                return true;
            }

            Current = _target.Substring(start, endIndex - start);
            _currentStartIndex = endIndex + 1;
            return true;
        }

        /// <inheritdoc />
        public void Reset()
        {
            throw new NotSupportedException("Resetting enumerator is not supported");
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public string Current { get; private set; }

        object IEnumerator.Current => Current;

        /// <summary>
        /// Return unboxed enumerator when type is well known, do not remove.
        /// </summary>
        public StringTokenEnumerator GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => this;
    }

    internal static class StringExtensions
    {
        /// <summary>
        /// Tokenizes given string with given separator.
        /// </summary>
        /// <param name="s">Input string.</param>
        /// <param name="separator">Separator to use.</param>
        /// <param name="returnEmptyTokens">Whether to return empty token (length 0), defaults to false.</param>
        /// <returns></returns>
        [MethodImpl(256)] // aggressive inlining, not available for old frameworks via enum
        internal static StringTokenEnumerator Tokenize(this string s, char separator, bool returnEmptyTokens = false)
        {
            return new StringTokenEnumerator(s, separator, returnEmptyTokens);
        }
    }
}
