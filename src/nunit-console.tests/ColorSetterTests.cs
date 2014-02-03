// ***********************************************************************
// Copyright (c) 2014 Rob Prouse
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
    [TestFixture]
    public class ColorSetterTests
    {
        [SetUp]
        public void SetUp()
        {
            var options = new ConsoleOptions( new[] {"--color"} );
            ColorSetter.Options = options;

            // Set to an unknown, unlikely color so that we can test for change
            Console.ForegroundColor = ConsoleColor.Magenta;
        }

        [TearDown]
        public void TearDown()
        {
            Console.ResetColor();
        }

        [Test]
        public void TestConstructor()
        {
            ConsoleColor expected = ColorSetter.GetColor( ColorStyle.Error );
            using ( new ColorSetter( ColorStyle.Error ) )
            {
                Assert.That(Console.ForegroundColor, Is.EqualTo(expected));
            }
            Assert.That( Console.ForegroundColor, Is.Not.EqualTo(expected) );
        }

        [Test]
        public void TestNoOption()
        {
            var options = new ConsoleOptions(new[] { "" });
            ColorSetter.Options = options;

            using (new ColorSetter(ColorStyle.Error))
            {
                Assert.That(Console.ForegroundColor, Is.EqualTo(ConsoleColor.Magenta));
            }
            Assert.That(Console.ForegroundColor, Is.EqualTo(ConsoleColor.Magenta));
        }

        [TestCase(ColorStyle.Pass, ConsoleColor.Green)]
        [TestCase(ColorStyle.Failure, ConsoleColor.Red)]
        [TestCase(ColorStyle.Warning, ConsoleColor.Yellow)]
        [TestCase(ColorStyle.Error, ConsoleColor.DarkRed)]
        public void TestGetColor( ColorStyle style, ConsoleColor expected )
        {
            Assert.That( ColorSetter.GetColor( style ), Is.EqualTo( expected ) );
        }
    }
}
