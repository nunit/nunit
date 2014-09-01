// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Threading;
using System.Globalization;
using NUnit.Framework;

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
        public void IsTrueFails()
        {
            var expectedMessage =
                "  Expected: True" + Env.NewLine +
                "  But was:  False" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsTrue(false));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsFalse()
        {
            Assert.IsFalse(false);
        }

        [Test]
        public void IsFalseFails()
        {
            var expectedMessage =
                "  Expected: False" + Env.NewLine +
                "  But was:  True" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsFalse(true));
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
                "  Expected: null" + Env.NewLine +
                "  But was:  \"S1\"" + Env.NewLine;
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
                "  Expected: not null" + Env.NewLine +
                "  But was:  null" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotNull(null));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    
#if !NUNITLITE
        [Test]
        public void IsNaN()
        {
            Assert.IsNaN(double.NaN);
        }

        [Test]
        public void IsNaNFails()
        {
            var expectedMessage =
                "  Expected: NaN" + Env.NewLine +
                "  But was:  10.0d" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNaN(10.0));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsEmpty()
        {
            Assert.IsEmpty( "", "Failed on empty String" );
            Assert.IsEmpty( new int[0], "Failed on empty Array" );
            Assert.IsEmpty( new ArrayList(), "Failed on empty ArrayList" );
            Assert.IsEmpty( new Hashtable(), "Failed on empty Hashtable" );
            Assert.IsEmpty( (IEnumerable)new int[0], "Failed on empty IEnumerable" );
        }

        [Test]
        public void IsEmptyFailsOnString()
        {
            var expectedMessage =
                "  Expected: <empty>" + Env.NewLine +
                "  But was:  \"Hi!\"" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsEmpty( "Hi!" ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNullString()
        {
            var expectedMessage =
                "  Expected: <empty>" + Env.NewLine +
                "  But was:  null" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsEmpty( (string)null ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsEmptyFailsOnNonEmptyArray()
        {
            var expectedMessage =
                "  Expected: <empty>" + Env.NewLine +
                "  But was:  < 1, 2, 3 >" + Env.NewLine;
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
            ArrayList list = new ArrayList( array );
            Hashtable hash = new Hashtable();
            hash.Add( "array", array );

            Assert.IsNotEmpty( "Hi!", "Failed on String" );
            Assert.IsNotEmpty( array, "Failed on Array" );
            Assert.IsNotEmpty( list, "Failed on ArrayList" );
            Assert.IsNotEmpty( hash, "Failed on Hashtable" );
            Assert.IsNotEmpty( (IEnumerable)array, "Failed on IEnumerable" );
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyString()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Env.NewLine +
                "  But was:  <string.Empty>" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty( "" ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyArray()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Env.NewLine +
                "  But was:  <empty>" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty( new int[0] ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyArrayList()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Env.NewLine +
                "  But was:  <empty>" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty( new ArrayList() ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotEmptyFailsOnEmptyHashTable()
        {
            var expectedMessage =
                "  Expected: not <empty>" + Env.NewLine +
                "  But was:  <empty>" + Env.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotEmpty( new Hashtable() ));
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
#endif
    }
}
