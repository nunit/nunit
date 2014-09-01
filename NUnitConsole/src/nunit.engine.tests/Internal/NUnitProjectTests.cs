// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using NUnit.Framework;

namespace NUnit.Engine.Internal.Tests
{
    [TestFixture]
    public class NUnitProjectTests
    {
        private NUnitProject project;
        private static readonly char SEP = Path.DirectorySeparatorChar;

        [SetUp]
        public void Setup()
        {
            project = new NUnitProject();
        }

        [Test]
        public void CanLoadEmptyProject()
        {
            project.LoadXml(NUnitProjectXml.EmptyProject);

            //Assert.AreEqual(Path.GetFullPath(xmlfile), project.ProjectPath);
            //Assert.AreEqual(Path.GetDirectoryName(project.ProjectPath), project.EffectiveBasePath);

            Assert.AreEqual(0, project.Configs.Count);

            Assert.AreEqual(null, project.ActiveConfig);
        }

        [Test]
        public void LoadEmptyConfigs()
        {
            project.LoadXml(NUnitProjectXml.EmptyConfigs);

            //Assert.AreEqual(Path.GetFullPath(xmlfile), project.ProjectPath);
            //Assert.AreEqual(Path.GetDirectoryName(project.ProjectPath), project.EffectiveBasePath);

            Assert.AreEqual(2, project.Configs.Count);
            Assert.AreEqual("Debug", project.ActiveConfig.Name);

            IProjectConfig config1 = project.Configs["Debug"];
            Assert.AreEqual("Debug", config1.Name);
            Assert.AreEqual(1, config1.Settings.Count);
            Assert.AreEqual(true, config1.Settings["AutoBinPath"]);

            IProjectConfig config2 = project.Configs["Release"];
            Assert.AreEqual("Release", config2.Name);
            Assert.AreEqual(1, config2.Settings.Count);
            Assert.AreEqual(true, config2.Settings["AutoBinPath"]);
        }

        [Test]
        public void LoadNormalProject()
        {
            project.LoadXml(NUnitProjectXml.NormalProject);

            //Assert.AreEqual(Path.GetFullPath(xmlfile), project.ProjectPath);
            //Assert.AreEqual(Path.GetDirectoryName(project.ProjectPath), project.EffectiveBasePath);

            Assert.AreEqual(2, project.Configs.Count);
            Assert.AreEqual("Debug", project.ActiveConfig.Name);

            IProjectConfig config1 = project.Configs["Debug"];
            Assert.AreEqual(2, config1.Assemblies.Length);
            Assert.AreEqual(
                "bin" + SEP + "debug" + SEP + "assembly1.dll",
                config1.Assemblies[0]);
            Assert.AreEqual(
                "bin" + SEP + "debug" + SEP + "assembly2.dll",
                config1.Assemblies[1]);

            Assert.AreEqual(2, config1.Settings.Count);
            Assert.AreEqual("bin" + SEP + "debug", config1.Settings["BasePath"]);
            Assert.AreEqual(true, config1.Settings["AutoBinPath"]);

            IProjectConfig config2 = project.Configs["Release"];
            Assert.AreEqual(2, config2.Assemblies.Length);
            Assert.AreEqual(
                "bin" + SEP + "release" + SEP + "assembly1.dll",
                config2.Assemblies[0]);
            Assert.AreEqual(
                "bin" + SEP + "release" + SEP + "assembly2.dll",
                config2.Assemblies[1]);

            Assert.AreEqual(2, config2.Settings.Count);
            Assert.AreEqual("bin" + SEP + "release", config2.Settings["BasePath"]);
            Assert.AreEqual(true, config2.Settings["AutoBinPath"]);
        }

        [Test]
        public void LoadProjectWithManualBinPath()
        {
            project.LoadXml(NUnitProjectXml.ManualBinPathProject);

            //Assert.AreEqual(Path.GetFullPath(xmlfile), project.ProjectPath);
            //Assert.AreEqual(Path.GetDirectoryName(project.ProjectPath), project.EffectiveBasePath);

            Assert.AreEqual(1, project.Configs.Count);

            IProjectConfig config1 = project.Configs["Debug"];
            Assert.AreEqual("Debug", config1.Name);
            Assert.AreEqual(1, config1.Settings.Count);
            Assert.AreEqual("bin_path_value", config1.Settings["PrivateBinPath"]);
        }

        [Test]
        public void LoadProjectWithComplexSettings()
        {
            project.LoadXml(NUnitProjectXml.ComplexSettingsProject);

            Assert.AreEqual(2, project.Configs.Count);

            IProjectConfig config1 = project.Configs["Debug"];
            Assert.AreEqual(5, config1.Settings.Count);
            Assert.AreEqual("bin" + SEP + "debug", config1.Settings["BasePath"]);
            Assert.AreEqual(true, config1.Settings["AutoBinPath"]);
            Assert.AreEqual("Separate", config1.Settings["ProcessModel"]);
            Assert.AreEqual("Multiple", config1.Settings["DomainUsage"]);
            Assert.AreEqual("v2.0", config1.Settings["RuntimeFramework"]);

            Assert.AreEqual(2, config1.Assemblies.Length);
            Assert.AreEqual(
                "bin" + SEP + "debug" + SEP + "assembly1.dll",
                config1.Assemblies[0]);
            Assert.AreEqual(
                "bin" + SEP + "debug" + SEP + "assembly2.dll",
                config1.Assemblies[1]);

            IProjectConfig config2 = project.Configs["Release"];
            Assert.AreEqual(5, config2.Settings.Count);
            Assert.AreEqual(true, config2.Settings["AutoBinPath"]);
            Assert.AreEqual("Separate", config2.Settings["ProcessModel"]);
            Assert.AreEqual("Multiple", config2.Settings["DomainUsage"]);
            Assert.AreEqual("v4.0", config2.Settings["RuntimeFramework"]);

            Assert.AreEqual(2, config2.Assemblies.Length);
            Assert.AreEqual(
                "bin" + SEP + "release",
                config2.Settings["BasePath"]);
            Assert.AreEqual(
                "bin" + SEP + "release" + SEP + "assembly1.dll",
                config2.Assemblies[0]);
            Assert.AreEqual(
                "bin" + SEP + "release" + SEP + "assembly2.dll",
                config2.Assemblies[1]);
        }

        //[Test]
        //public void MakeTestPackage()
        //{
        //    project.LoadXml(NUnitProjectXml.ComplexSettingsProject);
        //    IProjectConfig config = project.Configs["Release"];
        //    TestPackage package = config.MakeTestPackage();

        //    Assert.AreEqual("bin" + SEP + "release", package.Settings["BasePath"]);
        //    Assert.AreEqual(ProcessModel.Separate, package.Settings["ProcessModel"]);
        //    Assert.AreEqual(DomainUsage.Multiple, package.Settings["DomainUsage"]);

        //    RuntimeFramework framework = (RuntimeFramework)package.Settings["RuntimeFramework"];
        //    Assert.AreEqual(RuntimeType.Any, framework.Runtime);
        //    Assert.AreEqual(new Version(4, 0), framework.ClrVersion);

        //    Assert.True(package.HasSubPackages);
        //    Assert.AreEqual(2, package.SubPackages.Length);
        //    Assert.AreEqual(
        //        Path.GetFullPath(config.Assemblies[0]),
        //        package.SubPackages[0].FilePath);
        //    Assert.AreEqual(
        //        Path.GetFullPath(config.Assemblies[1]),
        //        package.SubPackages[1].FilePath);
        //}
    }
}
