// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using NUnit.Framework.Api;

namespace NUnit.Framework.Tests.Internal
{
    internal class DefaultTestAssemblyBuilderTests
    {
        [Test]
        public void Build_CanBuildTests()
        {
            var builder = new DefaultTestAssemblyBuilder();
            var opts = new Dictionary<string, object>();
            var asm = Assembly.Load("nunit.testdata");

            var result = builder.Build(asm, opts);

            Assert.That(result.TestCaseCount, Is.GreaterThan(500));
        }

        [Test]
        public void Build_CanBuildTestsForCulture([ValueSource(nameof(GetTestedCultures))] string culture)
        {
            using var cultureContext = ThreadCultureContext.Capture();

            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Build_CanBuildTests();
        }

        private static string[] GetTestedCultures() => new[]
            {
                "en-US", // US English
                "th-TH", // Thai
                "tr-TR", // Turkish
            };

        private class ThreadCultureContext : IDisposable
        {
            private readonly CultureInfo _culture;
            private readonly CultureInfo _uiCulture;
            private bool _disposed = false;

            public ThreadCultureContext(CultureInfo culture, CultureInfo uiCulture)
            {
                _culture = culture;
                _uiCulture = uiCulture;
            }

            public static ThreadCultureContext Capture()
                 => new ThreadCultureContext(CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    CultureInfo.CurrentCulture = _culture;
                    CultureInfo.CurrentUICulture = _uiCulture;
                }
            }
        }
    }
}
