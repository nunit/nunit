// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests
{
    public class TextOutputTests
    {
        private const string SOME_TEXT = "Should go to the result";
        private const string ERROR_TEXT = "This should be written directly to console";
        private static readonly string NL = NUnit.Env.NewLine;

        private string CapturedOutput
        {
            get { return TestExecutionContext.CurrentContext.CurrentResult.Output; }
        }

        [Test]
        public void ConsoleWrite_WritesToResult()
        {
            Console.Write(SOME_TEXT);
            Assert.That(CapturedOutput, Is.EqualTo(SOME_TEXT));
        }

        [Test]
        public void ConsoleWriteLine_WritesToResult()
        {
            Console.WriteLine(SOME_TEXT);
            Assert.That(CapturedOutput, Is.EqualTo(SOME_TEXT + NL));
        }

        [Test]
        public void ConsoleError_WritesToResult()
        {
            Console.Error.WriteLine(SOME_TEXT);
            Assert.That(CapturedOutput, Is.EqualTo(SOME_TEXT + NL));
        }

        [Test]
        public void TestContextOut_WritesToResult()
        {
            TestContext.Out.WriteLine(SOME_TEXT);
            Assert.That(CapturedOutput, Is.EqualTo(SOME_TEXT + NL));
        }

        [Test]
        public void TestContextWrite_WritesToResult()
        {
            TestContext.Write(SOME_TEXT);
            Assert.That(CapturedOutput, Is.EqualTo(SOME_TEXT));
        }

        [Test]
        public void TestContextWriteLine_WritesToResult()
        {
            TestContext.WriteLine(SOME_TEXT);
            Assert.That(Internal.TestExecutionContext.CurrentContext.CurrentResult.Output, Is.EqualTo(SOME_TEXT + NL));
        }
    }
}
