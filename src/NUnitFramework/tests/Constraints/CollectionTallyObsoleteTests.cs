// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    public class CollectionTallyObsoleteTests
    {
        [Test]
        public void CollectionTally_IsMarkedObsolete()
        {
            var collectionTallyType = typeof(Constraint).Assembly.GetType("NUnit.Framework.Constraints.CollectionTally", throwOnError: true);
            var obsoleteAttribute = (ObsoleteAttribute?)Attribute.GetCustomAttribute(collectionTallyType!, typeof(ObsoleteAttribute));

            Assert.That(obsoleteAttribute, Is.Not.Null);
            Assert.That(obsoleteAttribute!.Message, Is.EqualTo("CollectionTally is obsolete in NUnit 5.0 and will be removed in NUnit 6.0."));
            Assert.That(obsoleteAttribute.IsError, Is.False);
        }
    }
}
