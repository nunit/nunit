// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    public static class PropertyConstraintTests
    {
        [TestCase("PublicProperty")]
        [TestCase("PrivateProperty")]
        [TestCase("PublicPropertyPrivateShadow")]
        public static void PropertyExists_ShadowingPropertyOfDifferentType(string propertyName)
        {
            var instance = new DerivedClassWithoutProperty();
            Assert.That(instance, Has.Property(propertyName));
        }

        [TestCase("PublicProperty", true)]
        [TestCase("PrivateProperty", true)]
        [TestCase("PublicPropertyPrivateShadow", false)]
        public static void PropertyValue_ShadowingPropertyOfDifferentType(string propertyName, bool shouldUseShadowingProperty)
        {
            var instance = new DerivedClassWithoutProperty();
            Assert.That(instance, Has.Property(propertyName).EqualTo(shouldUseShadowingProperty ? 2 : 1));
        }

        public class BaseClass
        {
            public object PublicProperty => 1;
            // Private members can't be shadowed
            protected object PrivateProperty => 1;
            public object PublicPropertyPrivateShadow => 1;
        }

        public class ClassWithShadowingProperty : BaseClass
        {
            public new int PublicProperty => 2;
            private new int PrivateProperty => 2;
            private new int PublicPropertyPrivateShadow => 2;
        }

        public class DerivedClassWithoutProperty : ClassWithShadowingProperty
        {
        }
    }

    public class PropertyExistsTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new PropertyExistsConstraint("Length");
            expectedDescription = "property Length";
            stringRepresentation = "<propertyexists Length>";
        }

        static object[] SuccessData = new object[] { new int[0], "hello", typeof(Array) };

        static object[] FailureData = new object[] { 
            new TestCaseData( 42, "<System.Int32>" ),
            new TestCaseData( new List<int>(), "<System.Collections.Generic.List`1[System.Int32]>" ),
            new TestCaseData( typeof(Int32), "<System.Int32>" ) };

        public void NullDataThrowsArgumentNullException()
        {
            object value = null;
            Assert.Throws<ArgumentNullException>(() => theConstraint.ApplyTo(value));
        }
    }

    public class PropertyTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new PropertyConstraint("Length", new EqualConstraint(5));
            expectedDescription = "property Length equal to 5";
            stringRepresentation = "<property Length <equal 5>>";
        }

        static object[] SuccessData = new object[] { new int[5], "hello" };

        static object[] FailureData = new object[] { 
            new TestCaseData( new int[3], "3" ),
            new TestCaseData( "goodbye", "7" ) };

        [Test]
        public void NullDataThrowsArgumentNullException()
        {
            object value = null;
            Assert.Throws<ArgumentNullException>(() => theConstraint.ApplyTo(value));
        }

        [Test]
        public void InvalidDataThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => theConstraint.ApplyTo(42));
        }

        [Test]
        public void InvalidPropertyExceptionMessageContainsTypeName()
        {
            Assert.That(() => theConstraint.ApplyTo(42),
                Throws.ArgumentException.With.Message.Contains("System.Int32"));
        }

        [Test]
        public void PropertyEqualToValueWithTolerance()
        {
            Constraint c = new EqualConstraint(105m).Within(0.1m);
            Assert.That(c.Description, Is.EqualTo("105m +/- 0.1m"));

            c = new PropertyConstraint("D", new EqualConstraint(105m).Within(0.1m));
            Assert.That(c.Description, Is.EqualTo("property D equal to 105m +/- 0.1m"));
        }

        [Test]
        public void ChainedProperties()
        {
            var inputObject = new { Foo = new { Bar = "Baz" } };

            // First test one thing at a time
            Assert.That(inputObject, Has.Property("Foo"));
            Assert.That(inputObject.Foo, Has.Property("Bar"));
            Assert.That(inputObject.Foo.Bar, Is.EqualTo("Baz"));
            Assert.That(inputObject.Foo.Bar, Has.Length.EqualTo(3));

            // Chain the tests
            Assert.That(inputObject, Has.Property("Foo").Property("Bar").EqualTo("Baz"));
            Assert.That(inputObject, Has.Property("Foo").Property("Bar").Property("Length").EqualTo(3));
            Assert.That(inputObject, Has.Property("Foo").With.Property("Bar").EqualTo("Baz"));
            Assert.That(inputObject, Has.Property("Foo").With.Property("Bar").With.Property("Length").EqualTo(3));

            // Failure message
            var c = ((IResolveConstraint)Has.Property("Foo").Property("Bar").Length.EqualTo(5)).Resolve();
            var r = c.ApplyTo(inputObject);
            Assert.That(r.Status, Is.EqualTo(ConstraintStatus.Failure));
            Assert.That(r.Description, Is.EqualTo("property Foo property Bar property Length equal to 5"));
            Assert.That(r.ActualValue, Is.EqualTo(3));
        }

        [Test]
        public void MultipleProperties()
        {
            var inputObject = new { Foo = 42, Bar = "Baz" };

            // First test one thing at a time
            Assert.That(inputObject, Has.Property("Foo"));
            Assert.That(inputObject, Has.Property("Bar"));
            Assert.That(inputObject.Foo, Is.EqualTo(42));
            Assert.That(inputObject.Bar, Is.EqualTo("Baz"));
            Assert.That(inputObject.Bar, Has.Length.EqualTo(3));

            // Combine the tests
            Assert.That(inputObject, Has.Property("Foo").And.Property("Bar").EqualTo("Baz"));
            Assert.That(inputObject, Has.Property("Foo").And.Property("Bar").With.Length.EqualTo(3));

            // Failure message
            var c = ((IResolveConstraint)Has.Property("Foo").And.Property("Bar").With.Length.EqualTo(5)).Resolve();
            var r = c.ApplyTo(inputObject);
            Assert.That(r.Status, Is.EqualTo(ConstraintStatus.Failure));
            Assert.That(r.Description, Is.EqualTo("property Foo and property Bar property Length equal to 5"));
            Assert.That(r.ActualValue, Is.EqualTo(inputObject));
        }   

        [Test]
        public void FailureMessageContainsChainedConstraintMessage()
        {
            var inputObject = new { Foo = new List<int> { 2, 3, 5, 7 } };

            //Property Constraint Message with chained Equivalent Constraint.
            var constraint = ((IResolveConstraint)Has.Property("Foo").EquivalentTo(new List<int> { 2, 3, 5, 8 })).Resolve();
            
            //Apply the constraint and write message.
            var result = constraint.ApplyTo(inputObject);
            var textMessageWriter = new TextMessageWriter();
            result.WriteMessageTo(textMessageWriter);

            //Verify message contains "Equivalent Constraint" message too.
            Assert.That(result.Status, Is.EqualTo(ConstraintStatus.Failure));
            Assert.That(result.GetType().Name, Is.EqualTo("PropertyConstraintResult"));
            Assert.IsTrue(textMessageWriter.ToString().Contains("Missing"));
            Assert.IsTrue(textMessageWriter.ToString().Contains("Extra"));
        }
    }
}
