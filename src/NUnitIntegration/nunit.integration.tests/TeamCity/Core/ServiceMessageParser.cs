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
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Integration.Tests.TeamCity.Core.Common;

namespace NUnit.Integration.Tests.TeamCity.Core
{
    /// <summary>
    /// Provides service messages parsing from stream
    /// </summary>
    internal sealed class ServiceMessageParser : IServiceMessageParser
    {
        private static readonly char[] ServiceMessageOpenChars = ServiceMessageConstants.ServiceMessageOpen.ToCharArray();

        /// <summary>
        /// Lazy parses service messages from string
        /// </summary>
        /// <param name="text">text to parse</param>
        /// <returns>enumerable of service messages</returns>
        public IEnumerable<IServiceMessage> ParseServiceMessages(string text)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(text));

            return ParseServiceMessages(new StringReader(text));
        }

        /// <summary>
        /// Reads stream parsing service messages from it.
        /// </summary>
        /// <param name="reader">stream to parse. Stream will not be closed</param>
        /// <returns>Iterator of service messages</returns>
        public IEnumerable<IServiceMessage> ParseServiceMessages(TextReader reader)
        {
            while (true)
            {
                int currentSymbol = 0;
                int symbol;
                while ((symbol = reader.Read()) >= 0)
                {
                    var c = (char)symbol;
                    if (c != ServiceMessageOpenChars[currentSymbol])
                    {
                        // This was not a service message, let's try again in the next char
                        currentSymbol = 0;
                    }
                    else
                    {
                        currentSymbol++;
                        if (currentSymbol >= ServiceMessageOpenChars.Length)
                        {
                            break;
                        }
                    }
                }

                if (symbol < 0)
                {
                    yield break;
                }

                // There was ##teamcity[ parsed
                if (currentSymbol != ServiceMessageOpenChars.Length)
                {
                    yield break;
                }

                var messageName = new StringBuilder();
                while ((symbol = reader.Read()) >= 0 && !char.IsWhiteSpace((char)symbol))
                {
                    messageName.Append((char)symbol);
                }

                if (symbol < 0)
                {
                    yield break;
                }

                while ((symbol = reader.Read()) >= 0 && char.IsWhiteSpace((char)symbol))
                {
                }

                if (symbol < 0)
                {
                    yield break;
                }

                if (symbol == '\'')
                {
                    var buffer = new List<char>();
                    while ((symbol = reader.Read()) >= 0)
                    {
                        var ch = (char)symbol;
                        if (ch == '|')
                        {
                            buffer.Add(ch);
                            symbol = reader.Read();
                            if (symbol < 0)
                            {
                                yield break;
                            }

                            buffer.Add((char)symbol);
                        }
                        else
                        {
                            if (ch == '\'')
                            {
                                break;
                            }

                            buffer.Add(ch);
                        }
                    }
                    if (symbol < 0)
                    {
                        yield break;
                    }

                    while ((symbol = reader.Read()) >= 0 && char.IsWhiteSpace((char)symbol))
                    {                        
                    }

                    if (symbol < 0)
                    {
                        yield break;
                    }

                    if (symbol == ']')
                    {
                        yield return new ServiceMessage(messageName.ToString(), new string(ServiceLocator.Root.GetService<IServiceMessageReplacements>().Decode(buffer).ToArray()));
                    }
                }
                else
                {
                    var paramz = new Dictionary<string, string>();

                    while (true)
                    {
                        var name = new StringBuilder();
                        name.Append((char)symbol);

                        while ((symbol = reader.Read()) >= 0 && symbol != '=')
                        {
                            name.Append((char)symbol);
                        }

                        if (symbol < 0)
                        {
                            yield break;
                        }

                        while ((symbol = reader.Read()) >= 0 && char.IsWhiteSpace((char)symbol))
                        {                            
                        }

                        if (symbol < 0)
                        {
                            yield break;
                        }

                        if (symbol != '\'')
                        {
                            break;
                        }

                        var buffer = new List<char>();
                        while ((symbol = reader.Read()) >= 0)
                        {
                            var ch = (char)symbol;
                            if (ch == '|')
                            {
                                buffer.Add(ch);
                                symbol = reader.Read();
                                if (symbol < 0)
                                {
                                    yield break;
                                }

                                buffer.Add((char)symbol);
                            }
                            else
                            {
                                if (ch == '\'')
                                {
                                    break;
                                }

                                buffer.Add(ch);
                            }
                        }
                        
                        if (symbol < 0)
                        {
                            yield break;
                        }

                        paramz[name.ToString().Trim()] = new string(ServiceLocator.Root.GetService<IServiceMessageReplacements>().Decode(buffer).ToArray());
                        while ((symbol = reader.Read()) >= 0 && char.IsWhiteSpace((char)symbol))
                        {                            
                        }

                        if (symbol < 0)
                        {
                            yield break;
                        }

                        if (symbol == ']')
                        {
                            yield return new ServiceMessage(messageName.ToString(), null, paramz);
                            break;
                        }
                    }
                }
            }
        }
    }
}