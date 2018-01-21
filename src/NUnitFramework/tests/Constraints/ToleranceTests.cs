// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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

#region Using Directives

using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

#endregion

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class ToleranceTests
    {
        [Test, DefaultFloatingPointTolerance(0.1)]
        public void DefaultTolerance_Success()
        {
            Assert.That(2.05d, Is.EqualTo(2.0d));
        }

        [Test, DefaultFloatingPointTolerance(0.01)]
        public void DefaultTolerance_Failure()
        {
            Assert.That(2.05d, Is.Not.EqualTo(2.0d));
        }

        [Test, DefaultFloatingPointTolerance(0.5)]
        public void TestToleranceDefault()
        {
            var defaultTolerance = Tolerance.Default;
            Assert.IsTrue(defaultTolerance.IsUnsetOrDefault);

            var comparer = new NUnitEqualityComparer();
            Assert.IsTrue(comparer.AreEqual(2.0d, 2.1d, ref defaultTolerance ));
        }

        [Test, DefaultFloatingPointTolerance(0.5)]
        public void TestToleranceExact()
        {
            var noneTolerance = Tolerance.Exact;
            Assert.IsFalse(noneTolerance.IsUnsetOrDefault);

            var comparer = new NUnitEqualityComparer();
            Assert.IsFalse(comparer.AreEqual(2.0d, 2.1d, ref noneTolerance));
        }

        [Test]
        public void TestWithinCanOnlyBeUsedOnce()
        {
            Assert.That(() => Is.EqualTo(1.1d).Within(0.5d).Within(0.2d),
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("modifier may appear only once"));
        }

        [Test]
        public void TestModesCanOnlyBeUsedOnce()
        {
            var tolerance = new Tolerance(5);
            Assert.That(() => tolerance.Percent.Ulps,
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("multiple tolerance modes"));
        }

        [Test]
        public void TestNumericToleranceRequired()
        {
            var tolerance = new Tolerance("Five");
            Assert.That(() => tolerance.Percent,
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("numeric tolerance is required"));
        }

        [Test]
        public void TestModeMustFollowTolerance()
        {
            var tolerance = Tolerance.Default; // which is new Tolerance(0, ToleranceMode.Unset)
            Assert.That(() => tolerance.Percent, 
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("Tolerance amount must be specified"));
        }
    }
}