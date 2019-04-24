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
    public class GreaterThanTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<greaterthan 7>";
            StaticSyntax = Is.GreaterThan(7);
            BuilderSyntax = Builder().GreaterThan(7);
        }
    }

    public class GreaterThanOrEqualTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<greaterthanorequal 7>";
            StaticSyntax = Is.GreaterThanOrEqualTo(7);
            BuilderSyntax = Builder().GreaterThanOrEqualTo(7);
        }
    }

    public class AtLeastTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<greaterthanorequal 7>";
            StaticSyntax = Is.AtLeast(7);
            BuilderSyntax = Builder().AtLeast(7);
        }
    }

    public class LessThanTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<lessthan 7>";
            StaticSyntax = Is.LessThan(7);
            BuilderSyntax = Builder().LessThan(7);
        }
    }

    public class LessThanOrEqualTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<lessthanorequal 7>";
            StaticSyntax = Is.LessThanOrEqualTo(7);
            BuilderSyntax = Builder().LessThanOrEqualTo(7);
        }
    }

    public class AtMostTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<lessthanorequal 7>";
            StaticSyntax = Is.AtMost(7);
            BuilderSyntax = Builder().AtMost(7);
        }
    }
}
