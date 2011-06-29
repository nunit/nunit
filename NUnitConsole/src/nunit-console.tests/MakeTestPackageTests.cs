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
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Engine;

namespace NUnit.ConsoleRunner.Tests
{
    public class MakeTestPackageTests
    {
        [Test]
        public void SingleAssembly()
        {
            var options = new ConsoleOptions("test.dll");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.AreEqual(1, package.GetAssemblies().Length);
            Assert.AreEqual(Path.GetFullPath("test.dll"), package.FilePath);
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

            Assert.AreEqual(expected, package.GetAssemblies());
        }

        [Test]
        public void WhenTimeoutIsSpecified_PackageIncludesIt()
        {
            var options = new ConsoleOptions("test.dll", "--timeout=50");
            var package = ConsoleRunner.MakeTestPackage(options);

#if CLR_2_0 || CLR_4_0
            Assert.That(package.Settings.ContainsKey("DefaultTimeout"));
#else
            Assert.That(package.Settings.Contains("DefaultTimeout"));
#endif
            Assert.AreEqual(50, package.Settings["DefaultTimeout"]);
        }

        [Test]
        public void WhenProcessModelIsSpecified_PackageIncludesIt()
        {
            var options = new ConsoleOptions("test.dll", "--process=Separate");
            var package = ConsoleRunner.MakeTestPackage(options);

#if CLR_2_0 || CLR_4_0
            Assert.That(package.Settings.ContainsKey("ProcessModel"));
#else
            Assert.That(package.Settings.Contains("ProcessModel"));
#endif
            Assert.AreEqual(ProcessModel.Separate, package.Settings["ProcessModel"]);
        }

        [Test]
        public void WhenDomainUsageIsSpecified_PackageIncludesIt()
        {
            var options = new ConsoleOptions("test.dll", "--domain=Multiple");
            var package = ConsoleRunner.MakeTestPackage(options);

#if CLR_2_0 || CLR_4_0
            Assert.That(package.Settings.ContainsKey("DomainUsage"));
#else
            Assert.That(package.Settings.Contains("DomainUsage"));
#endif
            Assert.AreEqual(DomainUsage.Multiple, package.Settings["DomainUsage"]);
        }

        [Test]
        public void WhenFrameworkIsSpecified_PackageIncludesIt()
        {
            var options = new ConsoleOptions("test.dll", "--framework=net-4.0");
            var package = ConsoleRunner.MakeTestPackage(options);

#if CLR_2_0 || CLR_4_0
            Assert.That(package.Settings.ContainsKey("RuntimeFramework"));
#else
            Assert.That(package.Settings.Contains("RuntimeFramework"));
#endif
            Assert.AreEqual("net-4.0", package.Settings["RuntimeFramework"]);
        }

        [Test]
        public void WhenConfigIsSpecified_PackageIncludesIt()
        {
            var options = new ConsoleOptions("test.dll", "--config=Release");
            var package = ConsoleRunner.MakeTestPackage(options);

#if CLR_2_0 || CLR_4_0
            Assert.That(package.Settings.ContainsKey("ActiveConfig"));
#else
            Assert.That(package.Settings.Contains("ActiveConfig"));
#endif
            Assert.AreEqual("Release", package.Settings["ActiveConfig"]);
        }

        [Test]
        public void WhenTraceIsSpecified_PackageIncludesIt()
        {
            var options = new ConsoleOptions("test.dll", "--trace=Error");
            var package = ConsoleRunner.MakeTestPackage(options);

#if CLR_2_0 || CLR_4_0
            Assert.That(package.Settings.ContainsKey("InternalTraceLevel"));
#else
            Assert.That(package.Settings.Contains("InternalTraceLevel"));
#endif
            Assert.AreEqual(InternalTraceLevel.Error, package.Settings["InternalTraceLevel"]);
        }

        [Test]
        public void EnumOptions_MayBeSpecifiedAsInts()
        {
            var options = new ConsoleOptions("test.dll", "--trace=4");
            var package = ConsoleRunner.MakeTestPackage(options);

#if CLR_2_0 || CLR_4_0
            Assert.That(package.Settings.ContainsKey("InternalTraceLevel"));
#else
            Assert.That(package.Settings.Contains("InternalTraceLevel"));
#endif
            Assert.AreEqual(InternalTraceLevel.Info, package.Settings["InternalTraceLevel"]);
        }

        [Test, ExpectedException(typeof(NDesk.Options.OptionException))]
        public void EnumOptions_InvalidNamesCauseAnException()
        {
            var options = new ConsoleOptions("test.dll", "--trace=All");
        }

        [Test]
        public void EnumOptions_OutOfRangeValuesAreUsedAsIs()
        {
            var options = new ConsoleOptions("test.dll", "--trace=7");
            var package = ConsoleRunner.MakeTestPackage(options);

#if CLR_2_0 || CLR_4_0
            Assert.That(package.Settings.ContainsKey("InternalTraceLevel"));
#else
            Assert.That(package.Settings.Contains("InternalTraceLevel"));
#endif
            Assert.AreEqual((InternalTraceLevel)7, package.Settings["InternalTraceLevel"]);
        }

        [Test]
        public void WhenNoOptionsAreSpecified_PackageContainsNoSettngs()
        {
            var options = new ConsoleOptions("test.dll");
            var package = ConsoleRunner.MakeTestPackage(options);

            Assert.AreEqual(0, package.Settings.Count);
        }
    }
}
