// ***********************************************************************
// Copyright (c) 2008 Charlie Poole, Rob Prouse
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

#if !(NETCOREAPP1_1 || NETCOREAPP2_0)
using System;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class RuntimeFrameworkTests
    {
        static readonly RuntimeType currentRuntime =
            Type.GetType("Mono.Runtime", false) != null
                ? RuntimeType.Mono
                : RuntimeType.Net;

        [Test]
        public void CanGetCurrentFramework()
        {
            RuntimeFramework framework = RuntimeFramework.CurrentFramework;

            Assert.That(framework.Runtime, Is.EqualTo(currentRuntime), "#1");
            Assert.That(framework.ClrVersion, Is.EqualTo(Environment.Version), "#2");
        }

#if NET45
        [Test]
        public void TargetFrameworkIsSetCorrectly()
        {
            // We use reflection so it will compile and pass on Mono,
            // including older versions that do not have the property.
            var prop = typeof(AppDomainSetup).GetProperty("FrameworkName");
            Assume.That(prop, Is.Not.Null);
            Assert.That(
                prop.GetValue(AppDomain.CurrentDomain.SetupInformation),
                Is.EqualTo(".NETFramework,Version=v4.5"));
        }

        [Test]
        public void DoesNotRunIn40CompatibilityModeWhenCompiled45()
        {
            var uri = new Uri( "http://host.com/path./" );
            var uriStr = uri.ToString();
            Assert.AreEqual( "http://host.com/path./", uriStr );
        }
#elif NET40
        [Test]
        [Platform(Exclude = "Mono", Reason = "Mono does not run assemblies targeting 4.0 in compatibility mode")]
        public void RunsIn40CompatibilityModeWhenCompiled40()
        {
            var uri = new Uri("http://host.com/path./");
            var uriStr = uri.ToString();
            Assert.AreEqual("http://host.com/path/", uriStr);
        }
#endif

        [Test]
        public void CurrentFrameworkHasBuildSpecified()
        {
            Assert.That(RuntimeFramework.CurrentFramework.ClrVersion.Build, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(frameworkData))]
        public void CanCreateUsingFrameworkVersion(FrameworkData data)
        {
            RuntimeFramework framework = new RuntimeFramework(data.runtime, data.frameworkVersion);
            Assert.AreEqual(data.runtime, framework.Runtime, "#1");
            Assert.AreEqual(data.frameworkVersion, framework.FrameworkVersion, "#2");
            Assert.AreEqual(data.clrVersion, framework.ClrVersion, "#3");
        }

        [TestCaseSource(nameof(frameworkData))]
        public void CanCreateUsingClrVersion(FrameworkData data)
        {
            Assume.That(data.frameworkVersion.Major != 3, "#0");

            RuntimeFramework framework = new RuntimeFramework(data.runtime, data.clrVersion);
            Assert.AreEqual(data.runtime, framework.Runtime, "#1");
            Assert.AreEqual(data.frameworkVersion, framework.FrameworkVersion, "#2");
            Assert.AreEqual(data.clrVersion, framework.ClrVersion, "#3");
        }

        [TestCaseSource(nameof(frameworkData))]
        public void CanParseRuntimeFramework(FrameworkData data)
        {
            RuntimeFramework framework = RuntimeFramework.Parse(data.representation);
            Assert.AreEqual(data.runtime, framework.Runtime, "#1");
            Assert.AreEqual(data.clrVersion, framework.ClrVersion, "#2");
        }

        [TestCaseSource(nameof(frameworkData))]
        public void CanDisplayFrameworkAsString(FrameworkData data)
        {
            RuntimeFramework framework = new RuntimeFramework(data.runtime, data.frameworkVersion);
            Assert.AreEqual(data.representation, framework.ToString(), "#1");
            Assert.AreEqual(data.displayName, framework.DisplayName, "#2");
        }

        [TestCaseSource(nameof(matchData))]
        public bool CanMatchRuntimes(RuntimeFramework f1, RuntimeFramework f2)
        {
            return f1.Supports(f2);
        }

        internal static TestCaseData[] matchData = new TestCaseData[] {
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, new Version(3,5)),
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Net, new Version(3,5)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, new Version(3,5)),
                new RuntimeFramework(RuntimeType.Net, new Version(3,5)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Net, new Version(2,0,50727)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, new Version(2,0,50727)),
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, new Version(2,0,50727)),
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Mono, new Version(2,0)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Net, new Version(1,1)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, new Version(2,0,50727)),
                new RuntimeFramework(RuntimeType.Net, new Version(2,0,40607)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Mono, new Version(1,1)), // non-existent version but it works
                new RuntimeFramework(RuntimeType.Mono, new Version(1,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Mono, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Any, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Any, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Mono, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Any, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Any, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Any, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Any, new Version(4,0)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, RuntimeFramework.DefaultVersion),
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Net, RuntimeFramework.DefaultVersion))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Any, RuntimeFramework.DefaultVersion),
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Net, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Any, RuntimeFramework.DefaultVersion))
            .Returns(true)
        };

        public struct FrameworkData
        {
            public RuntimeType runtime;
            public Version frameworkVersion;
            public Version clrVersion;
            public string representation;
            public string displayName;

            public FrameworkData(RuntimeType runtime, Version frameworkVersion, Version clrVersion,
                                 string representation, string displayName)
            {
                this.runtime = runtime;
                this.frameworkVersion = frameworkVersion;
                this.clrVersion = clrVersion;
                this.representation = representation;
                this.displayName = displayName;
            }

            public override string ToString()
            {
                return string.Format("<{0},{1},{2}>", this.runtime, this.frameworkVersion, this.clrVersion);
            }
        }

        internal static FrameworkData[] frameworkData = new FrameworkData[] {
            new FrameworkData(RuntimeType.Net, new Version(1,0), new Version(1,0,3705), "net-1.0", "Net 1.0"),
            // new FrameworkData(RuntimeType.Net, new Version(1,0,3705), new Version(1,0,3705), "net-1.0.3705", "Net 1.0.3705"),
            // new FrameworkData(RuntimeType.Net, new Version(1,0), new Version(1,0,3705), "net-1.0.3705", "Net 1.0.3705"),
            new FrameworkData(RuntimeType.Net, new Version(1,1), new Version(1,1,4322), "net-1.1", "Net 1.1"),
            // new FrameworkData(RuntimeType.Net, new Version(1,1,4322), new Version(1,1,4322), "net-1.1.4322", "Net 1.1.4322"),
            new FrameworkData(RuntimeType.Net, new Version(2,0), new Version(2,0,50727), "net-2.0", "Net 2.0"),
            // new FrameworkData(RuntimeType.Net, new Version(2,0,40607), new Version(2,0,40607), "net-2.0.40607", "Net 2.0.40607"),
            // new FrameworkData(RuntimeType.Net, new Version(2,0,50727), new Version(2,0,50727), "net-2.0.50727", "Net 2.0.50727"),
            new FrameworkData(RuntimeType.Net, new Version(3,0), new Version(2,0,50727), "net-3.0", "Net 3.0"),
            new FrameworkData(RuntimeType.Net, new Version(3,5), new Version(2,0,50727), "net-3.5", "Net 3.5"),
            new FrameworkData(RuntimeType.Net, new Version(4,0), new Version(4,0,30319), "net-4.0", "Net 4.0"),
            new FrameworkData(RuntimeType.Net, RuntimeFramework.DefaultVersion, RuntimeFramework.DefaultVersion, "net", "Net"),
            new FrameworkData(RuntimeType.Mono, new Version(1,0), new Version(1,1,4322), "mono-1.0", "Mono 1.0"),
            new FrameworkData(RuntimeType.Mono, new Version(2,0), new Version(2,0,50727), "mono-2.0", "Mono 2.0"),
            // new FrameworkData(RuntimeType.Mono, new Version(2,0,50727), new Version(2,0,50727), "mono-2.0.50727", "Mono 2.0.50727"),
            new FrameworkData(RuntimeType.Mono, new Version(3,5), new Version(2,0,50727), "mono-3.5", "Mono 3.5"),
            new FrameworkData(RuntimeType.Mono, new Version(4,0), new Version(4,0,30319), "mono-4.0", "Mono 4.0"),
            new FrameworkData(RuntimeType.Mono, RuntimeFramework.DefaultVersion, RuntimeFramework.DefaultVersion, "mono", "Mono"),
            new FrameworkData(RuntimeType.Any, new Version(1,1), new Version(1,1,4322), "v1.1", "v1.1"),
            new FrameworkData(RuntimeType.Any, new Version(2,0), new Version(2,0,50727), "v2.0", "v2.0"),
            // new FrameworkData(RuntimeType.Any, new Version(2,0,50727), new Version(2,0,50727), "v2.0.50727", "v2.0.50727"),
            new FrameworkData(RuntimeType.Any, new Version(3,5), new Version(2,0,50727), "v3.5", "v3.5"),
            new FrameworkData(RuntimeType.Any, new Version(4,0), new Version(4,0,30319), "v4.0", "v4.0"),
            new FrameworkData(RuntimeType.Any, RuntimeFramework.DefaultVersion, RuntimeFramework.DefaultVersion, "any", "Any")
        };
    }
}
#endif
