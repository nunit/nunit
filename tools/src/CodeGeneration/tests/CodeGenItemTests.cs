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
    public class CodeGenItemTests
    {
        [Test, ExpectedException(typeof(ArgumentException))]
        public void ItemWithoutColonThrowsException()
        {
            CodeGenItem item = new CodeGenItem("Gen x.f()=>y");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ItemWithoutArrowThrowsException()
        {
            CodeGenItem item = new CodeGenItem("Gen: x.f()=y");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ItemWithoutDotThrowsException()
        {
            CodeGenItem item = new CodeGenItem("Gen: x=>y");
        }

        [Test]
        public void BasicCodeGenItem()
        {
            CodeGenItem item = new CodeGenItem("Gen: x.f(string s)=>y.g(s)");
            Assert.That(item.ItemType, Is.EqualTo("Gen:"));
            Assert.That(item.LeftPart, Is.EqualTo("x.f(string s)"));
            Assert.That(item.RightPart, Is.EqualTo("y.g(s)"));
            Assert.That(item.ClassName, Is.EqualTo("x"));
            Assert.That(item.MethodName, Is.EqualTo("f(string s)"));
            Assert.That(item.Attributes, Is.EqualTo(null));
            Assert.False(item.IsProperty);
            Assert.False(item.IsGeneric);
        }

        [Test]
        public void ItemWithAttribute()
        {
            CodeGenItem item = new CodeGenItem("Gen: [Obsolete]x.f(string s)=>y.g(s)");
            Assert.That(item.ItemType, Is.EqualTo("Gen:"));
            Assert.That(item.LeftPart, Is.EqualTo("[Obsolete]x.f(string s)"));
            Assert.That(item.RightPart, Is.EqualTo("y.g(s)"));
            Assert.That(item.ClassName, Is.EqualTo("x"));
            Assert.That(item.MethodName, Is.EqualTo("f(string s)"));
            Assert.That(item.Attributes, Is.EqualTo("[Obsolete]"));
            Assert.False(item.IsProperty);
            Assert.False(item.IsGeneric);
        }

        [Test]
        public void ItemWithAttribute_ArgumentContainingDot()
        {
            CodeGenItem item = new CodeGenItem("Gen: [Obsolete(\"Use A.f()\")]x.f(string s)=>y.g(s)");
            Assert.That(item.ItemType, Is.EqualTo("Gen:"));
            Assert.That(item.LeftPart, Is.EqualTo("[Obsolete(\"Use A.f()\")]x.f(string s)"));
            Assert.That(item.RightPart, Is.EqualTo("y.g(s)"));
            Assert.That(item.ClassName, Is.EqualTo("x"));
            Assert.That(item.MethodName, Is.EqualTo("f(string s)"));
            Assert.That(item.Attributes, Is.EqualTo("[Obsolete(\"Use A.f()\")]"));
            Assert.False(item.IsProperty);
            Assert.False(item.IsGeneric);
        }

        [Test]
        public void ItemDefinesProperty()
        {
            CodeGenItem item = new CodeGenItem("Gen: Is.Null=>new NullConstraint()");
            Assert.That(item.ItemType, Is.EqualTo("Gen:"));
            Assert.That(item.LeftPart, Is.EqualTo("Is.Null"));
            Assert.That(item.RightPart, Is.EqualTo("new NullConstraint()"));
            Assert.That(item.ClassName, Is.EqualTo("Is"));
            Assert.That(item.MethodName, Is.EqualTo("Null"));
            Assert.That(item.Attributes, Is.EqualTo(null));
            Assert.True(item.IsProperty);
            Assert.False(item.IsGeneric);
        }

        [Test]
        public void ItemIsGeneric()
        {
            CodeGenItem item = new CodeGenItem("Gen: Is.TypeOf<T>()=>new ExactTypeConstraint<T>()");
            Assert.That(item.ItemType, Is.EqualTo("Gen:"));
            Assert.That(item.LeftPart, Is.EqualTo("Is.TypeOf<T>()"));
            Assert.That(item.RightPart, Is.EqualTo("new ExactTypeConstraint<T>()"));
            Assert.That(item.ClassName, Is.EqualTo("Is"));
            Assert.That(item.MethodName, Is.EqualTo("TypeOf<T>()"));
            Assert.That(item.Attributes, Is.EqualTo(null));
            Assert.False(item.IsProperty);
            Assert.True(item.IsGeneric);
        }
    }
}
