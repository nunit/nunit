// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
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

        private class Base
        {
        }

        private class Derived : Base
        {
            public int SomeProperty { get; set; } = 42;
        }

        private int[]? _array;
        private PropertyConstraint? _countPropertyConstraint;
        private PropertyExistsConstraint? _countPropertyExistsConstraint;

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
