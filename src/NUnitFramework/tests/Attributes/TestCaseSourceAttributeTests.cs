// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.Attributes
{
    public class TestCaseSourceAttributeTests
    {
        private const string SourceName = "SourceName";
        private static readonly object?[] MethodParams = { 1, "two" };

        [Test]
        public void MethodParams_IsNullWhenNotProvided()
        {
            var attribute = new TestCaseSourceAttribute(SourceName);

            Assert.Multiple(() =>
            {
                Assert.That(attribute.SourceName, Is.EqualTo(SourceName));
                Assert.That(attribute.MethodParams, Is.Null);
            });
        }

        [Test]
        public void MethodParams_IsPreservedWhenProvidedWithSourceName()
        {
            var attribute = new TestCaseSourceAttribute(SourceName, MethodParams);

            Assert.Multiple(() =>
            {
                Assert.That(attribute.SourceName, Is.EqualTo(SourceName));
                Assert.That(attribute.MethodParams, Is.EqualTo(MethodParams));
            });
        }

        [Test]
        public void MethodParams_IsPreservedWhenProvidedWithSourceTypeAndSourceName()
        {
            var attribute = new TestCaseSourceAttribute(typeof(TestCaseSourceAttributeTests), SourceName, MethodParams);

            Assert.Multiple(() =>
            {
                Assert.That(attribute.SourceType, Is.EqualTo(typeof(TestCaseSourceAttributeTests)));
                Assert.That(attribute.SourceName, Is.EqualTo(SourceName));
                Assert.That(attribute.MethodParams, Is.EqualTo(MethodParams));
            });
        }
    }
}
