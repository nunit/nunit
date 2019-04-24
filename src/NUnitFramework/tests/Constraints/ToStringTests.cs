// ***********************************************************************
// Copyright (c) 2010 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Constraints
{
    public class ToStringTests
    {
        [Test]
        public void CanDisplaySimpleConstraints_Unresolved()
        {
            Assert.That(Is.EqualTo(5).ToString(), Is.EqualTo("<equal 5>"));
            Assert.That(Has.Property("X").ToString(), Is.EqualTo("<propertyexists X>"));
            Assert.That(Has.Attribute(typeof(TestAttribute)).ToString(), 
                Is.EqualTo("<attributeexists NUnit.Framework.TestAttribute>"));
        }

        [Test]
        public void CanDisplaySimpleConstraints_Resolved()
        {
            IResolveConstraint constraint = Is.EqualTo(5);
            Assert.That(constraint.Resolve().ToString(), Is.EqualTo("<equal 5>"));
            constraint = Has.Property("X");
            Assert.That(constraint.Resolve().ToString(), Is.EqualTo("<propertyexists X>"));
            constraint = Has.Attribute(typeof(TestAttribute)).With.Property("Description").EqualTo("smoke");
            Assert.That(constraint.Resolve().ToString(),
                Is.EqualTo("<attribute NUnit.Framework.TestAttribute <property Description <equal \"smoke\">>>"));
        }

        [Test]
        public void DisplayPrefixConstraints_Unresolved()
        {
            Assert.That(Is.Not.EqualTo(5).ToString(), Is.EqualTo("<unresolved <equal 5>>"));
            Assert.That(Is.Not.All.EqualTo(5).ToString(), Is.EqualTo("<unresolved <equal 5>>"));
            Assert.That(Has.Property("X").EqualTo(5).ToString(), Is.EqualTo("<unresolved <equal 5>>"));
            Assert.That(Has.Attribute(typeof(TestAttribute)).With.Property("Description").EqualTo("smoke").ToString(),
                Is.EqualTo("<unresolved <equal \"smoke\">>"));
        }

        [Test]
        public void CanDisplayPrefixConstraints_Resolved()
        {
            IResolveConstraint constraint = Is.Not.EqualTo(5);
            Assert.That(constraint.Resolve().ToString(), Is.EqualTo("<not <equal 5>>"));
            constraint = Is.Not.All.EqualTo(5);
            Assert.That(constraint.Resolve().ToString(), Is.EqualTo("<not <all <equal 5>>>"));
            constraint = Has.Property("X").EqualTo(5);
            Assert.That(constraint.Resolve().ToString(), Is.EqualTo("<property X <equal 5>>"));
        }

        [Test]
        public void DisplayBinaryConstraints_Resolved()
        {
            IResolveConstraint constraint = Is.GreaterThan(0).And.LessThan(100);
            Assert.That(constraint.Resolve().ToString(), Is.EqualTo("<and <greaterthan 0> <lessthan 100>>"));
        }

        [Test]
        public void DisplayBinaryConstraints_UnResolved()
        {
            IResolveConstraint constraint = Is.GreaterThan(0).And.LessThan(100);
            Assert.That(constraint.ToString(), Is.EqualTo("<unresolved <lessthan 100>>"));
        }
    }
}
