// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.Syntax
{
    public class NullTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<null>";
            StaticSyntax = Is.Null;
            BuilderSyntax = Builder().Null;
        }
    }

    public class TrueTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<true>";
            StaticSyntax = Is.True;
            BuilderSyntax = Builder().True;
        }
    }

    public class FalseTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<false>";
            StaticSyntax = Is.False;
            BuilderSyntax = Builder().False;
        }
    }

    public class PositiveTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<greaterthan 0>";
            StaticSyntax = Is.Positive;
            BuilderSyntax = Builder().Positive;
        }
    }

    public class NegativeTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<lessthan 0>";
            StaticSyntax = Is.Negative;
            BuilderSyntax = Builder().Negative;
        }
    }

    public class ZeroTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<equal 0>";
            StaticSyntax = Is.Zero;
            BuilderSyntax = Builder().Zero;
        }
    }

    public class NaNTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<nan>";
            StaticSyntax = Is.NaN;
            BuilderSyntax = Builder().NaN;
        }
    }
}
