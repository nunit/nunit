// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.Syntax
{
    public class DefaultConstraintSyntaxTests : SyntaxTest
    {
        public DefaultConstraintSyntaxTests()
        {
            ParseTree = "<default>";
            StaticSyntax = Is.Default;
            BuilderSyntax = Builder().Default;
        }
    }
}
