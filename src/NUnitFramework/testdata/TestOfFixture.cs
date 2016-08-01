// **********************************************************************************
// The MIT License (MIT)
// 
// Copyright (c) 2014 Charlie Poole
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// **********************************************************************************

#region Using Directives

using NUnit.Framework;

#endregion

namespace NUnit.TestData
{
    [TestFixture(TestOf = typeof(TestOfAttribute))]
    public class TestOfFixture
    {
        [Test(TestOf = typeof(TestOfAttribute))]
        public void Method()
        { }

        [Test]
        public void NoTestOfMethod()
        { }

        [Test]
        [TestOf(typeof(TestOfAttribute))]
        public void SeparateTestOfTypeMethod()
        { }

        [Test]
        [TestOf("NUnit.Framework.TestOfAttribute")]
        public void SeparateTestOfStringMethod()
        { }

        [Test, TestOf(typeof(TestAttribute))]
        [TestCase(5, TestOf = typeof(TestCaseAttribute))]
        public void TestCaseWithTestOf(int x)
        { }
    }
}