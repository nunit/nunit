// ***********************************************************************
// Copyright (c) 2011-2015 Charlie Poole
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

using System.IO;
using NUnit.Common;
using NUnit.Framework;

namespace NUnit.ConsoleRunner.Tests
{
    public class MakeTestPackageTests
    {
        [Test]
        public void SingleAssembly()
        {
            var options = new ConsoleOptions("test.dll");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.AreEqual(1, package.SubPackages.Count);
            Assert.AreEqual(Path.GetFullPath("test.dll"), package.SubPackages[0].FullName);
        }

        [Test]
        public void MultipleAssemblies()
        {
            var names = new [] { "test1.dll", "test2.dll", "test3.dll" };
            var options = new ConsoleOptions(names);
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.AreEqual(3, package.SubPackages.Count);
            Assert.AreEqual(Path.GetFullPath("test1.dll"), package.SubPackages[0].FullName);
            Assert.AreEqual(Path.GetFullPath("test2.dll"), package.SubPackages[1].FullName);
            Assert.AreEqual(Path.GetFullPath("test3.dll"), package.SubPackages[2].FullName);
        }

        [TestCase("--timeout=50", "DefaultTimeout", 50)]
        [TestCase("--x86", "RunAsX86", true)]
        [TestCase("--dispose-runners", "DisposeRunners", true)]
        [TestCase("--shadowcopy", "ShadowCopyFiles", true)]
        [TestCase("--process=Separate", "ProcessModel", "Separate")]
        [TestCase("--process=separate", "ProcessModel", "Separate")]
        [TestCase("--process=Single", "ProcessModel", "InProcess")]
        [TestCase("--process=InProcess", "ProcessModel", "InProcess")]
        [TestCase("--inprocess", "ProcessModel", "InProcess")]
        [TestCase("--domain=Multiple", "DomainUsage", "Multiple")]
        [TestCase("--domain=multiple", "DomainUsage", "Multiple")]
        [TestCase("--framework=net-4.0", "RuntimeFramework", "net-4.0")]
        [TestCase("--config=Release", "ActiveConfig", "Release")]
        [TestCase("--trace=Error", "InternalTraceLevel", "Error")]
        [TestCase("--trace=error", "InternalTraceLevel", "Error")]
        [TestCase("--seed=1234", "RandomSeed", 1234)]
        [TestCase("--agents=5", "MaxAgents", 5)]
        [TestCase("--workers=3", "NumberOfTestWorkers", 3)]
        [TestCase("--workers=0", "NumberOfTestWorkers", 0)]
        [TestCase("--debug", "DebugTests", true)]
        [TestCase("--pause", "PauseBeforeRun", true)]
        [TestCase("--params:X=5;Y=7", "TestParameters", "X=5;Y=7")]
#if DEBUG
        [TestCase("--debug-agent", "DebugAgent", true)]
#endif
        public void WhenOptionIsSpecified_PackageIncludesSetting(string option, string key, object val)
        {
            var options = new ConsoleOptions("test.dll", option);
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.That(package.Settings.ContainsKey(key), "Setting not included for {0}", option);
            Assert.AreEqual(val, package.Settings[key], "NumberOfTestWorkers not set correctly for {0}", option);
        }

        [Test]
        public void WhenDebugging_NumberOfTestWorkersDefaultsToZero()
        {
            var options = new ConsoleOptions("test.dll", "--debug");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.That(package.Settings["DebugTests"], Is.EqualTo(true));
            Assert.That(package.Settings["NumberOfTestWorkers"], Is.EqualTo(0));
        }

        [Test]
        public void WhenDebugging_NumberOfTestWorkersMayBeOverridden()
        {
            var options = new ConsoleOptions("test.dll", "--debug", "--workers=3");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.That(package.Settings["DebugTests"], Is.EqualTo(true));
            Assert.That(package.Settings["NumberOfTestWorkers"], Is.EqualTo(3));
        }

        //[Test]
        //public void EnumOptions_MayBeSpecifiedAsInts()
        //{
        //    var options = new ConsoleOptions("test.dll", "--trace=4");
        //    var package = ConsoleRunner.MakeTestPackage(options);

        //    Assert.That(package.Settings.ContainsKey("InternalTraceLevel"));
        //    Assert.AreEqual("Info", package.Settings["InternalTraceLevel"]);
        //}

        //[Test]
        //public void EnumOptions_InvalidNamesCauseAnError()
        //{
        //    var options = new ConsoleOptions("test.dll", "--trace=All");
        //    Assert.False(options.Validate());
        //}

        //[Test]
        //public void EnumOptions_OutOfRangeValuesAreUsedAsIs()
        //{
        //    var options = new ConsoleOptions("test.dll", "--trace=7");
        //    var package = ConsoleRunner.MakeTestPackage(options);

        //    Assert.That(package.Settings.ContainsKey("InternalTraceLevel"));
        //    Assert.AreEqual(7, package.Settings["InternalTraceLevel"]);
        //}

        [Test]
        public void WhenNoOptionsAreSpecified_PackageContainsWorkDirectorySettingOnly()
        {
            var options = new ConsoleOptions("test.dll");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.AreEqual(1, package.Settings.Count);
            Assert.That(package.Settings.Keys, Contains.Item("WorkDirectory"));
        }
    }
}
