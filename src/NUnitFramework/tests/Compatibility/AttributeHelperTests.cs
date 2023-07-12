// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Tests.Compatibility
{
    [TestFixture]
    public class AttributeHelperTests
    {
        [Test]
        public void CanGetAttributesOnAssemblies()
        {
            var assembly = typeof(TestAttribute).Assembly;
            Assert.That(assembly, Is.Not.Null);
            var attr = AttributeHelper.GetCustomAttributes(assembly, typeof(AssemblyCompanyAttribute), true);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr, Is.Not.Empty);
        }

        [Test]
        public void CanGetAttributesOnClasses()
        {
            var type = typeof(A);
            Assert.That(type, Is.Not.Null);
            var attr = AttributeHelper.GetCustomAttributes(type, typeof(CategoryAttribute), true);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr, Has.Length.EqualTo(1));
        }

        [Test]
        public void CanGetAttributesOnDerivedClasses()
        {
            var type = typeof(B);
            Assert.That(type, Is.Not.Null);
            var attr = AttributeHelper.GetCustomAttributes(type, typeof(CategoryAttribute), true);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr, Has.Length.EqualTo(1));
        }

        [Test]
        public void DoesNotGetAttributesOnDerivedClassesWhenInheritedIsFalse()
        {
            var type = typeof(B);
            Assert.That(type, Is.Not.Null);
            var attr = AttributeHelper.GetCustomAttributes(type, typeof(CategoryAttribute), false);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr, Is.Empty);
        }

        [Test]
        public void CanGetAttributesOnMethods()
        {
            var type = typeof(A);
            Assert.That(type, Is.Not.Null);
            var method = type.GetMethod("Add");
            Assert.That(method, Is.Not.Null);
            var attr = AttributeHelper.GetCustomAttributes(method, typeof(AuthorAttribute), true);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr, Has.Length.EqualTo(1));
        }

        [Test]
        public void CanGetAttributesOnProperties()
        {
            var type = typeof(A);
            Assert.That(type, Is.Not.Null);
            var property = type.GetProperty("MyProperty");
            Assert.That(property, Is.Not.Null);
            var attr = AttributeHelper.GetCustomAttributes(property, typeof(DatapointSourceAttribute), true);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr, Has.Length.EqualTo(1));
        }

        [Test]
        public void CanGetAttributesOnFields()
        {
            var type = typeof(A);
            Assert.That(type, Is.Not.Null);
            var field = type.GetField(nameof(A.Field));
            Assert.That(field, Is.Not.Null);
            var attr = AttributeHelper.GetCustomAttributes(field, typeof(DatapointAttribute), true);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr, Has.Length.EqualTo(1));
        }

        [Test]
        public void CanGetAttributesOnParameters()
        {
            var type = typeof(A);
            Assert.That(type, Is.Not.Null);
            var method = type.GetMethod("Add");
            Assert.That(method, Is.Not.Null);
            ParameterInfo[] param = method.GetParameters();
            Assert.That(param, Is.Not.Null);
            Assert.That(param, Has.Length.EqualTo(2));
            foreach (var p in param)
            {
                var attr = AttributeHelper.GetCustomAttributes(p, typeof(RandomAttribute), true);
                Assert.That(attr, Is.Not.Null);
                Assert.That(attr, Has.Length.EqualTo(1));
            }
        }

        [Category("A Category")]
        private class A
        {
            [Author("John Doe")]
            public int Add([Random(1)]int x, [Random(1)]int y)
            {
                return x + y;
            }

            [DatapointSource]
            public int MyProperty { get; set; }

            [Datapoint]
            public int Field = 1;
        }

        private class B : A
        {
        }
    }
}
