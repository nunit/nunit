// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.TestData;

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

        [Test]
        public void NonDefaultUnionCases()
        {
            UnionMember member = new();

            GeneratedUnionType generatedUnion = member;
            CustomUnionType customUnion = member;
            ExplicitIUnionType explicitUnion = member;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(generatedUnion, Is.Not.Default);
                Assert.That(customUnion, Is.Not.Default);
                Assert.That(explicitUnion, Is.Not.Default);
            }
        }

        [Test]
        public void DefaultUnionCases()
        {
            GeneratedUnionType generatedUnion = default;
            ExplicitIUnionType explicitUnion = new(default(UnionMember));

            using (Assert.EnterMultipleScope())
            {
                Assert.That(generatedUnion, Is.Default);
                Assert.That(explicitUnion, Is.Default);
            }
        }
    }
}
