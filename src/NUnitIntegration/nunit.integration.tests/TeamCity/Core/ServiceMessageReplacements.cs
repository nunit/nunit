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
using System.Collections.Generic;
using System.Linq;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    internal sealed class ServiceMessageReplacements : IServiceMessageReplacements
    {
        private static readonly Dictionary<char, char> EscMap = new Dictionary<char, char>
        {
            { '|', '|' },
            { '\'', '\'' },
            { '\n', 'n' },
            { '\r', 'r' },
            { '[', '[' },
            { ']', ']' },
            { '\u0085', 'x' },
            { '\u2028', 'l' },
            { '\u2029', 'p' }                                                                                 
        };

        private static readonly Dictionary<char, char> RevesedEscMap = EscMap.ToDictionary(i => i.Value, i => i.Key);

        /// <summary>
        /// Performs TeamCity-format escaping of a string.
        /// </summary>
        public IEnumerable<char> Encode(IEnumerable<char> chars)
        {
            Contract.Requires<ArgumentNullException>(chars != null);
            Contract.Ensures(Contract.Result<IEnumerable<char>>() != null);

            foreach (var ch in chars)
            {
                char escCh;
                if (EscMap.TryGetValue(ch, out escCh))
                {
                    yield return '|';
                    yield return escCh;                    
                }
                else
                {
                    yield return ch;                    
                }
            }
        }

        public IEnumerable<char> Decode(IEnumerable<char> chars)
        {
            Contract.Requires<ArgumentNullException>(chars != null);
            Contract.Ensures(Contract.Result<IEnumerable<char>>() != null);

            var escape = false;
            foreach (var ch in chars)
            {
                if (!escape)
                {
                    if (ch == '|')
                    {
                        escape = true;
                    }
                    else
                    {
                        yield return ch;
                    }
                }
                else
                {
                    escape = false;
                    char smb;
                    if (RevesedEscMap.TryGetValue(ch, out smb))
                    {
                        yield return smb;
                    }
                    else
                    {
                        yield return '?';
                    }
                }
            }
        }
    }
}