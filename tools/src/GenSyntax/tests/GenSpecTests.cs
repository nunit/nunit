using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace GenSyntax.Tests
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
