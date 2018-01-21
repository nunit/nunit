// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

#if !(NETCOREAPP1_1 || NETCOREAPP2_0)
using System;
using System.Collections;
using System.CodeDom.Compiler;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Syntax
{
    [TestFixture]
    public class InvalidCodeTests
    {
        static readonly string template1 =
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
            string code = template1.Replace("$FRAGMENT$", fragment);
            TestCompiler compiler = new TestCompiler(
                new string[] { "system.dll", "nunit.framework.dll" },
                "test.dll");
            CompilerResults results = compiler.CompileCode(code);
            if (results.NativeCompilerReturnValue == 0)
                Assert.Fail("Code fragment \"" + fragment + "\" should not compile but it did");
        }

        static readonly string template2 =
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
            string code = template2.Replace("$FRAGMENT$", fragment);
            TestCompiler compiler = new TestCompiler(
                new string[] { "system.dll", "nunit.framework.dll" },
                "test.dll");
            CompilerResults results = compiler.CompileCode(code);
            if (results.NativeCompilerReturnValue == 0)
                Assert.Fail("Code fragment \"" + fragment + "\" should not compile as a finished constraint but it did");
        }
    }
}
#endif
