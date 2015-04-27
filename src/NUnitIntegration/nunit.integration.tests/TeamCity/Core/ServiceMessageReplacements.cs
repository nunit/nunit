namespace JetBrains.TeamCityCert.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

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