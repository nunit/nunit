// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using Microsoft.CodeAnalysis.Emit;

namespace NUnit.Framework.Tests.Syntax
{
    [TestFixture]
    public class InvalidCodeTests
    {
        private static readonly string Template1 =
@"using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

class SomeClass
{
    void SomeMethod()
    {
        object c = $FRAGMENT$;
    }
}";

        [TestCase("Is.Null.Not")]
        [TestCase("Is.Not.Null.GreaterThan(10))")]
        [TestCase("Is.Null.All")]
        [TestCase("Is.And")]
        [TestCase("Is.All.And.And")]
        [TestCase("Is.Null.And.Throws")]
        [TestCase("Has.Some.Items")]
        public void CodeShouldNotCompile(string fragment)
        {
            string code = Template1.Replace("$FRAGMENT$", fragment);
            TestCompiler compiler = new TestCompiler(
                [typeof(Assert).Assembly.Location],
                "test.dll");
            EmitResult results = compiler.CompileCode(code);
            if (results.Success)
                Assert.Fail("Code fragment \"" + fragment + "\" should not compile but it did");
        }

        private static readonly string Template2 =
@"using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

class SomeClass
{
    void SomeMethod()
    {
        Assert.That(42, $FRAGMENT$);
    }
}";

        [TestCase("Is.Not")]
        [TestCase("Is.All")]
        [TestCase("Is.Not.All")]
        [TestCase("Is.All.Not")]
        [TestCase("Has.Some")]
        [TestCase("Has.Exactly(5)")]
        public void CodeShouldNotCompileAsFinishedConstraint(string fragment)
        {
            string code = Template2.Replace("$FRAGMENT$", fragment);
            TestCompiler compiler = new TestCompiler(
                ["nunit.framework.dll"],
                "test.dll");
            EmitResult results = compiler.CompileCode(code);
            if (results.Success)
                Assert.Fail("Code fragment \"" + fragment + "\" should not compile as a finished constraint but it did");
        }
    }
}
