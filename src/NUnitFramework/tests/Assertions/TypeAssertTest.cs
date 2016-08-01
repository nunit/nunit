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

namespace NUnit.Framework.Assertions
{
    [TestFixture()]
    public class TypeAssertTests
    {
        [Test]
        public void ExactType()
        {
            Assert.That( "Hello", Is.TypeOf( typeof(System.String) ) );
        }

        [Test]
        public void ExactTypeFails()
        {
            var expectedMessage =
                "  Expected: <System.Int32>" + Environment.NewLine +
                "  But was:  <System.String>" + Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That( "Hello", Is.TypeOf( typeof(System.Int32) ) ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsInstanceOf()
        {
            var ex = new ArgumentException();

            Assert.IsInstanceOf(typeof(System.Exception), ex );
            Assert.That( ex, Is.InstanceOf(typeof(Exception)));
            Assert.IsInstanceOf<Exception>( ex );
        }

        [Test]
        public void IsInstanceOfFails()
        {
            var expectedMessage =
                "  Expected: instance of <System.Int32>" + System.Environment.NewLine + 
                "  But was:  <System.String>" + System.Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That( "abc123", Is.InstanceOf( typeof(System.Int32) ) ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void IsNotInstanceOf()
        {
            Assert.IsNotInstanceOf(typeof(System.Int32), "abc123" );
            Assert.That( "abc123", Is.Not.InstanceOf(typeof(System.Int32)) );
            Assert.IsNotInstanceOf<System.Int32>("abc123");
        }

        [Test]
        public void IsNotInstanceOfFails()
        {
            var expectedMessage =
                "  Expected: not instance of <System.Exception>" + System.Environment.NewLine + 
                "  But was:  <System.ArgumentException>" + System.Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.IsNotInstanceOf( typeof(System.Exception), new ArgumentException() ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test()]
        public void IsAssignableFrom()
        {
            int[] array10 = new int[10];

            Assert.IsAssignableFrom(typeof(int[]), array10);
            Assert.That(array10, Is.AssignableFrom(typeof(int[])));
            Assert.IsAssignableFrom<int[]>(array10);
        }

        [Test]
        public void IsAssignableFromFails()
        {
            int [] array10 = new int [10];
            int [,] array2 = new int[2,2];

            var expectedMessage =
                "  Expected: assignable from <System.Int32[,]>" + System.Environment.NewLine + 
                "  But was:  <System.Int32[]>" + System.Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That( array10, Is.AssignableFrom( array2.GetType() ) ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }

        [Test()]
        public void IsNotAssignableFrom()
        {
            int [] array10 = new int [10];

            Assert.IsNotAssignableFrom( typeof(int[,] ),array10);
            Assert.That( array10, Is.Not.AssignableFrom( typeof(int[,] ) ) );
            Assert.IsNotAssignableFrom<int[,]>(array10);
        }

        [Test]
        public void IsNotAssignableFromFails()
        {
            int [] array10 = new int [10];
            int [] array2 = new int[2];

            var expectedMessage =
                "  Expected: not assignable from <System.Int32[]>" + System.Environment.NewLine + 
                "  But was:  <System.Int32[]>" + System.Environment.NewLine;
            var ex = Assert.Throws<AssertionException>(() => Assert.That( array10, Is.Not.AssignableFrom( array2.GetType() ) ));
            Assert.That(ex.Message, Is.EqualTo(expectedMessage));
        }
    }
}
