// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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

namespace NUnitLite.Tests
{
    public class MakeRunSettingsTests
    {
#if !NETCOREAPP1_1
        [Test]
        public void WhenTimeoutIsSpecified_RunSettingsIncludeIt()
        {
            var options = new NUnitLiteOptions("test.dll", "--timeout=50");
            var settings = TextRunner.MakeRunSettings(options);

            Assert.That(settings.ContainsKey("DefaultTimeout"));
            Assert.AreEqual(50, settings["DefaultTimeout"]);
        }
#endif

        [Test]
        public void WhenWorkDirectoryIsSpecified_RunSettingsIncludeIt()
        {
            var options = new NUnitLiteOptions("test.dll", "--work=results");
            var settings = TextRunner.MakeRunSettings(options);

            Assert.That(settings.ContainsKey("WorkDirectory"));
            Assert.AreEqual(Path.GetFullPath("results"), settings["WorkDirectory"]);
        }

        [Test]
        public void WhenSeedIsSpecified_RunSettingsIncludeIt()
        {
            var options = new NUnitLiteOptions("test.dll", "--seed=1234");
            var settings = TextRunner.MakeRunSettings(options);

            Assert.That(settings.ContainsKey("RandomSeed"));
            Assert.AreEqual(1234, settings["RandomSeed"]);
        }

        [Test]
        public void WhenWorkersIsSpecified_RunSettingsIncludeIt()
        {
            var options = new NUnitLiteOptions("test.dll", "--workers=8");
            var settings = TextRunner.MakeRunSettings(options);

            Assert.That(settings.ContainsKey("NumberOfTestWorkers"));
            Assert.AreEqual(8, settings["NumberOfTestWorkers"]);
        }
    }
}
