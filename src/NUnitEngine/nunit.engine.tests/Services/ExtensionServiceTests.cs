// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Services.Tests
{
    public class ExtensionServiceTests
    {
        private ExtensionService _serviceClass;
        private IExtensionService _serviceInterface;


        // NOTE: Some of these tests depend on certain extensions being built
        // at the same time as NUnit and therefore being present in the addins
        // directory. As extensions are moved out to separate builds, we will
        // need to modify those tests.

#pragma warning disable 414
        private static readonly string[] KNOWN_EXTENSION_POINT_PATHS = new string[] {
            "/NUnit/Engine/TypeExtensions/IDriverFactory",
            "/NUnit/Engine/TypeExtensions/IProjectLoader", 
            "/NUnit/Engine/TypeExtensions/IResultWriter",
            "/NUnit/Engine/TypeExtensions/ITestEventListener",
            "/NUnit/Engine/TypeExtensions/IService",
            "/NUnit/Engine/NUnitV2Driver"
        };

        private static readonly Type[] KNOWN_EXTENSION_POINT_TYPES = new Type[] {
            typeof(IDriverFactory),
            typeof(IProjectLoader),
            typeof(IResultWriter),
            typeof(ITestEventListener),
            typeof(IService),
            typeof(IFrameworkDriver)
        };

        private static readonly int[] KNOWN_EXTENSION_POINT_COUNTS = new int[] { 1, 1, 1, 2, 1, 1 };
#pragma warning restore 414

        [SetUp]
        public void CreateService()
        {
            _serviceInterface = _serviceClass = new ExtensionService();

            // Rather than actually starting the service, which would result
            // in finding the extensions actually in use on the current system,
            // we simulate the start using this assemblies dummy extensions.
            _serviceClass.FindExtensionPoints(typeof(TestEngine).Assembly);
            _serviceClass.FindExtensionPoints(typeof(ITestEngine).Assembly);

            _serviceClass.FindExtensionsInAssembly(new ExtensionAssembly(GetType().Assembly.Location, true));
        }

        [TestCaseSource("KNOWN_EXTENSION_POINT_PATHS")]
        public void CanListExtensionPoints(string path)
        {
            foreach (var ep in _serviceInterface.ExtensionPoints)
                if (ep.Path == path)
                    return;

            Assert.Fail("Couldn't find known ExtensionPoint {0}", path);
        }

        [Test]
        public void AllExtensionPointsAreKnown()
        {
            foreach (var ep in _serviceInterface.ExtensionPoints)
            {
                var known = false;
                foreach (var path in KNOWN_EXTENSION_POINT_PATHS)
                    if (path == ep.Path)
                    {
                        known = true;
                        break;
                    }
                if (!known)
                    Assert.Fail("Unknown ExtensionPoint {0}", ep.Path);
            }
        }

        [Test, Sequential]
        public void CanGetExtensionPointByPath(
            [ValueSource("KNOWN_EXTENSION_POINT_PATHS")] string path,
            [ValueSource("KNOWN_EXTENSION_POINT_TYPES")] Type type)
        {
            var ep = _serviceInterface.GetExtensionPoint(path);
            Assert.NotNull(ep);
            Assert.That(ep.Path, Is.EqualTo(path));
            Assert.That(ep.TypeName, Is.EqualTo(type.FullName));
        }

        [Test, Sequential]
        public void CanGetExtensionPointByType(
            [ValueSource("KNOWN_EXTENSION_POINT_PATHS")] string path,
            [ValueSource("KNOWN_EXTENSION_POINT_TYPES")] Type type)
        {
            var ep = _serviceClass.GetExtensionPoint(type);
            Assert.NotNull(ep);
            Assert.That(ep.Path, Is.EqualTo(path));
            Assert.That(ep.TypeName, Is.EqualTo(type.FullName));
        }

#pragma warning disable 414
        private static readonly string[] KNOWN_EXTENSIONS = new string[] {
            "NUnit.Engine.Tests.DummyFrameworkDriverExtension",
            "NUnit.Engine.Tests.DummyProjectLoaderExtension",
            "NUnit.Engine.Tests.DummyResultWriterExtension",
            "NUnit.Engine.Tests.DummyEventListenerExtension",
            "NUnit.Engine.Tests.DummyServiceExtension",
            "NUnit.Engine.Tests.V2DriverExtension"
        };
#pragma warning restore 414

        [TestCaseSource("KNOWN_EXTENSIONS")]
        public void CanListExtensions(string typeName)
        {
            foreach (ExtensionNode node in _serviceClass.Extensions)
                if (node.TypeName == typeName)
                {
                    Assert.True(node.Enabled);
                    return;
                }
            
            Assert.Fail("Couldn't find known Extension {0}", typeName);
        }

        [Test, Sequential]
        public void ExtensionsAreAddedToExtensionPoint(
            [ValueSource("KNOWN_EXTENSION_POINT_PATHS")] string path,
            [ValueSource("KNOWN_EXTENSION_POINT_COUNTS")] int extensionCount)
        {
            var ep = _serviceClass.GetExtensionPoint(path);
            Assume.That(ep, Is.Not.Null);

            Assert.That(ep.Extensions.Count, Is.EqualTo(extensionCount));
        }

        [Test]
        public void ExtensionMayBeDisabledByDefault()
        {
            foreach (ExtensionNode node in _serviceInterface.Extensions)
                if (node.TypeName == "NUnit.Engine.Tests.DummyDisabledExtension")
                {
                    Assert.False(node.Enabled, "Should be disabled by default");
                    return;
                }

            Assert.Fail("Could not find DummyDisabledExtension");
        }

        [Test]
        public void DisabledExtensionMayBeEnabled()
        {
            _serviceInterface.EnableExtension("NUnit.Engine.Tests.DummyDisabledExtension", true);
            foreach (var node in _serviceInterface.Extensions)
                if (node.TypeName == "NUnit.Engine.Tests.DummyDisabledExtension")
                {
                    Assert.True(node.Enabled, "Failed to enable extension");
                    return;
                }

            Assert.Fail("Could not find DummyDisabledExtension");
        }
    }
}
