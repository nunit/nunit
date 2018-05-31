// ***********************************************************************
// Copyright (c) 2015 Charlie Poole, Rob Prouse
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
using System.Text;

// Missing XML Docs
#pragma warning disable 1591

namespace NUnit.Common
{
    public enum TokenKind
    {
        Eof,
        Word,
        String,
        Symbol
    }

    public class Token
    {
        public Token(TokenKind kind) : this(kind, string.Empty) { }

        public Token(TokenKind kind, char ch) : this(kind, ch.ToString()) { }

        public Token(TokenKind kind, string text)
        {
            Kind = kind;
            Text = text;
        }

        public TokenKind Kind { get; }

        public string Text { get; }

        public int Pos { get; set; }

        #region Equality Overrides

        public override bool Equals(object obj)
        {
            return obj is Token && this == (Token)obj;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public override string ToString()
        {
            return Text != null
                ? Kind.ToString() + ":" + Text
                : Kind.ToString();
        }

        public static bool operator ==(Token t1, Token t2)
        {
            bool t1Null = ReferenceEquals(t1, null);
            bool t2Null = ReferenceEquals(t2, null);

            if (t1Null && t2Null)
                return true;

            if (t1Null || t2Null)
                return false;

            return t1.Kind == t2.Kind && t1.Text == t2.Text;
        }

        public static bool operator !=(Token t1, Token t2)
        {
            return !(t1 == t2);
        }

        #endregion
    }

    /// <summary>
    /// Tokenizer class performs lexical analysis for the TestSelectionParser.
    /// It recognizes a very limited set of tokens: words, symbols and
    /// quoted strings. This is sufficient for the simple DSL we use to
    /// select which tests to run.
    /// </summary>
    public class Tokenizer
    {
        private readonly string _input;
        private int _index;

        private const char EOF_CHAR = '\0';
        private const string WORD_BREAK_CHARS = "=!()&|";
        private readonly string[] DOUBLE_CHAR_SYMBOLS = new string[] { "==", "=~", "!=", "!~", "&&", "||" };

        private Token _lookahead;

        public Tokenizer(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            _input = input;
            _index = 0;
        }

        public Token LookAhead
        {
            get
            {
                if (_lookahead == null)
                    _lookahead = GetNextToken();

                return _lookahead;
            }
        }

        public Token NextToken()
        {
            Token result = _lookahead ?? GetNextToken();
            _lookahead = null;
            return result;
        }

        private Token GetNextToken()
        {
            SkipBlanks();

            var ch = NextChar;
            int pos = _index;

            switch (ch)
            {
                case EOF_CHAR:
                    return new Token(TokenKind.Eof) { Pos = pos };

                // Single char symbols
                case '(':
                case ')':
                    GetChar();
                    return new Token(TokenKind.Symbol, ch) { Pos = pos };

                // Possible double char symbols
                case '&':
                case '|':
                case '=':
                case '!':
                    GetChar();
                    foreach(string dbl in DOUBLE_CHAR_SYMBOLS)
                        if (ch == dbl[0] && NextChar == dbl[1])
                        {
                            GetChar();
                            return new Token(TokenKind.Symbol, dbl) { Pos = pos };
                        }

                    return new Token(TokenKind.Symbol, ch);

                case '"':
                case '\'':
                case '/':
                    return GetString();

                default:
                    return GetWord();
            }
        }

        private bool IsWordChar(char c)
        {
            if (char.IsWhiteSpace(c) || c == EOF_CHAR)
                return false;

            return WORD_BREAK_CHARS.IndexOf(c) < 0;
        }

        private Token GetWord()
        {
            var sb = new StringBuilder();
            int pos = _index;

            while (IsWordChar(NextChar))
                sb.Append(GetChar());

            return new Token(TokenKind.Word, sb.ToString()) { Pos = pos };
        }

        private Token GetString()
        {
            var sb = new StringBuilder();
            int pos = _index;

            char quote = GetChar(); // Save the initial quote char

            while (NextChar != EOF_CHAR)
            {
                var ch = GetChar();
                if (ch == '\\')
                    ch = GetChar();
                else if (ch == quote)
                    break;
                sb.Append(ch);
            }

            return new Token(TokenKind.String, sb.ToString()) { Pos = pos };
        }

        /// <summary>
        /// Get the next character in the input, consuming it.
        /// </summary>
        /// <returns>The next char</returns>
        private char GetChar()
        {
            return _index < _input.Length ? _input[_index++] : EOF_CHAR;
        }

        /// <summary>
        /// Peek ahead at the next character in input
        /// </summary>
        private char NextChar
        {
            get
            {
                return _index < _input.Length ? _input[_index] : EOF_CHAR;
            }
        }

        private void SkipBlanks()
        {
            while (char.IsWhiteSpace(NextChar))
                _index++;
        }
    }
}
