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
using System.IO;

namespace NUnit.Framework.Syntax
{
    public class SamePathTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";
            string defaultCaseSensitivity = Path.DirectorySeparatorChar == '\\'
                ? "ignorecase" : "respectcase";

            ParseTree = string.Format(@"<samepath ""{0}"" {1}>", path, defaultCaseSensitivity);
            StaticSyntax = Is.SamePath(path);
            BuilderSyntax = Builder().SamePath(path);
        }
    }

    public class SamePathTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = string.Format(@"<samepath ""{0}"" ignorecase>", path);
            StaticSyntax = Is.SamePath(path).IgnoreCase;
            BuilderSyntax = Builder().SamePath(path).IgnoreCase;
        }
    }

    public class NotSamePathTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = string.Format(@"<not <samepath ""{0}"" ignorecase>>", path);
            StaticSyntax = Is.Not.SamePath(path).IgnoreCase;
            BuilderSyntax = Builder().Not.SamePath(path).IgnoreCase;
        }
    }

    public class SamePathTest_RespectCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = string.Format(@"<samepath ""{0}"" respectcase>", path);
            StaticSyntax = Is.SamePath(path).RespectCase;
            BuilderSyntax = Builder().SamePath(path).RespectCase;
        }
    }

    public class NotSamePathTest_RespectCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = string.Format(@"<not <samepath ""{0}"" respectcase>>", path);
            StaticSyntax = Is.Not.SamePath(path).RespectCase;
            BuilderSyntax = Builder().Not.SamePath(path).RespectCase;
        }
    }

    public class SamePathOrUnderTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";
            string defaultCaseSensitivity = Path.DirectorySeparatorChar == '\\'
                ? "ignorecase" : "respectcase";

            ParseTree = string.Format(@"<samepathorunder ""{0}"" {1}>", path, defaultCaseSensitivity);
            StaticSyntax = Is.SamePathOrUnder(path);
            BuilderSyntax = Builder().SamePathOrUnder(path);
        }
    }

    public class SamePathOrUnderTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = string.Format(@"<samepathorunder ""{0}"" ignorecase>", path);
            StaticSyntax = Is.SamePathOrUnder(path).IgnoreCase;
            BuilderSyntax = Builder().SamePathOrUnder(path).IgnoreCase;
        }
    }

    public class NotSamePathOrUnderTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = string.Format(@"<not <samepathorunder ""{0}"" ignorecase>>", path);
            StaticSyntax = Is.Not.SamePathOrUnder(path).IgnoreCase;
            BuilderSyntax = Builder().Not.SamePathOrUnder(path).IgnoreCase;
        }
    }

    public class SamePathOrUnderTest_RespectCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = string.Format(@"<samepathorunder ""{0}"" respectcase>", path);
            StaticSyntax = Is.SamePathOrUnder(path).RespectCase;
            BuilderSyntax = Builder().SamePathOrUnder(path).RespectCase;
        }
    }

    public class NotSamePathOrUnderTest_RespectCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = string.Format(@"<not <samepathorunder ""{0}"" respectcase>>", path);
            StaticSyntax = Is.Not.SamePathOrUnder(path).RespectCase;
            BuilderSyntax = Builder().Not.SamePathOrUnder(path).RespectCase;
        }
    }
}
