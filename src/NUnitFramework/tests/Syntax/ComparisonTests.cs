// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.Syntax
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
