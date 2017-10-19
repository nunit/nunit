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

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class NoItemConstraintTests
    {   
        private readonly string NL = Environment.NewLine;
        
        [Test]
        public void NoItemsAreNotNull()
        {
            object[] c = new object[] { 1, "hello", 3, Environment.NewLine };
            Assert.That(c, new NoItemConstraint(Is.Null));
        }

        [Test]
        public void NoItemsAreNotNullFails()
        {
            object[] c = new object[] { 1, "hello", null, 3 };
            var expectedMessage =
                TextMessageWriter.Pfx_Expected + "no item null" + NL +
                TextMessageWriter.Pfx_Actual + "< 1, \"hello\", null, 3 >" + NL;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(c, new NoItemConstraint(Is.Null)));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
        
        [Test]
        public void FailsWhenNotUsedAgainstAnEnumerable()
        {
            var notEnumerable = 42;
            TestDelegate act = () => Assert.That(notEnumerable, new NoItemConstraint(Is.Null));
            Assert.That(act, Throws.ArgumentException.With.Message.Contains("IEnumerable"));
        }
    }
}
