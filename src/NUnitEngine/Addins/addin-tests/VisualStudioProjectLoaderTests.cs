// ***********************************************************************
// Copyright (c) 2008-2014 Charlie Poole
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
using System.IO;
using NUnit.Engine.Extensibility;
using NUnit.Engine.Tests.resources;
using NUnit.Framework;

namespace NUnit.Engine.Services.ProjectLoaders.Tests
{
    [TestFixture]
    public class VisualStudioProjectLoaderTests
    {
        private VisualStudioProjectLoader _loader;

        [SetUp]
        public void CreateLoader()
        {
            _loader = new VisualStudioProjectLoader();
        }

        [TestCase("project.csproj", true)]
        [TestCase("project.vbproj", true)]
        [TestCase("project.vjsproj", true)]
        [TestCase("project.fsproj", true)]
        [TestCase("project.vcproj", true)]
        [TestCase("project.sln", true)]
        [TestCase("project.xyproj", false)]
        public void ValidExtensions(string project, bool isGood)
        {
            if (isGood)
                Assert.That(_loader.CanLoadFrom(project), "Should be loadable: {0}", project);
            else
                Assert.False(_loader.CanLoadFrom(project), "Should not be loadable: {0}", project);
        }

        [Test]
        public void CheckExtensionAttribute()
        {
            Assert.That(typeof(VisualStudioProjectLoader),
                Has.Attribute<ExtensionAttribute>());
        }

        // Note for review:
        // The following test doesn't pass because AttributeConstraint always uses the
        // first attribute it finds. Should we fix the syntax or just document it?
        //[TestCase(".sln")]
        //[TestCase(".csproj")]
        //[TestCase(".vbproj")]
        //[TestCase(".vjsproj")]
        //[TestCase(".vcproj")]
        //[TestCase(".fsproj")]
        //public void CheckExtensionPropertyAttributes(string ext)
        //{
        //    Assert.That(typeof(VisualStudioProjectLoader),
        //        Has.Attribute<ExtensionPropertyAttribute>()
        //            .With.Property("Name").EqualTo("FileExtension").And.Property("Value").EqualTo(ext));
        //}

        [TestCase(".sln")]
        [TestCase(".csproj")]
        [TestCase(".vbproj")]
        [TestCase(".vjsproj")]
        [TestCase(".vcproj")]
        [TestCase(".fsproj")]
        public void CheckExtensionPropertyAttributes(string ext)
        {
            var attrs = typeof(VisualStudioProjectLoader).GetCustomAttributes(typeof(ExtensionPropertyAttribute), false);

            Assert.That(attrs, 
                Has.Exactly(1)
                    .With.Property("Name").EqualTo("FileExtension")
                    .And.Property("Value").EqualTo(ext));
        }

        [Test]
        public void CannotLoadWebProject()
        {
            Assert.IsFalse(_loader.CanLoadFrom(@"http://localhost/web.csproj"));
            Assert.IsFalse(_loader.CanLoadFrom(@"\MyProject\http://localhost/web.csproj"));
        }

        [TestCase("csharp-sample.csproj", new string[] { "Debug", "Release" }, "csharp-sample")]
        [TestCase("csharp-sample.csproj", new string[] { "Debug", "Release" }, "csharp-sample")]
        [TestCase("csharp-missing-output-path.csproj", new string[] { "Debug", "Release" }, "MissingOutputPath")]
        [TestCase("csharp-xna-project.csproj", new string[] { "Debug|x86", "Release|x86" }, "XNAWindowsProject")]
        [TestCase("vb-sample.vbproj", new string[] { "Debug", "Release" }, "vb-sample")]
        [TestCase("jsharp-sample.vjsproj", new string[] { "Debug", "Release" }, "jsharp-sample")]
        [TestCase("fsharp-sample.fsproj", new string[] { "Debug", "Release" }, "fsharp-sample")]
        [TestCase("cpp-sample.vcproj", new string[] { "Debug|Win32", "Release|Win32" }, "cpp-sample")]
        [TestCase("cpp-default-library.vcproj", new string[] { "Debug|Win32", "Release|Win32" }, "cpp-default-library")]
        [TestCase("legacy-csharp-sample.csproj", new string[] { "Debug", "Release" }, "csharp-sample")]
        [TestCase("legacy-csharp-hebrew-file-problem.csproj", new string[] { "Debug", "Release" }, "HebrewFileProblem")]
        [TestCase("legacy-vb-sample.vbproj", new string[] { "Debug", "Release" }, "vb-sample")]
        [TestCase("legacy-jsharp-sample.vjsproj", new string[] { "Debug", "Release" }, "jsharp-sample")]
        [TestCase("legacy-cpp-sample.vcproj", new string[] { "Debug|Win32", "Release|Win32" }, "cpp-sample")]
        [TestCase("legacy-cpp-library-with-macros.vcproj", new string[] { "Debug|Win32", "Release|Win32" }, "legacy-cpp-library-with-macros")]
        [TestCase("legacy-cpp-makefile-project.vcproj", new string[] { "Debug|Win32", "Release|Win32" }, "MakeFileProject")]
        public void CanLoadVsProject(string resourceName, string[] configs, string assemblyName)
        {
            Assert.That(_loader.CanLoadFrom(resourceName));

            using (TestResource file = new TestResource(resourceName))
            {
                IProject project = _loader.LoadFrom(file.Path);

                Assert.That(project.ConfigNames, Is.EqualTo(configs));

                foreach (var config in configs)
                {
                    TestPackage package = project.GetTestPackage(config);

                    Assert.AreEqual(resourceName, package.Name);
                    Assert.AreEqual(1, package.SubPackages.Count);
                    Assert.AreEqual(assemblyName, Path.GetFileNameWithoutExtension(package.SubPackages[0].FullName));
                    Assert.That(package.Settings.ContainsKey("BasePath"));
                    Assert.That(Path.GetDirectoryName(package.SubPackages[0].FullName), Is.SamePath((string)package.Settings["BasePath"]));
                }
            }
        }

