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
    public class NullTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<null>";
            staticSyntax = Is.Null;
            builderSyntax = Builder().Null;
        }
    }

    public class TrueTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<true>";
            staticSyntax = Is.True;
            builderSyntax = Builder().True;
        }
    }

    public class FalseTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<false>";
            staticSyntax = Is.False;
            builderSyntax = Builder().False;
        }
    }

    public class PositiveTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<greaterthan 0>";
            staticSyntax = Is.Positive;
            builderSyntax = Builder().Positive;
        }
    }

    public class NegativeTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<lessthan 0>";
            staticSyntax = Is.Negative;
            builderSyntax = Builder().Negative;
        }
    }

    public class ZeroTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<equal 0>";
            staticSyntax = Is.Zero;
            builderSyntax = Builder().Zero;
        }
    }

    public class NaNTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<nan>";
            staticSyntax = Is.NaN;
            builderSyntax = Builder().NaN;
        }
    }
}
