// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using NUnit.Framework.Api;

namespace NUnit.Framework.Tests.Internal
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
                    Assert.That(
                        type.Namespace!.StartsWith("System", StringComparison.OrdinalIgnoreCase), Is.False,
                        $"Type {type.FullName} is publicly visible in the System namespace but should not be.");
                }
            });
        }
    }
}
