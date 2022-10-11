// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class RuntimeFrameworkTests
    {
#if NETCOREAPP
        static readonly RuntimeType currentRuntime = RuntimeType.NetCore;
#else
        static readonly RuntimeType currentRuntime =
            Type.GetType("Mono.Runtime", false) != null
                ? RuntimeType.Mono
                : RuntimeType.NetFramework;
#endif

        [Test]
        public void CanGetCurrentFramework()
        {
            RuntimeFramework framework = RuntimeFramework.CurrentFramework;

            Assert.That(framework.Runtime, Is.EqualTo(currentRuntime), "#1");
            Assert.That(framework.ClrVersion, Is.EqualTo(Environment.Version), "#2");
        }

        [Test]
        [TestCaseSource(nameof(netcoreRuntimes))]
        public void SpecifyingNetCoreVersioningThrowsPlatformException(string netcoreRuntime)
        {
            PlatformHelper platformHelper = new PlatformHelper();
            Assert.Throws<PlatformNotSupportedException>(() => platformHelper.IsPlatformSupported(netcoreRuntime));
        }

        [Test]
        public void SpecifyingNetCoreWithoutVersioningSucceeds()
        {
            PlatformHelper platformHelper = new PlatformHelper();
            bool isNetCore;
#if NETCOREAPP
            isNetCore = true;
#else
            isNetCore = false;
#endif
            Assert.AreEqual(isNetCore, platformHelper.IsPlatformSupported("netcore"));
        }

        [TestCaseSource(nameof(frameworkData))]
        public void CanCreateUsingFrameworkVersion(FrameworkData data)
        {
            RuntimeFramework framework = new RuntimeFramework(data.Runtime, data.FrameworkVersion);
            Assert.AreEqual(data.Runtime, framework.Runtime, "#1");
            Assert.AreEqual(data.FrameworkVersion, framework.FrameworkVersion, "#2");
            Assert.AreEqual(data.ClrVersion, framework.ClrVersion, "#3");
        }

        [TestCaseSource(nameof(frameworkData))]
        public void CanCreateUsingClrVersion(FrameworkData data)
        {
            Assume.That(data.FrameworkVersion.Major != 3, "#0");
            //.NET Framework 4.0+ and .NET Core 2.0+ all have the same CLR version
            Assume.That(data.FrameworkVersion.Major != 4 && data.FrameworkVersion.Minor != 5, "#0");
            Assume.That(data.Runtime != RuntimeType.NetCore, "#0");

            RuntimeFramework framework = new RuntimeFramework(data.Runtime, data.ClrVersion);
            Assert.AreEqual(data.Runtime, framework.Runtime, "#1");
            Assert.AreEqual(data.FrameworkVersion, framework.FrameworkVersion, "#2");
            Assert.AreEqual(data.ClrVersion, framework.ClrVersion, "#3");
        }

        [TestCaseSource(nameof(frameworkData))]
        public void CanParseRuntimeFramework(FrameworkData data)
        {
            RuntimeFramework framework = RuntimeFramework.Parse(data.Representation);
            Assert.AreEqual(data.Runtime, framework.Runtime, "#1");
            Assert.AreEqual(data.ClrVersion, framework.ClrVersion, "#2");
        }

        [TestCaseSource(nameof(frameworkData))]
        public void CanDisplayFrameworkAsString(FrameworkData data)
        {
            RuntimeFramework framework = new RuntimeFramework(data.Runtime, data.FrameworkVersion);
            Assert.AreEqual(data.Representation, framework.ToString(), "#1");
            Assert.AreEqual(data.DisplayName, framework.DisplayName, "#2");
        }

        [TestCaseSource(nameof(matchData))]
        public bool CanMatchRuntimes(RuntimeFramework f1, RuntimeFramework f2)
        {
            return f1.Supports(f2);
        }

        internal static TestCaseData[] matchData = new TestCaseData[] {
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(3,5)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(3,5)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(3,5)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(3,5)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0,50727)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0,50727)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0,50727)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Mono, new Version(2,0)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(1,1)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0,50727)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0,40607)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(3,5)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4,0)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4,0)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(3,5)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4,5)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4,0)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4,5)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4,5)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4,5)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4,5)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(3,5)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4,5)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(1,0)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,0)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,0)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(1,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(3,0)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(1,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(1,0)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(1,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,0)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,1)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,1)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(3,0)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(3,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,0)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,1)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,1)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,2)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,2)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(2,1)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(5,0)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(3,1)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(3,1)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(5,0)))
            .Returns(false),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(5,0)),
                new RuntimeFramework(RuntimeType.NetCore, new Version(3,1)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetCore, new Version(5,0)),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4,8)))
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
                new RuntimeFramework(RuntimeType.NetFramework, RuntimeFramework.DefaultVersion),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)),
                new RuntimeFramework(RuntimeType.NetFramework, RuntimeFramework.DefaultVersion))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.Any, RuntimeFramework.DefaultVersion),
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)))
            .Returns(true),
            new TestCaseData(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(2,0)),
                new RuntimeFramework(RuntimeType.Any, RuntimeFramework.DefaultVersion))
            .Returns(true)
        };

        public struct FrameworkData
        {
            public RuntimeType Runtime;
            public Version FrameworkVersion;
            public Version ClrVersion;
            public string Representation;
            public string DisplayName;

            public FrameworkData(RuntimeType runtime, Version frameworkVersion, Version clrVersion,
                                 string representation, string displayName)
            {
                Runtime = runtime;
                FrameworkVersion = frameworkVersion;
                ClrVersion = clrVersion;
                Representation = representation;
                DisplayName = displayName;
            }

            public override string ToString()
            {
                return $"<{this.Runtime},{this.FrameworkVersion},{this.ClrVersion}>";
            }
        }

        internal static FrameworkData[] frameworkData = new FrameworkData[] {
            new FrameworkData(RuntimeType.NetFramework, new Version(1,0), new Version(1,0,3705), "net-1.0", "Net 1.0"),
            new FrameworkData(RuntimeType.NetFramework, new Version(1,1), new Version(1,1,4322), "net-1.1", "Net 1.1"),
            new FrameworkData(RuntimeType.NetFramework, new Version(2,0), new Version(2,0,50727), "net-2.0", "Net 2.0"),
            new FrameworkData(RuntimeType.NetFramework, new Version(3,0), new Version(2,0,50727), "net-3.0", "Net 3.0"),
            new FrameworkData(RuntimeType.NetFramework, new Version(3,5), new Version(2,0,50727), "net-3.5", "Net 3.5"),
            new FrameworkData(RuntimeType.NetFramework, new Version(4,0), new Version(4,0,30319), "net-4.0", "Net 4.0"),
            new FrameworkData(RuntimeType.NetFramework, new Version(4,5), new Version(4,0,30319), "net-4.5", "Net 4.5"),
            new FrameworkData(RuntimeType.NetFramework, new Version(4,6), new Version(4,0,30319), "net-4.6", "Net 4.6"),
            new FrameworkData(RuntimeType.NetFramework, RuntimeFramework.DefaultVersion, RuntimeFramework.DefaultVersion, "net", "Net"),
            new FrameworkData(RuntimeType.NetCore, new Version(2, 0), new Version(4,0,30319), "netcore-2.0", "NetCore 2.0"),
            new FrameworkData(RuntimeType.NetCore, new Version(2, 1), new Version(4,0,30319), "netcore-2.1", "NetCore 2.1"),
            new FrameworkData(RuntimeType.NetCore, new Version(5, 0), new Version(4,0,30319), "net-5.0", "Net 5.0"),
            new FrameworkData(RuntimeType.NetCore, RuntimeFramework.DefaultVersion, RuntimeFramework.DefaultVersion, "netcore", "NetCore"),
            new FrameworkData(RuntimeType.Mono, new Version(1,0), new Version(1,1,4322), "mono-1.0", "Mono 1.0"),
            new FrameworkData(RuntimeType.Mono, new Version(2,0), new Version(2,0,50727), "mono-2.0", "Mono 2.0"),
            new FrameworkData(RuntimeType.Mono, new Version(3,5), new Version(2,0,50727), "mono-3.5", "Mono 3.5"),
            new FrameworkData(RuntimeType.Mono, new Version(4,0), new Version(4,0,30319), "mono-4.0", "Mono 4.0"),
            new FrameworkData(RuntimeType.Mono, RuntimeFramework.DefaultVersion, RuntimeFramework.DefaultVersion, "mono", "Mono"),
            new FrameworkData(RuntimeType.Any, new Version(1,1), new Version(1,1,4322), "v1.1", "v1.1"),
            new FrameworkData(RuntimeType.Any, new Version(2,0), new Version(2,0,50727), "v2.0", "v2.0"),
            new FrameworkData(RuntimeType.Any, new Version(3,5), new Version(2,0,50727), "v3.5", "v3.5"),
            new FrameworkData(RuntimeType.Any, new Version(4,0), new Version(4,0,30319), "v4.0", "v4.0"),
            new FrameworkData(RuntimeType.Any, RuntimeFramework.DefaultVersion, RuntimeFramework.DefaultVersion, "any", "Any")
        };

        internal static string[] netcoreRuntimes = new string[] {
            "netcore-1.0",
            "netcore-1.1",
            "netcore-2.0",
            "netcore-2.1",
            "netcore-2.2",
            "netcore-3.0",
            "netcore-3.1",
        };
    }
}
