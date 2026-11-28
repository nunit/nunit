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

        [TestCaseSource(typeof(UnionTestData), nameof(UnionTestData.InitializedValueTestCases))]
        public void NonDefaultUnionCases<TUnion>(TUnion instance)
        {
            Assert.That(instance, Is.Not.Default);
        }

        [TestCaseSource(typeof(UnionTestData), nameof(UnionTestData.DefaultValueTestCases))]
        public void DefaultUnionCases<TUnion>(TUnion instance)
        {
            Assert.That(instance, Is.Default);
        }
    }
}
