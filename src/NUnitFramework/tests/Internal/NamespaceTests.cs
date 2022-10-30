// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class NamespaceTests
    {
        [Test]
        public void AllExportedNameSpacesAreNotSystem()
        {
            var exportedTypes = typeof(FrameworkController).Assembly.GetExportedTypes();

            var exportedTypesWhitelist = new[] 
            {
                typeof(System.Web.UI.ICallbackEventHandler)
            };

            Assert.Multiple(() =>
            {
                foreach (var type in exportedTypes.Except(exportedTypesWhitelist))
                {
                    Assert.IsFalse(
                        type.Namespace.StartsWith("System", StringComparison.OrdinalIgnoreCase),
                        $"Type {type.FullName} is publicly visible in the System namespace but should not be.");
                }
            });
        }
    }
}
