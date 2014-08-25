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
    public class SubstringTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<substring ""X"">";
            staticSyntax = Does.Contain("X");
            inheritedSyntax = Helper().Does.Contain("X");
            builderSyntax = Builder().Does.Contain("X");
        }
    }

    public class TextContains : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<substring ""X"">";
            staticSyntax = Does.Contain("X");
            inheritedSyntax = Helper().Does.Contain("X");
            builderSyntax = Builder().Does.Contain("X");
        }
    }

    public class SubstringTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<substring ""X"">";
            staticSyntax = Does.Contain("X").IgnoreCase;
            inheritedSyntax = Helper().Does.Contain("X").IgnoreCase;
            builderSyntax = Builder().Does.Contain("X").IgnoreCase;
        }
    }

    public class StartsWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<startswith ""X"">";
            staticSyntax = Does.StartWith("X");
            inheritedSyntax = Helper().StartsWith("X");
            builderSyntax = Builder().StartsWith("X");
        }
    }

    public class DoesStartWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<startswith ""X"">";
            staticSyntax = Does.StartWith("X");
            inheritedSyntax = Helper().Does.StartWith("X");
            builderSyntax = Builder().Does.StartWith("X");
        }
    }

    public class TextStartsWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<startswith ""X"">";
            staticSyntax = Does.StartWith("X");
            inheritedSyntax = Helper().StartsWith("X");
            builderSyntax = Builder().StartsWith("X");
        }
    }

    public class TextDoesStartWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<startswith ""X"">";
            staticSyntax = Does.StartWith("X");
            inheritedSyntax = Helper().Does.StartWith("X");
            builderSyntax = Builder().Does.StartWith("X");
        }
    }

    public class StartsWithTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<startswith ""X"">";
            staticSyntax = Does.StartWith("X").IgnoreCase;
            inheritedSyntax = Helper().Does.StartWith("X").IgnoreCase;
            builderSyntax = Builder().Does.StartWith("X").IgnoreCase;
        }
    }

    public class EndsWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<endswith ""X"">";
            staticSyntax = Does.EndWith("X");
            inheritedSyntax = Helper().EndsWith("X");
            builderSyntax = Builder().EndsWith("X");
        }
    }

    public class DoesEndWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<endswith ""X"">";
            staticSyntax = Does.EndWith("X");
            inheritedSyntax = Helper().Does.EndWith("X");
            builderSyntax = Builder().Does.EndWith("X");
        }
    }

    public class TextEndsWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<endswith ""X"">";
            staticSyntax = Does.EndWith("X");
            inheritedSyntax = Helper().EndsWith("X");
            builderSyntax = Builder().EndsWith("X");
        }
    }

    public class TextDoesEndWithTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<endswith ""X"">";
            staticSyntax = Does.EndWith("X");
            inheritedSyntax = Helper().Does.EndWith("X");
            builderSyntax = Builder().Does.EndWith("X");
        }
    }

    public class EndsWithTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<endswith ""X"">";
            staticSyntax = Does.EndWith("X").IgnoreCase;
            inheritedSyntax = Helper().Does.EndWith("X").IgnoreCase;
            builderSyntax = Builder().Does.EndWith("X").IgnoreCase;
        }
    }

#if !NETCF
    public class RegexTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<regex ""X"">";
            staticSyntax = Does.Match("X");
            inheritedSyntax = Helper().Does.Match("X");
            builderSyntax = Builder().Does.Match("X");
        }
    }

    public class TextMatchesTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<regex ""X"">";
            staticSyntax = Does.Match("X");
            inheritedSyntax = Helper().Matches("X");
            builderSyntax = Builder().Matches("X");
        }
    }

    public class TextDoesMatchesTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<regex ""X"">";
            staticSyntax = Does.Match("X");
            inheritedSyntax = Helper().Does.Match("X");
            builderSyntax = Builder().Does.Match("X");
        }
    }

    public class RegexTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = @"<regex ""X"">";
            staticSyntax = Does.Match("X").IgnoreCase;
            inheritedSyntax = Helper().Does.Match("X").IgnoreCase;
            builderSyntax = Builder().Does.Match("X").IgnoreCase;
        }
    }
#endif
}
