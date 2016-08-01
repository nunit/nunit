// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
            var assembly = typeof(AttributeHelperTests).GetTypeInfo().Assembly;
            Assert.That(assembly, Is.Not.Null);
            var attr = AttributeHelper.GetCustomAttributes(assembly, typeof(AssemblyCompanyAttribute), true);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr.Length, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void CanGetAttributesOnClasses()
        {
            var type = typeof(A);
            Assert.That(type, Is.Not.Null);
            var attr = AttributeHelper.GetCustomAttributes(type, typeof(CategoryAttribute), true);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr.Length, Is.EqualTo(1));
        }

        [Test]
        public void CanGetAttributesOnDerivedClasses()
        {
            var type = typeof(B);
            Assert.That(type, Is.Not.Null);
            var attr = AttributeHelper.GetCustomAttributes(type, typeof(CategoryAttribute), true);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr.Length, Is.EqualTo(1));
        }

        [Test]
        public void DoesNotGetAttributesOnDerivedClassesWhenInheritedIsFalse()
        {
            var type = typeof(B);
            Assert.That(type, Is.Not.Null);
            var attr = AttributeHelper.GetCustomAttributes(type, typeof(CategoryAttribute), false);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr.Length, Is.EqualTo(0));
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
            Assert.That(attr.Length, Is.EqualTo(1));
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
            Assert.That(attr.Length, Is.EqualTo(1));
        }

        [Test]
        public void CanGetAttributesOnFields()
        {
            var type = typeof(A);
            Assert.That(type, Is.Not.Null);
            var field = type.GetField("field");
            Assert.That(field, Is.Not.Null);
            var attr = AttributeHelper.GetCustomAttributes(field, typeof(DatapointAttribute), true);
            Assert.That(attr, Is.Not.Null);
            Assert.That(attr.Length, Is.EqualTo(1));
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
            Assert.That(param.Length, Is.EqualTo(2));
            foreach (var p in param)
            {
                var attr = AttributeHelper.GetCustomAttributes(p, typeof(RandomAttribute), true);
                Assert.That(attr, Is.Not.Null);
                Assert.That(attr.Length, Is.EqualTo(1));
            }
        }

        [Category("A Category")]
        class A
        {
            [Author("John Doe")]
            public int Add([Random(1)]int x, [Random(1)]int y)
            {
                return x + y;
            }

            [DatapointSource]
            public int MyProperty { get; set; }

            [Datapoint]
            public int field = 1;
        }

        class B : A
        {
        }
    }
}
