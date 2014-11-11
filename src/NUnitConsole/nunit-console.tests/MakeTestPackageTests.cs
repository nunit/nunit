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

using System.IO;
using NUnit.Framework;

namespace NUnit.ConsoleRunner.Tests
{
    using Options;

    public class MakeTestPackageTests
    {
        [Test]
        public void SingleAssembly()
        {
            var options = new ConsoleOptions("test.dll");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.AreEqual(1, package.TestFiles.Length);
            Assert.AreEqual(Path.GetFullPath("test.dll"), package.FullName);
        }

        [Test]
        public void MultipleAssemblies()
        {
            var names = new [] { "test1.dll", "test2.dll", "test3.dll" };
            var expected = new[] {
                Path.GetFullPath("test1.dll"),
                Path.GetFullPath("test2.dll"),
                Path.GetFullPath("test3.dll")
            };
            var options = new ConsoleOptions(names);
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.AreEqual(expected, package.TestFiles);
        }

        [Test]
        public void WhenTimeoutIsSpecified_PackageIncludesIt()
        {
            var options = new ConsoleOptions("test.dll", "--timeout=50");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.That(package.Settings.ContainsKey("DefaultTimeout"));
            Assert.AreEqual(50, package.Settings["DefaultTimeout"]);
        }

        [Test]
        public void WhenX86IsSpecified_PackageIncludesIt()
        {
            var options = new ConsoleOptions("test.dll", "--x86");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.IsTrue(package.GetSetting("RunAsX86", false));
        }

        [TestCase("Separate")]
        [TestCase("separate")]
        public void WhenProcessModelIsSpecified_PackageIncludesIt(string optionValue)
        {
            var options = new ConsoleOptions("test.dll", "--process=" + optionValue);
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.That(package.Settings.ContainsKey("ProcessModel"));
            Assert.AreEqual("Separate", package.Settings["ProcessModel"]);
        }

        [TestCase("Multiple")]
        [TestCase("multiple")]
        public void WhenDomainUsageIsSpecified_PackageIncludesIt(string optionValue)
        {
            var options = new ConsoleOptions("test.dll", "--domain=" + optionValue);
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.That(package.Settings.ContainsKey("DomainUsage"));
            Assert.AreEqual("Multiple", package.Settings["DomainUsage"]);
        }

        [Test]
        public void WhenFrameworkIsSpecified_PackageIncludesIt()
        {
            var options = new ConsoleOptions("test.dll", "--framework=net-4.0");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.That(package.Settings.ContainsKey("RuntimeFramework"));
            Assert.AreEqual("net-4.0", package.Settings["RuntimeFramework"]);
        }

        [Test]
        public void WhenConfigIsSpecified_PackageIncludesIt()
        {
            var options = new ConsoleOptions("test.dll", "--config=Release");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.That(package.Settings.ContainsKey("ActiveConfig"));
            Assert.AreEqual("Release", package.Settings["ActiveConfig"]);
        }

        [TestCase("Error")]
        [TestCase("error")]
        public void WhenTraceIsSpecified_PackageIncludesIt(string optionValue)
        {
            var options = new ConsoleOptions("test.dll", "--trace=" + optionValue);
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.That(package.Settings.ContainsKey("InternalTraceLevel"));
            Assert.AreEqual("Error", package.Settings["InternalTraceLevel"]);
        }

        [Test]
        public void WhenSeedIsSpecified_PackageIncludesIt()
        {
            var options = new ConsoleOptions("test.dll", "--seed=1234");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.That(package.Settings.ContainsKey("RandomSeed"));
            Assert.AreEqual(1234, package.Settings["RandomSeed"]);
        }

        [Test]
        public void WhenWorkersIsSpecified_PackageIncludesIt()
        {
            var options = new ConsoleOptions("test.dll", "--workers=3");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.That(package.Settings.ContainsKey("NumberOfTestWorkers"));
            Assert.AreEqual(3, package.Settings["NumberOfTestWorkers"]);
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
        public void WhenNoOptionsAreSpecified_PackageContainsNoSettings()
        {
            var options = new ConsoleOptions("test.dll");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.AreEqual(0, package.Settings.Count);
        }
    }
}
