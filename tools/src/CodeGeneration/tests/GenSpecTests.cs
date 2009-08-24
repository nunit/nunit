// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NUnit.Framework.CodeGeneration
{
    [TestFixture]
    public class GenSpecTests
    {
        [Test, ExpectedException(typeof(ArgumentException))]
        public void SpecWithoutColonThrowsException()
        {
            GenSpec spec = new GenSpec("Gen x.f()=>y");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void SpecWithoutArrowThrowsException()
        {
            GenSpec spec = new GenSpec("Gen: x.f()=y");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void SpecWithoutDotThrowsException()
        {
            GenSpec spec = new GenSpec("Gen: x=>y");
        }

        [Test]
        public void BasicSpec()
        {
            GenSpec spec = new GenSpec("Gen: x.f(string s)=>y.g(s)");
            Assert.That(spec.SpecType, Is.EqualTo("Gen:"));
            Assert.That(spec.LeftPart, Is.EqualTo("x.f(string s)"));
            Assert.That(spec.RightPart, Is.EqualTo("y.g(s)"));
            Assert.That(spec.ClassName, Is.EqualTo("x"));
            Assert.That(spec.MethodName, Is.EqualTo("f(string s)"));
            Assert.That(spec.Attributes, Is.EqualTo(null));
            Assert.False(spec.IsProperty);
            Assert.False(spec.IsGeneric);
        }

        [Test]
        public void SpecWithAttribute()
        {
            GenSpec spec = new GenSpec("Gen: [Obsolete]x.f(string s)=>y.g(s)");
            Assert.That(spec.SpecType, Is.EqualTo("Gen:"));
            Assert.That(spec.LeftPart, Is.EqualTo("[Obsolete]x.f(string s)"));
            Assert.That(spec.RightPart, Is.EqualTo("y.g(s)"));
            Assert.That(spec.ClassName, Is.EqualTo("x"));
            Assert.That(spec.MethodName, Is.EqualTo("f(string s)"));
            Assert.That(spec.Attributes, Is.EqualTo("[Obsolete]"));
            Assert.False(spec.IsProperty);
            Assert.False(spec.IsGeneric);
        }

        [Test]
        public void SpecWithAttribute_ArgumentContainingDot()
        {
            GenSpec spec = new GenSpec("Gen: [Obsolete(\"Use A.f()\")]x.f(string s)=>y.g(s)");
            Assert.That(spec.SpecType, Is.EqualTo("Gen:"));
            Assert.That(spec.LeftPart, Is.EqualTo("[Obsolete(\"Use A.f()\")]x.f(string s)"));
            Assert.That(spec.RightPart, Is.EqualTo("y.g(s)"));
            Assert.That(spec.ClassName, Is.EqualTo("x"));
            Assert.That(spec.MethodName, Is.EqualTo("f(string s)"));
            Assert.That(spec.Attributes, Is.EqualTo("[Obsolete(\"Use A.f()\")]"));
            Assert.False(spec.IsProperty);
            Assert.False(spec.IsGeneric);
        }

        [Test]
        public void SpecDefinesProperty()
        {
            GenSpec spec = new GenSpec("Gen: Is.Null=>new NullConstraint()");
            Assert.That(spec.SpecType, Is.EqualTo("Gen:"));
            Assert.That(spec.LeftPart, Is.EqualTo("Is.Null"));
            Assert.That(spec.RightPart, Is.EqualTo("new NullConstraint()"));
            Assert.That(spec.ClassName, Is.EqualTo("Is"));
            Assert.That(spec.MethodName, Is.EqualTo("Null"));
            Assert.That(spec.Attributes, Is.EqualTo(null));
            Assert.True(spec.IsProperty);
            Assert.False(spec.IsGeneric);
        }

        [Test]
        public void SpecIsGeneric()
        {
            GenSpec spec = new GenSpec("Gen: Is.TypeOf<T>()=>new ExactTypeConstraint<T>()");
            Assert.That(spec.SpecType, Is.EqualTo("Gen:"));
            Assert.That(spec.LeftPart, Is.EqualTo("Is.TypeOf<T>()"));
            Assert.That(spec.RightPart, Is.EqualTo("new ExactTypeConstraint<T>()"));
            Assert.That(spec.ClassName, Is.EqualTo("Is"));
            Assert.That(spec.MethodName, Is.EqualTo("TypeOf<T>()"));
            Assert.That(spec.Attributes, Is.EqualTo(null));
            Assert.False(spec.IsProperty);
            Assert.True(spec.IsGeneric);
        }
    }
}
