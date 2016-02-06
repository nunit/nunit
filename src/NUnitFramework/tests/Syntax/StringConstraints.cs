// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
            parseTree = @"<contains>";
            staticSyntax = Does.Contain("X");
            builderSyntax = Builder().Contains("X");
        }
    }

    public class ContainsTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<contains>";
            staticSyntax = Does.Contain("X").IgnoreCase;
            builderSyntax = Builder().Contains("X").IgnoreCase;
        }
    }

    public class StartsWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<startswith ""X"">";
            staticSyntax = Does.StartWith("X");
            builderSyntax = Builder().StartsWith("X");
        }
    }

    public class TextStartsWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<startswith ""X"">";
            staticSyntax = Does.StartWith("X");
            builderSyntax = Builder().StartsWith("X");
        }
    }

    public class StartsWithTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<startswith ""X"">";
            staticSyntax = Does.StartWith("X").IgnoreCase;
            builderSyntax = Builder().StartsWith("X").IgnoreCase;
        }
    }

    public class EndsWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<endswith ""X"">";
            staticSyntax = Does.EndWith("X");
            builderSyntax = Builder().EndsWith("X");
        }
    }

    public class EndsWithTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<endswith ""X"">";
            staticSyntax = Does.EndWith("X").IgnoreCase;
            builderSyntax = Builder().EndsWith("X").IgnoreCase;
        }
    }

    public class RegexTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<regex ""X"">";
            staticSyntax = Does.Match("X");
            builderSyntax = Builder().Matches("X");
        }
    }

    public class RegexTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<regex ""X"">";
            staticSyntax = Does.Match("X").IgnoreCase;
            builderSyntax = Builder().Matches("X").IgnoreCase;
        }
    }
}
