// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Assertions
{
    [TestFixture]
    public class SameFixture
    {
        [Test]
        public void Same()
        {
            string s1 = "S1";
            Assert.That(s1, Is.SameAs(s1));
        }

        [Test]
        public void SameFails()
        {
            var ex1 = new Exception("one");
            var ex2 = new Exception("two");
            var expectedMessage =
                "  Expected: same as <System.Exception: one>" + Environment.NewLine +
                "  But was:  <System.Exception: two>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(ex2, Is.SameAs(ex1)));
            Assert.That(ex?.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public void SameUsingInternedConstStrings()
        {
            string s1 = string.Intern(new string('A', 2));
            string s2 = string.Intern(s1);
            Assert.That(s1, Is.SameAs(s2));
        }

        [Test]
        public void ShouldNotCallToStringOnClassForPassingTests()
        {
            var actual = new ThrowsIfToStringIsCalled(1);

            Assert.That(actual, Is.SameAs(actual));
        }

        [Test]
        public void SameValueTypes_CantCompile()
        {
            var testCompiler = new TestCompiler(typeof(Assert));
            var result = testCompiler.CompileCode(ValueTypeCode);

            Assert.That(result.Success, Is.False);

            var error = result.Diagnostics.First(x => x.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(error.Id, Is.EqualTo("CS0452"));
                Assert.That(error.GetMessage(), Does.StartWith("The type 'int' must be a reference type in order to use it as parameter 'T'"));
            }
        }

        private const string ValueTypeCode = @"
            using NUnit.Framework;

            [TestFixture]
            public class SameAsValueTypeFixture
            {
                [Test]
                public void SameValueTypes()
                {
                    int index = 2;
                    Assert.That(2, Is.SameAs(index));
                }
            }
            ";
    }
}
