// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;

namespace NUnit.Framework
{
    /// <summary>
    /// Contains extension methods that do not require a special <c>using</c> directive.
    /// </summary>
    internal static class XmlExtensions
    {
        // we want to write just the main element without XML declarations
        internal static readonly XmlWriterSettings FragmentWriterSettings = new()
        {
            ConformanceLevel = ConformanceLevel.Fragment
        };

        /// <summary>
        /// Checks that attribute value contains safe content and if not, escapes it.
        /// </summary>
        internal static void WriteAttributeStringSafe(this XmlWriter writer, string name, string value)
        {
            writer.WriteAttributeString(name, EscapeInvalidXmlCharacters(value));
        }

        /// <summary>
        /// Checks that CDATA section contains safe content and if not, escapes it.
        /// </summary>
        internal static void WriteCDataSafe(this XmlWriter writer, string text)
        {
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            text = EscapeInvalidXmlCharacters(text);

            int start = 0;

            while (true)
            {
                int illegal = text.IndexOf("]]>", start, StringComparison.Ordinal);
                if (illegal < 0)
                    break;
                writer.WriteCData(text.Substring(start, illegal - start + 2));
                start = illegal + 2;
                if (start >= text.Length)
                    return;
            }

            if (start > 0)
                writer.WriteCData(text.Substring(start));
            else
                writer.WriteCData(text);
        }


        [return: NotNullIfNotNull("str")]
        internal static string? EscapeInvalidXmlCharacters(string? str)
        {
            if (str is null) return null;

            // quick check when we expect valid input
            foreach (var c in str)
            {
                if (c < 0x20 || c > 0x7F)
                {
                    return EscapeInvalidXmlCharactersUnlikely(str);
                }
            }

            return str;
        }

        private static string EscapeInvalidXmlCharactersUnlikely(string str)
        {
            StringBuilder? builder = null;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c > 0x20 && c < 0x7F)
                {
                    // ASCII characters - break quickly for these
                    builder?.Append(c);
                }
                // From the XML specification: https://www.w3.org/TR/xml/#charsets
                // Char ::= #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]
                // Any Unicode character, excluding the surrogate blocks, FFFE, and FFFF.
                else if (!(0x0 <= c && c <= 0x8) &&
                         c != 0xB &&
                         c != 0xC &&
                         !(0xE <= c && c <= 0x1F) &&
                         !(0x7F <= c && c <= 0x84) &&
                         !(0x86 <= c && c <= 0x9F) &&
                         !(0xD800 <= c && c <= 0xDFFF) &&
                         c != 0xFFFE &&
                         c != 0xFFFF)
                {
                    builder?.Append(c);
                }
                // Also check if the char is actually a high/low surrogate pair of two characters.
                // If it is, then it is a valid XML character (from above based on the surrogate blocks).
                else if (char.IsHighSurrogate(c) &&
                         i + 1 != str.Length &&
                         char.IsLowSurrogate(str[i + 1]))
                {
                    if (builder is not null)
                    {
                        builder.Append(c);
                        builder.Append(str[i + 1]);
                    }

                    i++;
                }
                else
                {
                    // We keep the builder null so that we don't allocate a string
                    // when doing this conversion until we encounter a unicode character.
                    // Then, we allocate the rest of the string and escape the invalid
                    // character.
                    if (builder is null)
                    {
                        builder = new StringBuilder();
                        for (int index = 0; index < i; index++)
                            builder.Append(str[index]);
                    }

                    builder.Append(CharToUnicodeSequence(c));
                }
            }

            if (builder is not null)
                return builder.ToString();
            else
                return str;
        }

        private static string CharToUnicodeSequence(char symbol)
        {
            return $"\\u{(int)symbol:x4}";
        }
    }
}
