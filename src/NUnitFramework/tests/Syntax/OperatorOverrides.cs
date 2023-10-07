// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Syntax
{
    public class NotOperatorOverride : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<not <null>>";
            StaticSyntax = !Is.Null;
            BuilderSyntax = !Builder().Null;
        }
    }

    public class AndOperatorOverride : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<and <greaterthan 5> <lessthan 10>>";
            StaticSyntax = Is.GreaterThan(5) & Is.LessThan(10);
            BuilderSyntax = Builder().GreaterThan(5) & Builder().LessThan(10);
        }
    }

    public class OrOperatorOverride : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<or <lessthan 5> <greaterthan 10>>";
            StaticSyntax = Is.LessThan(5) | Is.GreaterThan(10);
            BuilderSyntax = Builder().LessThan(5) | Is.GreaterThan(10);
        }
    }

    public class MixedOperatorOverrides
    {
        [Test]
        public void ComplexTests()
        {
            string expected = "<and <and <not <null>> <not <lessthan 5>>> <not <greaterthan 10>>>";

            Constraint c =
                Is.Not.Null & Is.Not.LessThan(5) & Is.Not.GreaterThan(10);
            Assert.That(c.ToString(), Is.EqualTo(expected).NoClip);

            c = !Is.Null & !Is.LessThan(5) & !Is.GreaterThan(10);
            Assert.That(c.ToString(), Is.EqualTo(expected).NoClip);
        }
    }
}
