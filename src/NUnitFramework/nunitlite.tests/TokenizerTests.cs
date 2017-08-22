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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.Common.Tests
{
    public class TokenizerTests
    {
        [Test]
        public void NullInputThrowsException()
        {
            Assert.That(() => new Tokenizer(null), Throws.ArgumentNullException);
        }

        [Test]
        public void BlankStringReturnsEof()
        {
            var tokenizer = new Tokenizer("    ");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)), "First Call");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)), "Second Call");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)), "Third Call");
        }

        [Test]
        public void IdentifierTokens()
        {
            var tokenizer = new Tokenizer("  Identifiers x abc123 a1x  ");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "Identifiers")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "x")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "abc123")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "a1x")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)));
        }

        [Test]
        public void WordsInUnicode()
        {
            var tokenizer = new Tokenizer("method == Здравствуйте");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "method")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "==")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "Здравствуйте")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)));
        }

        [Test]
        public void StringWithDoubleQuotes()
        {
            var tokenizer = new Tokenizer("\"string at start\" \"may contain ' char\" \"string at end\"");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "string at start")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "may contain ' char")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "string at end")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)));
        }

        [Test]
        public void StringWithSingleQuotes()
        {
            var tokenizer = new Tokenizer("'string at start' 'may contain \" char' 'string at end'");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "string at start")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "may contain \" char")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "string at end")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)));
        }

        [Test]
        public void StringWithSlashes()
        {
            var tokenizer = new Tokenizer("/string at start/ /may contain \" char/ /string at end/");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "string at start")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "may contain \" char")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "string at end")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)));
        }

        [Test]
        public void StringsMayContainEscapedQuoteChar()
        {
            var tokenizer = new Tokenizer("/abc\\/xyz/   'abc\\'xyz'  \"abc\\\"xyz\"");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "abc/xyz")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "abc'xyz")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "abc\"xyz")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)));
        }

        [Test]
        public void SymbolTokens_SingleChar()
        {
            var tokenizer = new Tokenizer("=!&|()");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "=")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "!")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "&")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "|")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "(")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, ")")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)));
        }

        [Test]
        public void SymbolTokens_DoubleChar()
        {
            var tokenizer = new Tokenizer("==&&||!==~!~");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "==")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "&&")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "||")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "!=")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "=~")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "!~")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)));
        }

        [Test]
        public void MixedTokens_Simple()
        {
            var tokenizer = new Tokenizer("id=123");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "id")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "=")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "123")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)));
        }

        [Test]
        public void MixedTokens_Complex()
        {
            var tokenizer = new Tokenizer("name =~ '*DataBase*' && (category = Urgent || Priority = High)");
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "name")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "=~")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.String, "*DataBase*")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "&&")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "(")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "category")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "=")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "Urgent")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "||")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "Priority")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, "=")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Word, "High")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Symbol, ")")));
            Assert.That(tokenizer.NextToken(), Is.EqualTo(new Token(TokenKind.Eof)));
        }
    }
}
