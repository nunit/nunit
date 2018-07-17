using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class NamespaceTests
    {
        [Test]
        public void AllExportedNameSpacesAreNotSystem()
        {
#if !NETCOREAPP1_1
            var exportedTypes = Assembly.GetAssembly(typeof(FrameworkController)).GetExportedTypes();
#else
            var exportedTypes = typeof(FrameworkController).GetTypeInfo().Assembly.GetExportedTypes();
#endif

            var exportedTypesWhitelist = new[] 
            {
                typeof(System.Web.UI.ICallbackEventHandler)
            };

            Assert.Multiple(() =>
            {
                foreach (var type in exportedTypes.Except(exportedTypesWhitelist))
                {
                    Assert.IsFalse(
                        type.Namespace.StartsWith("System", StringComparison.CurrentCultureIgnoreCase),
                        $"Type {type.FullName} is in the System namespace but should not be.");
                }
            });


        }
    }
}
