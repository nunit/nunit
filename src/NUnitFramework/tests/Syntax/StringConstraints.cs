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

using System;

namespace NUnit.Framework.Syntax
{
    public class ContainsTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = @"<contains>";
            StaticSyntax = Does.Contain("X");
            BuilderSyntax = Builder().Contains("X");
        }
    }

    public class ContainsTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = @"<contains>";
            StaticSyntax = Does.Contain("X").IgnoreCase;
            BuilderSyntax = Builder().Contains("X").IgnoreCase;
        }
    }

    public class StartsWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = @"<startswith ""X"">";
            StaticSyntax = Does.StartWith("X");
            BuilderSyntax = Builder().StartsWith("X");
        }
    }

    public class TextStartsWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = @"<startswith ""X"">";
            StaticSyntax = Does.StartWith("X");
            BuilderSyntax = Builder().StartsWith("X");
        }
    }

    public class StartsWithTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = @"<startswith ""X"">";
            StaticSyntax = Does.StartWith("X").IgnoreCase;
            BuilderSyntax = Builder().StartsWith("X").IgnoreCase;
        }
    }

    public class EndsWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = @"<endswith ""X"">";
            StaticSyntax = Does.EndWith("X");
            BuilderSyntax = Builder().EndsWith("X");
        }
    }

    public class EndsWithTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = @"<endswith ""X"">";
            StaticSyntax = Does.EndWith("X").IgnoreCase;
            BuilderSyntax = Builder().EndsWith("X").IgnoreCase;
        }
    }

    public class RegexTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = @"<regex ""X"">";
            StaticSyntax = Does.Match("X");
            BuilderSyntax = Builder().Matches("X");
        }
    }

    public class RegexTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = @"<regex ""X"">";
            StaticSyntax = Does.Match("X").IgnoreCase;
            BuilderSyntax = Builder().Matches("X").IgnoreCase;
        }
    }
}
