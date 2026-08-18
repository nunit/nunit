// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture, NonParallelizable]
    public partial class DependsOnAttributeTests
    {
        [Test]
        public void AttributeUsageIsClassLevelSingleUse()
        {
            var usage = (AttributeUsageAttribute?)Attribute.GetCustomAttribute(typeof(DependsOnAttribute), typeof(AttributeUsageAttribute));

            Assert.That(usage, Is.Not.Null, "DependsOnAttribute should have an AttributeUsage attribute.");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(usage.ValidOn, Is.EqualTo(AttributeTargets.Class | AttributeTargets.Method));
                Assert.That(usage.AllowMultiple, Is.False);
                Assert.That(usage.Inherited, Is.False);
            }
        }
    }
}
