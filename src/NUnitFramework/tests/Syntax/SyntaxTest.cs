// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Syntax
{
    public abstract class SyntaxTest
    {
        protected string ParseTree;
        protected IResolveConstraint StaticSyntax;
        protected IResolveConstraint BuilderSyntax;

        protected ConstraintExpression Builder()
        {
            return new ConstraintExpression();
        }

        [Test]
        public void SupportedByStaticSyntax()
        {
            Assert.That(
                StaticSyntax.Resolve().ToString(),
                Is.EqualTo(ParseTree).NoClip);
        }

        [Test]
        public void SupportedByConstraintBuilder()
        {
            Assert.That(
                BuilderSyntax.Resolve().ToString(),
                Is.EqualTo(ParseTree).NoClip);
        }
    }
}
