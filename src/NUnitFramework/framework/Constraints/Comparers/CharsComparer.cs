// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Comparator for two <see cref="char"/>s.
    /// </summary>
    internal sealed class CharsComparer : ChainComparer<char>
    {
        private readonly bool _caseInsensitive;

        internal CharsComparer(bool caseInsensitive)
        {
            _caseInsensitive = caseInsensitive;
        }

        public override bool Equals(char x, char y, ref Tolerance tolerance)
        {
            char c1 = _caseInsensitive ? char.ToLower(x) : x;
            char c2 = _caseInsensitive ? char.ToLower(y) : y;
            return c1 == c2;
        }

        public override int GetHashCode(char ch)
        {
            return _caseInsensitive ? char.ToLower(ch) : ch;
        }
    }
}
