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
using System.Collections;

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class ConditionAssertTests
    {
        [Test]
        public void IsTrue()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void IsTrueNullable()
        {
            bool? actual = true;
            Assert.IsTrue(actual);
        }

        [Test]
        public void IsTrueFails()
        {
            var expectedMessage =
                "  Expected: True" + Environment.NewLine +
                "  But was:  False" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsTrue(false));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [TestCase(false, "  But was:  False")]
        [TestCase(null,"  But was:  null")]
        public void IsTrueFailsForNullable(bool? actual, string expectedButWas)
        {
            var expectedMessage =
                "  Expected: True" + Environment.NewLine +
                expectedButWas + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsTrue(actual));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsFalse()
        {
            Assert.IsFalse(false);
        }

        [Test]
        public void IsFalseNullable()
        {
            bool? actual = false;
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsFalseFails()
        {
            var expectedMessage =
                "  Expected: False" + Environment.NewLine +
                "  But was:  True" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsFalse(true));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [TestCase(true,"  But was:  True")]
        [TestCase(null, "  But was:  null")]
        public void IsFalseFailsForNullable(bool? actual, string expectedButWas)
        {
            var expectedMessage =
                "  Expected: False" + Environment.NewLine +
                expectedButWas + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsFalse(actual));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    
        [Test]
        public void IsNull()
        {
            Assert.IsNull(null);
        }

        [Test]
        public void IsNullFails()
        {
            String s1 = "S1";
            var expectedMessage =
                "  Expected: null" + Environment.NewLine +
                "  But was:  \"S1\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNull(s1));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    
        [Test]
        public void IsNotNull()
        {
            String s1 = "S1";
            Assert.IsNotNull(s1);
        }

        [Test]
        public void IsNotNullFails()
        {
            var expectedMessage =
                "  Expected: not null" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotNull(null));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    
        [Test]
        public void IsNaN()
        {
            Assert.IsNaN(double.NaN);
        }

        [Test]
        public void IsNaNFails()
        {
            var expectedMessage =
                "  Expected: NaN" + Environment.NewLine +
                "  But was:  10.0d" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNaN(10.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsEmpty()
        {
            Assert.IsEmpty( "", "Failed on empty String" );
            Assert.IsEmpty( new int[0], "Failed on empty Array" );
            Assert.IsEmpty((IEnumerable)new int[0], "Failed on empty IEnumerable");

#if !NETCOREAPP1_1
            Assert.IsEmpty( new ArrayList(), "Failed on empty ArrayList" );
            Assert.IsEmpty( new Hashtable(), "Failed on empty Hashtable" );
#endif
        }

        [Test]
        public void IsEmptyFailsOnString()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  \"Hi!\"" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsEmpty( "Hi!" ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNullString()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsEmpty( (string)null ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNonEmptyArray()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  < 1, 2, 3 >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsEmpty( new int[] { 1, 2, 3 } ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNonEmptyIEnumerable()
        {
            var expectedMessage =
                "  Expected: <empty>" + Environment.NewLine +
                "  But was:  < 1, 2, 3 >" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsEmpty((IEnumerable)new int[] { 1, 2, 3 }));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
 
        [Test]
        public void IsNotEmpty()
        {
            int[] array = new int[] { 1, 2, 3 };

            Assert.IsNotEmpty( "Hi!", "Failed on String" );
            Assert.IsNotEmpty( array, "Failed on Array" );
            Assert.IsNotEmpty( (IEnumerable)array, "Failed on IEnumerable" );

#if !NETCOREAPP1_1
            ArrayList list = new ArrayList(array);
            Hashtable hash = new Hashtable();
            hash.Add("array", array);

            Assert.IsNotEmpty(list, "Failed on ArrayList");
            Assert.IsNotEmpty(hash, "Failed on Hashtable");
#endif
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyString()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <string.Empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty( "" ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyArray()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty( new int[0] ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyIEnumerable()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty((IEnumerable)new int[0]));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

#if !NETCOREAPP1_1
        [Test]
        public void IsNotEmptyFailsOnEmptyArrayList()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty(new ArrayList()));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyHashTable()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty(new Hashtable()));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
#endif
    }
}
