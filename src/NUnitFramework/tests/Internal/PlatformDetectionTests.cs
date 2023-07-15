// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    /// <summary>
    /// Summary description for PlatformHelperTests.
    /// </summary>
    [TestFixture]
    public class PlatformDetectionTests
    {
        private static readonly PlatformHelper Win95Helper = new PlatformHelper(
            new OSPlatform(PlatformID.Win32Windows, new Version(4, 0)),
            new RuntimeFramework(RuntimeType.NetFramework, new Version(1, 1, 4322, 0)));

        private static readonly PlatformHelper WinXPHelper = new PlatformHelper(
            new OSPlatform(PlatformID.Win32NT, new Version(5, 1)),
            new RuntimeFramework(RuntimeType.NetFramework, new Version(1, 1, 4322, 0)));

        private void CheckOSPlatforms(OSPlatform os,
            string expectedPlatforms)
        {
            Assert.That(expectedPlatforms, Is.SubsetOf(PlatformHelper.OSPlatforms).IgnoreCase,
                "Error in test: one or more expected platforms is not a valid OSPlatform.");

            CheckPlatforms(
                new PlatformHelper(os, RuntimeFramework.CurrentFramework),
                expectedPlatforms,
                PlatformHelper.OSPlatforms);
        }

        private void CheckRuntimePlatforms(RuntimeFramework runtimeFramework,
            string expectedPlatforms)
        {
            CheckPlatforms(
                new PlatformHelper(OSPlatform.CurrentPlatform, runtimeFramework),
                expectedPlatforms,
                PlatformHelper.RuntimePlatforms + ",NET-1.0,NET-1.1,NET-2.0,NET-3.0,NET-3.5,NET-4.0,NET-4.5,NET-4.6,MONO-1.0,MONO-2.0,MONO-3.0,MONO-3.5,MONO-4.0,MONOTOUCH");
        }

        private void CheckPlatforms(PlatformHelper helper,
            string expectedPlatforms, string checkPlatforms)
        {
            string[] expected = expectedPlatforms.Split(new[] { ',' });
            string[] check = checkPlatforms.Split(new[] { ',' });

            foreach (string testPlatform in check)
            {
                bool shouldPass = false;

                foreach (string platform in expected)
                {
                    if (shouldPass = platform.ToLower() == testPlatform.ToLower())
                        break;
                }

                bool didPass = helper.IsPlatformSupported(testPlatform);

                if (shouldPass && !didPass)
                    Assert.Fail("Failed to detect {0}", testPlatform);
                else if (didPass && !shouldPass)
                    Assert.Fail("False positive on {0}", testPlatform);
                else if (!shouldPass && !didPass)
                    Assert.That(helper.Reason, Is.EqualTo("Only supported on " + testPlatform));
            }
        }

        [Test]
        public void DetectWin95()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32Windows, new Version(4, 0)),
                "Win95,Win32Windows,Win32,Win");
        }

        [Test]
        public void DetectWin98()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32Windows, new Version(4, 10)),
                "Win98,Win32Windows,Win32,Win");
        }

        [Test]
        public void DetectWinMe()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32Windows, new Version(4, 90)),
                "WinMe,Win32Windows,Win32,Win");
        }

        [Test]
        public void DetectNT3()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(3, 51)),
                "NT3,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectNT4()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(4, 0)),
                "NT4,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWin2K()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(5, 0)),
                "Win2K,NT5,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWinXP()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(5, 1)),
                "WinXP,NT5,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWinXPProfessionalX64()
        {
            CheckOSPlatforms(
                    new OSPlatform(PlatformID.Win32NT, new Version(5, 2), OSPlatform.ProductType.WorkStation),
                    "WinXP,NT5,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWin2003Server()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(5, 2), OSPlatform.ProductType.Server),
                "Win2003Server,NT5,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectVista()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(6, 0), OSPlatform.ProductType.WorkStation),
                "Vista,NT6,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWin2008ServerOriginal()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(6, 0), OSPlatform.ProductType.Server),
                "Win2008Server,NT6,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWin2008ServerR2()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(6, 1), OSPlatform.ProductType.Server),
                "Win2008Server,Win2008ServerR2,NT6,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWindows7()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(6, 1), OSPlatform.ProductType.WorkStation),
                "Win7,Windows7,NT6,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWindows2012ServerOriginal()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(6, 2), OSPlatform.ProductType.Server),
                "Win2012Server,Win2012ServerR1,NT6,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWindows2012ServerR2()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(6, 3), OSPlatform.ProductType.Server),
                "Win2012Server,Win2012ServerR2,NT6,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWindows8()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(6, 2), OSPlatform.ProductType.WorkStation),
                "Win8,Windows8,NT6,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWindows81()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(6, 3), OSPlatform.ProductType.WorkStation),
                "Win8.1,Windows8.1,NT6,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWindows10()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(10, 0), OSPlatform.ProductType.WorkStation),
                "Win10,Windows10,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWindows11()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(10, 22000), OSPlatform.ProductType.WorkStation),
                "Win11,Windows11,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectWindowsServer()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Win32NT, new Version(10, 0), OSPlatform.ProductType.Server),
                "WindowsServer10,Win32NT,Win32,Win");
        }

        [Test]
        public void DetectUnixUnderMicrosoftDotNet()
        {
            CheckOSPlatforms(
                new OSPlatform(OSPlatform.UnixPlatformIDMicrosoft, new Version(0, 0)),
                "UNIX,Linux");
        }

        [Test]
        public void DetectUnixUnderMono()
        {
            CheckOSPlatforms(
                new OSPlatform(OSPlatform.UnixPlatformIDMono, new Version(0, 0)),
                "UNIX,Linux");
        }

        [Test]
        public void DetectXbox()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.Xbox, new Version(0, 0)),
                "Xbox");
        }

        [Test]
        public void DetectMacOSX()
        {
            CheckOSPlatforms(
                new OSPlatform(PlatformID.MacOSX, new Version(0, 0)),
                "MacOSX");
        }

        [Test]
        public void DetectNet35()
        {
            CheckRuntimePlatforms(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(3, 5)),
                "Net,Net-2.0,Net-3.0,Net-3.5");
        }

        [Test]
        public void DetectNet40()
        {
            CheckRuntimePlatforms(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4, 0, 30319, 0)),
                "Net,Net-4.0");
        }

        [Test]
        public void DetectNet45()
        {
            CheckRuntimePlatforms(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4, 5, 0, 0)),
                "Net,Net-4.0,Net-4.5");
        }

        [Test]
        public void DetectNet46()
        {
            CheckRuntimePlatforms(
                new RuntimeFramework(RuntimeType.NetFramework, new Version(4, 6, 0, 0)),
                "Net,Net-4.0,Net-4.5,Net-4.6");
        }

        [Test]
        public void DetectSSCLI()
        {
            CheckRuntimePlatforms(
                new RuntimeFramework(RuntimeType.SSCLI, new Version(1, 0, 3, 0)),
                "SSCLI,Rotor");
        }

        [Test]
        public void DetectMono10()
        {
            CheckRuntimePlatforms(
                new RuntimeFramework(RuntimeType.Mono, new Version(1, 1, 4322, 0)),
                "Mono,Mono-1.0");
        }

        [Test]
        public void DetectMono20()
        {
            CheckRuntimePlatforms(
                new RuntimeFramework(RuntimeType.Mono, new Version(2, 0, 50727, 0)),
                "Mono,Mono-2.0");
        }

        [Test]
        public void DetectMono30()
        {
            CheckRuntimePlatforms(
                new RuntimeFramework(RuntimeType.Mono, new Version(3, 0)),
                "Mono,Mono-2.0,Mono-3.0");
        }

        [Test]
        public void DetectMono35()
        {
            CheckRuntimePlatforms(
                new RuntimeFramework(RuntimeType.Mono, new Version(3, 5)),
                "Mono,Mono-2.0,Mono-3.0,Mono-3.5");
        }

        [Test]
        public void DetectMono40()
        {
            CheckRuntimePlatforms(
                new RuntimeFramework(RuntimeType.Mono, new Version(4, 0, 30319)),
                "Mono,Mono-4.0");
        }

        [Test]
        public void DetectMonoTouch()
        {
            CheckRuntimePlatforms(
                new RuntimeFramework(RuntimeType.MonoTouch, new Version(4, 0, 30319)),
                "MonoTouch");
        }

        [Test]
        public void DetectNetCore()
        {
            CheckRuntimePlatforms(
                new RuntimeFramework(RuntimeType.NetCore, new Version(0, 0, 0)),
                "NetCore");
        }

        [Test]
        public void DetectExactVersion()
        {
            Assert.That(WinXPHelper.IsPlatformSupported("net-1.1.4322"), Is.True);
            Assert.That(WinXPHelper.IsPlatformSupported("net-1.1.4322.0"), Is.True);
            Assert.That(WinXPHelper.IsPlatformSupported("net-1.1.4323.0"), Is.False);
            Assert.That(WinXPHelper.IsPlatformSupported("net-1.1.4322.1"), Is.False);
        }

        [Test]
        public void ArrayOfPlatforms()
        {
            string[] platforms = new[] { "NT4", "Win2K", "WinXP" };
            Assert.That(WinXPHelper.IsPlatformSupported(platforms), Is.True);
            Assert.That(Win95Helper.IsPlatformSupported(platforms), Is.False);
        }

        [Test]
        public void PlatformAttribute_Include()
        {
            PlatformAttribute attr = new PlatformAttribute("Win2K,WinXP,NT4");
            Assert.That(WinXPHelper.IsPlatformSupported(attr), Is.True);
            Assert.That(Win95Helper.IsPlatformSupported(attr), Is.False);
            Assert.That(Win95Helper.Reason, Is.EqualTo("Only supported on Win2K,WinXP,NT4"));
        }

        [Test]
        public void PlatformAttribute_Exclude()
        {
            PlatformAttribute attr = new PlatformAttribute();
            attr.Exclude = "Win2K,WinXP,NT4";
            Assert.That(WinXPHelper.IsPlatformSupported(attr), Is.False);
            Assert.That(WinXPHelper.Reason, Is.EqualTo("Not supported on Win2K,WinXP,NT4"));
            Assert.That(Win95Helper.IsPlatformSupported(attr), Is.True);
        }

        [Test]
        public void PlatformAttribute_IncludeAndExclude()
        {
            PlatformAttribute attr = new PlatformAttribute("Win2K,WinXP,NT4");
            attr.Exclude = "Mono";
            Assert.That(Win95Helper.IsPlatformSupported(attr), Is.False);
            Assert.That(Win95Helper.Reason, Is.EqualTo("Only supported on Win2K,WinXP,NT4"));
            Assert.That(WinXPHelper.IsPlatformSupported(attr), Is.True);
            attr.Exclude = "Net";
            Assert.That(Win95Helper.IsPlatformSupported(attr), Is.False);
            Assert.That(Win95Helper.Reason, Is.EqualTo("Only supported on Win2K,WinXP,NT4"));
            Assert.That(WinXPHelper.IsPlatformSupported(attr), Is.False);
            Assert.That(WinXPHelper.Reason, Is.EqualTo("Not supported on Net"));
        }

        [Test]
        public void PlatformAttribute_InvalidPlatform()
        {
            PlatformAttribute attr = new PlatformAttribute("Net-1.0,Net11,Mono");
            Assert.Throws<InvalidPlatformException>(
                () => WinXPHelper.IsPlatformSupported(attr),
                "Invalid platform name Net11");
        }

        [Test]
        public void PlatformAttribute_ProcessBitNess()
        {
            PlatformAttribute attr32 = new PlatformAttribute("32-Bit");
            PlatformAttribute attr64 = new PlatformAttribute("64-Bit");
            PlatformHelper helper = new PlatformHelper();

            // This test verifies that the two labels are known,
            // do not cause an error and return consistent results.
            bool is32BitProcess = helper.IsPlatformSupported(attr32);
            bool is64BitProcess = helper.IsPlatformSupported(attr64);
            Assert.That(is32BitProcess & is64BitProcess, Is.False, "Process cannot be both 32 and 64 bit");
            Assert.That(is64BitProcess, Is.EqualTo(Environment.Is64BitProcess));
        }

        [Test]
        public void PlatformAttribute_OperatingSystemBitNess()
        {
            PlatformAttribute attr32 = new PlatformAttribute("32-Bit-OS");
            PlatformAttribute attr64 = new PlatformAttribute("64-Bit-OS");
            PlatformHelper helper = new PlatformHelper();

            bool is64BitOS = Environment.Is64BitOperatingSystem;
            Assert.That(helper.IsPlatformSupported(attr32), Is.Not.EqualTo(is64BitOS));
            Assert.That(helper.IsPlatformSupported(attr64), Is.EqualTo(is64BitOS));
        }
    }
}
