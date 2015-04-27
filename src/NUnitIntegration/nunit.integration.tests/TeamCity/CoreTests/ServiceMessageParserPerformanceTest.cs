namespace JetBrains.TeamCityCert.Tools.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using NUnit.Framework;

    [TestFixture, Explicit]
    public class ServiceMessageParserPerformanceTest
    {
        [Test]
        public void Parse1000()
        {
            var ps = new ServiceMessageParser();
            var text = GenerateTestData(1000);
            var trash = new ArrayList();
            MeasureTime(TimeSpan.FromMilliseconds(100), 10, () => trash.Add(ps.ParseServiceMessages(text).ToArray()));
            Console.Out.WriteLine(trash);
        }

        [Test]
        public void Parse10000()
        {
            var ps = new ServiceMessageParser();
            var text = GenerateTestData(10000);
            var trash = new ArrayList();
            MeasureTime(TimeSpan.FromMilliseconds(1000), 10, () => trash.Add(ps.ParseServiceMessages(text).ToArray()));
            Console.Out.WriteLine(trash);
        }

        private static string GenerateTestData(int sz)
        {
            var sb = new StringBuilder();
            while (sz-- > 0)
            {
                sb.AppendFormat("##teamcity[package Id='CommonServiceLocator{0}' Version='1.0.{0}' IsLatestVersion='true' teamcity.artifactPath='some/package/download/CommonServiceLocator.1.0.{0}.nupkg' Authors='Microsoft' Description='The Common Service Locator library contains a shared interface for service location which application and framework developers can reference. The library provides an abstraction over IoC containers and service locators. Using the library allows an application to indirectly access the capabilities without relying on hard references. The hope is that using this library, third-party applications and frameworks can begin to leverage IoC/Service Location without tying themselves down to a specific implementation.'  LastUpdated='2011-10-21T16:34:09Z' LicenseUrl='http://commonservicelocator.codeplex.com/license' PackageHash='RJjv0yxm+Fk/ak/CVMTGr0ng7g/nudkVYos4eQrIDpth3BdE1j7J2ddRm8FXtOoIZbgDqTU6hKq5zoackwL3HQ==' PackageHashAlgorithm='SHA512' PackageSize='37216' ProjectUrl='http://commonservicelocator.codeplex.com/' RequireLicenseAcceptance='false' TeamCityBuildId='42' TeamCityDownloadUrl='/app/nuget-packages/jonnyzzz5Z8mBocMOdtH4CJhxRaev11WxcWpHVCrrulezz/42/some/package/download/CommonServiceLocator.1.0.nupkg']", sz);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private static void MeasureTime(TimeSpan time, int repeat, Action action)
        {
            var result = new List<TimeSpan>();
            while (repeat-- > 0)
            {
                var start = DateTime.Now;
                action();
                var span = DateTime.Now - start;

                Console.Out.WriteLine("Action finished in: {0}", span.TotalMilliseconds);

                if (span < time)
                {
                    return;
                }

                result.Add(span);
            }

            Assert.Fail("Action is expected to complete in {0}, but was [{1}]", time.TotalMilliseconds, string.Join(", ", result.Select(x => x.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)).ToArray()));
        }
    }
}