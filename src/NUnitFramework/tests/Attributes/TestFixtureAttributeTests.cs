// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Tests.Attributes
{
    public class TestFixtureAttributeTests
    {
        private static readonly object[] FixtureArgs = { 10, 20, "Charlie" };
        private static readonly Type[] TypeArgs = { typeof(int), typeof(string) };
        private static readonly object[] CombinedArgs = { typeof(int), typeof(string), 10, 20, "Charlie" };

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
            TestFixtureAttribute attr = new TestFixtureAttribute(FixtureArgs);
            Assert.Multiple(() =>
            {
                Assert.That(attr.Arguments, Is.EqualTo(FixtureArgs));
                Assert.That(attr.TypeArgs, Is.Empty);
            });
        }

        [Test]
        public void ConstructWithJustTypeArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(TypeArgs);
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
            attr.TypeArgs = TypeArgs;
            Assert.Multiple(() =>
            {
                Assert.That(attr.Arguments, Is.Empty);
                Assert.That(attr.TypeArgs, Is.EqualTo(TypeArgs));
            });
        }

        [Test]
        public void ConstructWithFixtureArgsAndSetTypeArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(FixtureArgs);
            attr.TypeArgs = TypeArgs;
            Assert.Multiple(() =>
            {
                Assert.That(attr.Arguments, Is.EqualTo(FixtureArgs));
                Assert.That(attr.TypeArgs, Is.EqualTo(TypeArgs));
            });
        }

        [Test]
        public void ConstructWithCombinedArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(CombinedArgs);
            Assert.Multiple(() =>
            {
                Assert.That(attr.Arguments, Is.EqualTo(CombinedArgs));
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
