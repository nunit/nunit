// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using NUnit.Common;

namespace NUnit.Engine
{
    public class TestSelectionParser
    {
        private Tokenizer _tokenizer;

        private readonly Token LPAREN = new Token(TokenKind.Symbol, "(");
        private readonly Token RPAREN = new Token(TokenKind.Symbol, ")");
        private readonly Token COMMA = new Token(TokenKind.Symbol, ",");
        private readonly Token AND_OP1 = new Token(TokenKind.Symbol, "&");
        private readonly Token AND_OP2 = new Token(TokenKind.Symbol, "&&");
        private readonly Token OR_OP1 = new Token(TokenKind.Symbol, "|");
        private readonly Token OR_OP2 = new Token(TokenKind.Symbol, "||");
        private readonly Token NOT_OP = new Token(TokenKind.Symbol, "!");

        public string Parse(string input)
        {
            if (input == null || input == "")
                return "";

            _tokenizer = new Tokenizer(input);

            return ParseFilterExpression();
        }

        /// <summary>
        /// Parse a single term or an or expression, returning the xml
        /// </summary>
        /// <returns></returns>
        public string ParseFilterExpression()
        {
            var terms = new List<string>();
            terms.Add(ParseFilterTerm());

            while (true)
            {
                Token op = Peek();
                if (op != OR_OP1 && op != OR_OP2)
                    break;

                NextToken();
                terms.Add(ParseFilterTerm());
            }

            if (terms.Count == 1)
                return terms[0];

            var sb = new StringBuilder("<or>");

            foreach (string term in terms)
                sb.Append(term);

            sb.Append("</or>");

            return sb.ToString();
        }

        /// <summary>
        /// Parse a single element or an and expression and return the xml
        /// </summary>
        public string ParseFilterTerm()
        {
            var elements = new List<string>();
            elements.Add(ParseFilterElement());

            while (true)
            {
                Token op = Peek();
                if (op != AND_OP1 && op != AND_OP2)
                    break;

                NextToken();
                elements.Add(ParseFilterElement());
            }

            if (elements.Count == 1)
                return elements[0];

            var sb = new StringBuilder("<and>");

            foreach (string element in elements)
                sb.Append(element);

            sb.Append("</and>");

            return sb.ToString();
        }

        /// <summary>
        /// Parse a single filter element such as a category expression
        /// and return the xml representation of the filter.
        /// </summary>
        public string ParseFilterElement()
        {
            Token nextOp = Peek();
            if (nextOp == LPAREN || nextOp == NOT_OP)
                return ParseExpressionInParentheses();

            string lhs = GetWord();

            switch (lhs)
            {
                case "id":
                    string op = GetOperator("=", "==");
                    string values = GetIdList();
                    return string.Format("<id>{0}</id>", values);

                case "cat":
                case "category":
                case "method":
                case "class":
                    op = GetOperator("=", "==", "!=", "=~", "!~");
                    values = GetWordList();
                    string tag = lhs == "category" ? "cat" : lhs;
                    switch (op)
                    {
                        case "=":
                        case "==":
                            return string.Format("<{0}>{1}</{0}>", tag, values);
                        case "!=":
                            return string.Format("<not><{0}>{1}</{0}></not>", tag, values);
                        default:
                            return string.Format("<{0} op='{1}'>{2}</{0}>", tag, op, values);
                    }

                case "name":
                case "test":
                case "fullname":
                    op = GetOperator("=", "==", "!=", "=~", "!~");
                    values = Peek().Kind == TokenKind.String
                        ? GetString()
                        : GetWord();
                    tag = lhs == "fullname" ? "test" : lhs;
                    switch (op)
                    {
                        case "=":
                        case "==":
                            return string.Format("<{0}>{1}</{0}>", tag, values);
                        case "!=":
                            return string.Format("<not><{0}>{1}</{0}></not>", tag, values);
                        default:
                            return string.Format("<{0} op={1}>{2}</{0}>", tag, op, values);
                    }

                default:
                    throw new NUnitEngineException("Unknown name '" + lhs + "' in where clause.");
            }
        }

        private string ParseExpressionInParentheses()
        {
            Token op = NextToken();
            bool negate = op == NOT_OP;
            Guard.OperationValid(negate || op == LPAREN, "Called with invalid token");

            if (negate && NextToken() != LPAREN)
                throw new NUnitEngineException("Expected '(' after '!' in where clause.");

            string result = ParseFilterExpression();

            if (NextToken() != RPAREN)
                throw new NUnitEngineException("Expected ')' in where clause.");

            if (negate)
                result = "<not>" + result + "</not>";

            return result;
        }

        private string GetWord()
        {
            return Expect(TokenKind.Word).Text;
        }

        private string GetWord(params string[] valid)
        {
            string name = GetWord();

            foreach (string item in valid)
                if (name == item)
                    return name;

            throw new NUnitEngineException("Unexpected name '" + name + "' in where clause.");
        }

        private string GetWordList()
        {
            string wordList = GetWord();

            while (Peek() == COMMA)
            {
                NextToken();
                wordList += ",";
                wordList += GetWord();
            }

            return wordList;
        }

        private string GetString()
        {
            return Expect(TokenKind.String).Text;
        }

        private string GetOperator()
        {
            return Expect(TokenKind.Symbol).Text;
        }

        private string GetOperator(params string[] valid)
        {
            string op = GetOperator();

            foreach (string item in valid)
                if (op == item)
                    return op;

            throw new NUnitEngineException("Unexpected operator '" + op + "' in where clause.");
        }

        private string GetIdList()
        {
            return null;
        }

        private Token Expect(TokenKind kind)
        {
            Token token = NextToken();

            if (token.Kind != kind)
                throw new NUnitEngineException("Unexpected token '" + token.Text + "' in where clause.");
             

            return token;
        }

        private Token Peek()
        {
            return _tokenizer.Peek();
        }

        private Token NextToken()
        {
            return _tokenizer.NextToken();
        }
    }
}
