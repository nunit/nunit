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

namespace NUnit.Framework.Constraints.Tests
{
    [TestFixture]
    public class ExactTypeTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new ExactTypeConstraint(typeof(D1));
            expectedDescription = string.Format("<{0}>", typeof(D1));
            stringRepresentation = string.Format("<typeof {0}>", typeof(D1));
        }

        object[] SuccessData = new object[] { new D1() };
        
        object[] FailureData = new object[] { 
            new TestCaseData( new B(), "<NUnit.Framework.Constraints.Tests.B>" ),
            new TestCaseData( new D2(), "<NUnit.Framework.Constraints.Tests.D2>" )
        };
    }

    [TestFixture]
    public class InstanceOfTypeTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new InstanceOfTypeConstraint(typeof(D1));
            expectedDescription = string.Format("instance of <{0}>", typeof(D1));
            stringRepresentation = string.Format("<instanceof {0}>", typeof(D1));
        }

        object[] SuccessData = new object[] { new D1(), new D2() };

        object[] FailureData = new object[] { 
            new TestCaseData( new B(), "<NUnit.Framework.Constraints.Tests.B>" ) 
        };
    }

    [TestFixture]
    public class AssignableFromTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new AssignableFromConstraint(typeof(D1));
            expectedDescription = string.Format("assignable from <{0}>", typeof(D1));
            stringRepresentation = string.Format("<assignablefrom {0}>", typeof(D1));
        }

        object[] SuccessData = new object[] { new D1(), new B() };
            
        object[] FailureData = new object[] { 
            new TestCaseData( new D2(), "<NUnit.Framework.Constraints.Tests.D2>" ) };
    }

    [TestFixture]
    public class AssignableToTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new AssignableToConstraint(typeof(D1));
            expectedDescription = string.Format("assignable to <{0}>", typeof(D1));
            stringRepresentation = string.Format("<assignableto {0}>", typeof(D1));
        }
        
        object[] SuccessData = new object[] { new D1(), new D2() };
            
        object[] FailureData = new object[] { 
            new TestCaseData( new B(), "<NUnit.Framework.Constraints.Tests.B>" ) };
    }

    class B { }

    class D1 : B { }

    class D2 : D1 { }

    [TestFixture]
    public class AttributeExistsConstraintTest : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new AttributeExistsConstraint(typeof(TestFixtureAttribute));
            expectedDescription = "type with attribute <NUnit.Framework.TestFixtureAttribute>";
            stringRepresentation = "<attributeexists NUnit.Framework.TestFixtureAttribute>";
        }

        object[] SuccessData = new object[] { typeof(AttributeExistsConstraintTest) };
            
        object[] FailureData = new object[] { 
            new TestCaseData( typeof(D2), "<NUnit.Framework.Constraints.Tests.D2>" ) };

        [Test, ExpectedException(typeof(System.ArgumentException))]
        public void NonAttributeThrowsException()
        {
            new AttributeExistsConstraint(typeof(string));
        }

        [Test]
        public void AttributeExistsOnMethodInfo()
        {
            Assert.That(
                GetType().GetMethod("AttributeExistsOnMethodInfo"),
                new AttributeExistsConstraint(typeof(TestAttribute)));
        }

        [Test, Description("my description")]
        public void AttributeTestPropertyValueOnMethodInfo()
        {
            Assert.That(
                GetType().GetMethod("AttributeTestPropertyValueOnMethodInfo"),
                Has.Attribute(typeof(DescriptionAttribute)).Property("Properties").Property("Keys").Contains("_DESCRIPTION"));
        }
    }
}