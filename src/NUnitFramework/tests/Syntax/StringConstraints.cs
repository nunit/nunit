// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
