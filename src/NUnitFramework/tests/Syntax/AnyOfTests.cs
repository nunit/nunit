// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Syntax
{
    public class AnyOfTests : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<anyof 1 2 3>";
            staticSyntax = Is.AnyOf(1, 2, 3);
            builderSyntax = Builder().AnyOf(1, 2, 3);
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
            parseTree = "<anyof null>";
            staticSyntax = Is.AnyOf(null);
            builderSyntax = Builder().AnyOf(null);
        }
    }
}
