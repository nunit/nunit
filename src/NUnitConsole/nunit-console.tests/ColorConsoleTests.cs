// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using NUnit.Framework;

namespace NUnit.ConsoleRunner.Tests
{
    using Utilities;

    [TestFixture]
    public class ColorConsoleTests
    {
        private ColorStyle _testStyle;

        [SetUp]
        public void SetUp()
        {
            // Find a test color that is different than the console color
            if (Console.ForegroundColor != ColorConsole.GetColor( ColorStyle.Error ))
                _testStyle = ColorStyle.Error;
            else if (Console.ForegroundColor != ColorConsole.GetColor( ColorStyle.Pass ))
                _testStyle = ColorStyle.Pass;
            else
                Assert.Inconclusive("Could not find a color to test with");

            ColorConsole.Enabled = true;

            // Set to an unknown, unlikely color so that we can test for change
            Console.ForegroundColor = ConsoleColor.Magenta;

            Assume.That(Console.ForegroundColor, Is.EqualTo(ConsoleColor.Magenta), "Color tests cannot be run because the current console does not support color");
        }

        [TearDown]
        public void TearDown()
        {
            Console.ResetColor();
        }

        [Test]
        public void TestConstructor()
        {
            ConsoleColor expected = ColorConsole.GetColor(_testStyle);
            using(new ColorConsole(_testStyle))
            {
                Assert.That(Console.ForegroundColor, Is.EqualTo(expected));
            }
            Assert.That( Console.ForegroundColor, Is.Not.EqualTo(expected) );
        }

        [Test]
        public void TestNoColorOption()
        {
            ColorConsole.Enabled = false;

            using(new ColorConsole(_testStyle))
            {
                Assert.That(Console.ForegroundColor, Is.EqualTo(ConsoleColor.Magenta));
            }
            Assert.That(Console.ForegroundColor, Is.EqualTo(ConsoleColor.Magenta));
        }
    }
}
