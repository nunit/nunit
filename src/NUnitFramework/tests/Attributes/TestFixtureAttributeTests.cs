// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
            Assert.That(attr.Arguments.Length == 0);
            Assert.That(attr.TypeArgs.Length == 0);
        }

        [Test]
        public void ConstructWithFixtureArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(fixtureArgs);
            Assert.That(attr.Arguments, Is.EqualTo( fixtureArgs ) );
            Assert.That(attr.TypeArgs.Length == 0 );
        }

        [Test]
        public void ConstructWithJustTypeArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(typeArgs);
            Assert.That(attr.Arguments.Length == 2);
            Assert.That(attr.TypeArgs.Length == 0);
        }

        [Test]
        public void ConstructWithNoArgumentsAndSetTypeArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute();
            attr.TypeArgs = typeArgs;
            Assert.That(attr.Arguments.Length == 0);
            Assert.That(attr.TypeArgs, Is.EqualTo(typeArgs));
        }

        [Test]
        public void ConstructWithFixtureArgsAndSetTypeArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(fixtureArgs);
            attr.TypeArgs = typeArgs;
            Assert.That(attr.Arguments, Is.EqualTo(fixtureArgs));
            Assert.That(attr.TypeArgs, Is.EqualTo(typeArgs));
        }

        [Test]
        public void ConstructWithCombinedArgs()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(combinedArgs);
            Assert.That(attr.Arguments, Is.EqualTo(combinedArgs));
            Assert.That(attr.TypeArgs.Length, Is.EqualTo(0));
        }

        [Test]
        public void ConstructWithWeakTypedNullArgument()
        {
            TestFixtureAttribute attr = new TestFixtureAttribute(null);
            Assert.That(attr.Arguments, Is.Not.Null);
            Assert.That(attr.Arguments[0], Is.Null);
            Assert.That(attr.TypeArgs, Is.Not.Null);
        }
    }
}
