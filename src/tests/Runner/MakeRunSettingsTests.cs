using System;
using System.IO;
using NUnit.Framework;

namespace NUnitLite.Runner.Tests
{
    public class MakeRunSettingsTests
    {
        [Test]
        public void WhenTimeoutIsSpecified_RunSettingsIncludeIt()
        {
            var options = new CommandLineOptions("test.dll", "--timeout=50");
            var settings = TextUI.MakeRunSettings(options);

            Assert.That(settings.ContainsKey("DefaultTimeout"));
            Assert.AreEqual(50, settings["DefaultTimeout"]);
        }

        [Test]
        public void WhenWorkDirectoryIsSpecified_RunSettingsIncludeIt()
        {
            var options = new CommandLineOptions("test.dll", "--work=results");
            var settings = TextUI.MakeRunSettings(options);

            Assert.That(settings.ContainsKey("WorkDirectory"));
            Assert.AreEqual(Path.GetFullPath("results"), settings["WorkDirectory"]);
        }

        [Test]
        public void WhenSeedIsSpecified_RunSettingsIncludeIt()
        {
            var options = new CommandLineOptions("test.dll", "--seed=1234");
            var settings = TextUI.MakeRunSettings(options);

            Assert.That(settings.ContainsKey("RandomSeed"));
            Assert.AreEqual(1234, settings["RandomSeed"]);
        }
    }
}
