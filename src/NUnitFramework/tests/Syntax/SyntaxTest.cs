// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Syntax
{
    public abstract class SyntaxTest
    {
        protected string? ParseTree;
        protected IResolveConstraint? StaticSyntax;
        protected IResolveConstraint? BuilderSyntax;

        protected ConstraintExpression Builder()
        {
            return new ConstraintExpression();
        }

        [Test]
        public void SupportedByStaticSyntax()
        {
            Assert.That(ParseTree, Is.Not.Null);
            Assert.That(StaticSyntax, Is.Not.Null);
            Assert.That(
                StaticSyntax.Resolve().ToString(),
                Is.EqualTo(ParseTree).NoClip);
        }

        [Test]
        public void SupportedByConstraintBuilder()
        {
            Assert.That(ParseTree, Is.Not.Null);
            Assert.That(BuilderSyntax, Is.Not.Null);
            Assert.That(
                BuilderSyntax.Resolve().ToString(),
                Is.EqualTo(ParseTree).NoClip);
        }
    }
}
