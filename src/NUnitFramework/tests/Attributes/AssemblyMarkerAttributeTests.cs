// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Reflection;

namespace NUnit.Framework.Tests.Attributes
{
    public class AssemblyMarkerAttributeTests
    {
        [Test]
        public void NonTestAssemblyAttribute_CanBeAppliedToAssembly()
        {
            var attributeUsage = typeof(NonTestAssemblyAttribute).GetCustomAttribute<AttributeUsageAttribute>();

            Assert.Multiple(() =>
            {
                Assert.That(new NonTestAssemblyAttribute(), Is.InstanceOf<NUnitAttribute>());
                Assert.That(attributeUsage, Is.Not.Null);
                Assert.That(attributeUsage!.ValidOn, Is.EqualTo(AttributeTargets.Assembly));
                Assert.That(attributeUsage.AllowMultiple, Is.False);
                Assert.That(attributeUsage.Inherited, Is.False);
            });
        }

        [Test]
        public void TestAssemblyDirectoryResolveAttribute_CanBeAppliedToAssembly()
        {
            var attributeUsage = typeof(TestAssemblyDirectoryResolveAttribute).GetCustomAttribute<AttributeUsageAttribute>();

            Assert.Multiple(() =>
            {
                Assert.That(new TestAssemblyDirectoryResolveAttribute(), Is.InstanceOf<NUnitAttribute>());
                Assert.That(attributeUsage, Is.Not.Null);
                Assert.That(attributeUsage!.ValidOn, Is.EqualTo(AttributeTargets.Assembly));
                Assert.That(attributeUsage.AllowMultiple, Is.False);
                Assert.That(attributeUsage.Inherited, Is.False);
            });
        }
    }
}