        [Test]
        public void FromVSSolution2003()
        {
            using (new TestResource("legacy-csharp-sample.csproj", @"csharp\legacy-csharp-sample.csproj"))
            using (new TestResource("legacy-jsharp-sample.vjsproj", @"jsharp\legacy-jsharp-sample.vjsproj"))
            using (new TestResource("legacy-vb-sample.vbproj", @"vb\legacy-vb-sample.vbproj"))
            using (new TestResource("legacy-cpp-sample.vcproj", @"cpp-sample\legacy-cpp-sample.vcproj"))
            using (TestResource file = new TestResource("legacy-samples.sln"))
            {
                IProject project = _loader.LoadFrom(file.Path);

                Assert.AreEqual(2, project.ConfigNames.Count);
                Assert.AreEqual(4, project.GetTestPackage("Debug").SubPackages.Count);
                Assert.AreEqual(4, project.GetTestPackage("Release").SubPackages.Count);
            }
        }

        [Test]
        public void FromVSSolution2005()
        {
            using (new TestResource("csharp-sample.csproj", @"csharp\csharp-sample.csproj"))
            using (new TestResource("jsharp-sample.vjsproj", @"jsharp\jsharp-sample.vjsproj"))
            using (new TestResource("vb-sample.vbproj", @"vb\vb-sample.vbproj"))
            using (new TestResource("cpp-sample.vcproj", @"cpp-sample\cpp-sample.vcproj"))
            using (TestResource file = new TestResource("samples.sln"))
            {
                IProject project = _loader.LoadFrom(file.Path);

                Assert.AreEqual(2, project.ConfigNames.Count);
                Assert.AreEqual(4, project.GetTestPackage("Debug").SubPackages.Count);
                Assert.AreEqual(4, project.GetTestPackage("Release").SubPackages.Count);
            }
        }

        [Test]
        public void FromWebApplication()
        {
            using (new TestResource("legacy-csharp-sample.csproj", @"legacy-csharp-sample\legacy-csharp-sample.csproj"))
            using (TestResource file = new TestResource("solution-with-web-application.sln"))
            {
                IProject project = _loader.LoadFrom(file.Path);
                Assert.AreEqual(2, project.ConfigNames.Count);
                Assert.AreEqual(1, project.GetTestPackage("Debug").SubPackages.Count);
                Assert.AreEqual(1, project.GetTestPackage("Release").SubPackages.Count);
            }
        }

        [Test]
        public void WithUnmanagedCpp()
        {
            using (new TestResource("legacy-csharp-sample.csproj", @"legacy-csharp-sample\legacy-csharp-sample.csproj"))
            using (new TestResource("legacy-cpp-unmanaged.vcproj", @"legacy-cpp-unmanaged\legacy-cpp-unmanaged.vcproj"))
            using (TestResource file = new TestResource("solution-with-unmanaged-cpp.sln"))
            {
                IProject project = _loader.LoadFrom(file.Path);

                Assert.AreEqual(2, project.ConfigNames.Count);
                Assert.AreEqual(2, project.GetTestPackage("Debug").SubPackages.Count);
                Assert.AreEqual(2, project.GetTestPackage("Release").SubPackages.Count);
            }
        }

        [Test]
        public void FromSolutionWithDisabledProject()
        {
            using (new TestResource("csharp-sample.csproj", @"csharp-sample\csharp-sample.csproj"))
            using (new TestResource("csharp-debug-only.csproj", @"csharp-debug-only\csharp-debug-only.csproj"))
            using (TestResource file = new TestResource("solution-with-disabled-project.sln"))
            {
                IProject project = _loader.LoadFrom(file.Path);
                Assert.AreEqual(2, project.ConfigNames.Count);
                Assert.AreEqual(2, project.GetTestPackage("Release").SubPackages.Count, "Release should have 2 assemblies");
                Assert.AreEqual(1, project.GetTestPackage("Debug").SubPackages.Count, "Debug should have 1 assembly");
            }
        }

        [Test]
        public void FromSolutionWithNonNunitTestProject()
        {
            using (new TestResource("csharp-sample.csproj", @"csharp-sample\csharp-sample.csproj"))
            using (new TestResource("csharp-debug-only-no-nunit.csproj", @"csharp-debug-only-no-nunit\csharp-debug-only-no-nunit.csproj"))
            using (TestResource file = new TestResource("solution-with-non-nunit-project.sln"))
            {
                IProject project = _loader.LoadFrom(file.Path);
                Assert.AreEqual(2, project.ConfigNames.Count);
                Assert.AreEqual(1, project.GetTestPackage("Release").SubPackages.Count, "Release should have 2 assemblies");
                Assert.AreEqual(1, project.GetTestPackage("Debug").SubPackages.Count, "Debug should have 1 assembly");
            }
        }
    }
}
