// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.Syntax
{
    public class AnyOfTests : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<anyof 1 2 3>";
            StaticSyntax = Is.AnyOf(1, 2, 3);
            BuilderSyntax = Builder().AnyOf(1, 2, 3);
        }

        [Test]
        public void ThrowsExceptionIfNoValuesProvided_StaticSyntax()
        {
            Assert.That(() => Is.AnyOf(), Throws.ArgumentException);
        }

        [Test]
        public void ThrowsExceptionIfNoValuesProvided_ConstraintBuilder()
        {
            Assert.That(() => Builder().AnyOf(), Throws.ArgumentException);
        }
    }

    public class AnyOf_NullValue_Tests : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<anyof null>";
            StaticSyntax = Is.AnyOf(null);
            BuilderSyntax = Builder().AnyOf(null);
        }
    }
}
