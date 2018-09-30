// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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
using System.IO;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class FloatingPointNumericsTests
    {

        /// <summary>Tests the floating point value comparison helper</summary>
        [Test]
        public void FloatEqualityWithUlps()
        {
            Assert.IsTrue(
                FloatingPointNumerics.AreAlmostEqualUlps(0.00000001f, 0.0000000100000008f, 1)
            );
            Assert.IsFalse(
                FloatingPointNumerics.AreAlmostEqualUlps(0.00000001f, 0.0000000100000017f, 1)
            );

            Assert.IsTrue(
                FloatingPointNumerics.AreAlmostEqualUlps(1000000.00f, 1000000.06f, 1)
            );
            Assert.IsFalse(
                FloatingPointNumerics.AreAlmostEqualUlps(1000000.00f, 1000000.13f, 1)
            );
            Assert.IsFalse( // Ensure we don't overflow on twos complement values
                FloatingPointNumerics.AreAlmostEqualUlps(2.0f, -2.0f, 1)
            );
        }

        /// <summary>Tests the double precision floating point value comparison helper</summary>
        [Test]
        public void DoubleEqualityWithUlps()
        {
            Assert.IsTrue(
                FloatingPointNumerics.AreAlmostEqualUlps(0.00000001, 0.000000010000000000000002, 1)
            );
            Assert.IsFalse(
                FloatingPointNumerics.AreAlmostEqualUlps(0.00000001, 0.000000010000000000000004, 1)
            );

            Assert.IsTrue(
                FloatingPointNumerics.AreAlmostEqualUlps(1000000.00, 1000000.0000000001, 1)
            );
            Assert.IsFalse(
                FloatingPointNumerics.AreAlmostEqualUlps(1000000.00, 1000000.0000000002, 1)
            );
            Assert.IsFalse( // Ensure we don't overflow on twos complement values
                FloatingPointNumerics.AreAlmostEqualUlps(2.0, -2.0, 1)
            );
        }
    }
}
