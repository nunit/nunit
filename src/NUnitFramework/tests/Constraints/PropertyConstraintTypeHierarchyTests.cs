// ***********************************************************************
// Copyright (c) 2020 Charlie Poole
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

namespace NUnit.Framework.Constraints
{
    [TestFixture(TestOf = typeof(PropertyConstraint))]
    public class PropertyConstraintTypeHierarchyTests
    {
        [Test]
        public void PropertyDefinedInDerivedClass_ShouldExist()
        {
            var sut = new PropertyConstraint(nameof(Derived.SomeProperty), new EqualConstraint(42));

            var instance = (Base)new Derived();
            var actual = sut.ApplyTo(instance);

            Assert.That(actual, Has.Property(nameof(ConstraintResult.Status)).EqualTo(ConstraintStatus.Success));

            
            var existSut = new PropertyExistsConstraint(nameof(Derived.SomeProperty));
            var actualExist = existSut.ApplyTo(instance);
            Assert.That(actualExist.IsSuccess, Is.True);
        }

        class Base { }
        class Derived : Base
        {
            public int SomeProperty { get; set; } = 42;
        }


        private int[] _array;
        private PropertyConstraint _countPropertyConstraint;
        private PropertyExistsConstraint _countPropertyExistsConstraint;

        [SetUp]
        public void BeforeEveryTest()
        {
            _array = new[] { 1, 2, 3 };
            _countPropertyConstraint = new PropertyConstraint(nameof(IList<object>.Count), new EqualConstraint(_array.Length));
            _countPropertyExistsConstraint = new PropertyExistsConstraint(nameof(IList<object>.Count));
        }

        [Test]
        public void ExplicitlyImplementedProperty_ShouldNotExist_ViaImplementingType()
        {
            var ex = Assert.Throws<ArgumentException>(() => _countPropertyConstraint.ApplyTo(_array));

            Assert.That(ex, Has.Message.StartWith("Property Count was not found on System.Int32[]"));

            var actualExist = _countPropertyExistsConstraint.ApplyTo(_array);
            Assert.That(actualExist.IsSuccess, Is.False);
        }

        [Test]
        public void PropertyDefinedInInterface_ShouldNotExist_WhenCastToObject()
        {
            var ex = Assert.Throws<ArgumentException>(() => _countPropertyConstraint.ApplyTo((object)_array));

            Assert.That(ex, Has.Message.StartWith("Property Count was not found on System.Int32[]"));

            var actualExist = _countPropertyExistsConstraint.ApplyTo((object)_array);
            Assert.That(actualExist.IsSuccess, Is.False);
        }

        [Test]
        public void PropertyDefinedInInterface_ShouldExist_WhenCastToICollection()
        {
            var actual = _countPropertyConstraint.ApplyTo((ICollection<int>)_array);

            Assert.That(actual, Has.Property(nameof(ConstraintResult.Status)).EqualTo(ConstraintStatus.Success));

            var actualExist = _countPropertyExistsConstraint.ApplyTo((ICollection<int>)_array);
            Assert.That(actualExist.IsSuccess, Is.True);
        }

        [Test]
        public void PropertyDefinedInInterface_ShouldExist_WhenCastToIList()
        {
            var actual = _countPropertyConstraint.ApplyTo((IList<int>)_array);

            Assert.That(actual, Has.Property(nameof(ConstraintResult.Status)).EqualTo(ConstraintStatus.Success));

            var actualExist = _countPropertyExistsConstraint.ApplyTo((IList<int>)_array);
            Assert.That(actualExist.IsSuccess, Is.True);
        }
    }
}
