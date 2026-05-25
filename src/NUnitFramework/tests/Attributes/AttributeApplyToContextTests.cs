// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Attributes
{
    public class AttributeApplyToContextTests
    {
        [Test]
        public void CancelAfterAttributeAppliesToContext()
        {
            var attr = new CancelAfterAttribute(250);
            var context = new TestExecutionContext();

            ((IApplyToContext)attr).ApplyToContext(context);

            Assert.That(context.TestCaseTimeout, Is.EqualTo(250));
            Assert.That(context.UseCancellation, Is.True);
        }

        [Test]
        public void SingleThreadedAttributeAppliesToContext()
        {
            var attr = new SingleThreadedAttribute();
            var context = new TestExecutionContext();

            ((IApplyToContext)attr).ApplyToContext(context);

            Assert.That(context.IsSingleThreaded, Is.True);
        }

        [Test]
        public void SetCultureAttributeAppliesToContext()
        {
            var attr = new SetCultureAttribute("fr-FR");
            var context = new TestExecutionContext();

            ((IApplyToContext)attr).ApplyToContext(context);

            Assert.That(context.CurrentCulture.Name, Is.EqualTo("fr-FR"));
        }
    }
}
