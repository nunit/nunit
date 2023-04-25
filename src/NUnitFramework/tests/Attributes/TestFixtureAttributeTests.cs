// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Attributes
{
    public class TestFixtureAttributeTests
    {
        static readonly object[] fixtureArgs = { 10, 20, "Charlie" };
        static readonly Type[] typeArgs = { typeof(int), typeof(string) };
        static readonly object[] combinedArgs = { typeof(int), typeof(string), 10, 20, "Charlie" };

        [Test]
        public void ConstructWithoutArguments()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute();
            Assert.Multiple(() =>
            {
                Assert.That(attr.Arguments, Is.Empty);
                Assert.That(attr.TypeArgs, Is.Empty);
            });
        }

        [Test]
        public void ConstructWithFixtureArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(fixtureArgs);
            Assert.Multiple(() =>
            {
                Assert.That(attr.Arguments, Is.EqualTo(fixtureArgs));
                Assert.That(attr.TypeArgs, Is.Empty);
            });
        }

        [Test]
        public void ConstructWithJustTypeArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(typeArgs);
            Assert.Multiple(() =>
            {
                Assert.That(attr.Arguments, Has.Length.EqualTo(2));
                Assert.That(attr.TypeArgs, Is.Empty);
            });
        }

        [Test]
        public void ConstructWithNoArgumentsAndSetTypeArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute();
            attr.TypeArgs = typeArgs;
            Assert.Multiple(() =>
            {
                Assert.That(attr.Arguments, Is.Empty);
                Assert.That(attr.TypeArgs, Is.EqualTo(typeArgs));
            });
        }

        [Test]
        public void ConstructWithFixtureArgsAndSetTypeArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(fixtureArgs);
            attr.TypeArgs = typeArgs;
            Assert.Multiple(() =>
            {
                Assert.That(attr.Arguments, Is.EqualTo(fixtureArgs));
                Assert.That(attr.TypeArgs, Is.EqualTo(typeArgs));
            });
        }

        [Test]
        public void ConstructWithCombinedArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(combinedArgs);
            Assert.Multiple(() =>
            {
                Assert.That(attr.Arguments, Is.EqualTo(combinedArgs));
                Assert.That(attr.TypeArgs, Is.Empty);
            });
        }

        [Test]
        public void ConstructWithWeakTypedNullArgument()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(null);
            Assert.That(attr.Arguments, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(attr.Arguments[0], Is.Null);
                Assert.That(attr.TypeArgs, Is.Not.Null);
            });
        }
    }
}
