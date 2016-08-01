﻿// ***********************************************************************
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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using NUnit.Framework;

namespace NUnit.Engine.Services.Tests
{
    public static class DomainManagerStaticTests
    {
        static string path1 = TestPath("/test/bin/debug/test1.dll");
        static string path2 = TestPath("/test/bin/debug/test2.dll");
        static string path3 = TestPath("/test/utils/test3.dll");

        [Test]
        public static void GetPrivateBinPath()
        {
            string[] assemblies = new string[] { path1, path2, path3 };

            Assert.AreEqual(
                TestPath("bin/debug") + Path.PathSeparator + TestPath("utils"),
                DomainManager.GetPrivateBinPath(TestPath("/test"), assemblies));
        }

        [Test]
        public static void GetCommonAppBase_OneElement()
        {
            string[] assemblies = new string[] { path1 };

            Assert.AreEqual(
                TestPath("/test/bin/debug"),
                DomainManager.GetCommonAppBase(assemblies));
        }

        [Test]
        public static void GetCommonAppBase_TwoElements_SameDirectory()
        {
            string[] assemblies = new string[] { path1, path2 };

            Assert.AreEqual(
                TestPath("/test/bin/debug"),
                DomainManager.GetCommonAppBase(assemblies));
        }

        [Test]
        public static void GetCommonAppBase_TwoElements_DifferentDirectories()
        {
            string[] assemblies = new string[] { path1, path3 };

            Assert.AreEqual(
                TestPath("/test"),
                DomainManager.GetCommonAppBase(assemblies));
        }

        [Test]
        public static void GetCommonAppBase_ThreeElements_DifferentDirectories()
        {
            string[] assemblies = new string[] { path1, path2, path3 };

            Assert.AreEqual(
                TestPath("/test"),
                DomainManager.GetCommonAppBase(assemblies));
        }

        [Test]
        public static void ProperConfigFileIsUsed()
        {
            var configFileName = "nunit.engine.tests.dll.config";
            var expectedPath = Path.Combine(TestContext.CurrentContext.TestDirectory, configFileName);
            Assert.That(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, Is.EqualTo(expectedPath));
        }

        [Test]
        public static void CanReadConfigFile()
        {
            Assert.That(ConfigurationManager.AppSettings.Get("test.setting"), Is.EqualTo("54321"));
        }

        [TestCase("/path/to/mytest.dll", null, "/path/to/")]
        [TestCase("/path/to/mytest.dll", "/path", "/path/")]
        public static void ApplicationBaseTests(string filePath, string appBase, string expected)
        {
            filePath = TestPath(filePath);
            appBase = TestPath(appBase);
            expected = TestPath(expected);

            var package = new TestPackage(filePath);
            if (appBase != null)
                package.Settings["BasePath"] = appBase;

            Assert.That(DomainManager.GetApplicationBase(package), Is.SamePath(expected));
        }

        [TestCase("/path/to/mytest.dll", "/path/to", null)]
        [TestCase("/path/to/mytest.dll", "/path", "to")]
        public static void PrivateBinPathTests(string filePath, string appBase, string expected)
        {
            filePath = TestPath(filePath);
            appBase = TestPath(appBase);
            expected = TestPath(expected);

            var package = new TestPackage(filePath);

            Assert.That(DomainManager.GetPrivateBinPath(appBase, package), Is.EqualTo(expected));
        }

        [TestCase("/path/to/mytest.dll", "/path/to", null, "/path/to/mytest.dll.config")]
        [TestCase("/path/to/mytest.dll", "/path", null, "/path/to/mytest.dll.config")]
        [TestCase("/path/to/mytest.nunit", "/path/to", null, null)]
        [TestCase("/path/to/mytest.nunit", "/path/to", "/path/to/mytest.config", "/path/to/mytest.config")]
        public static void ConfigFileTests(string filePath, string appBase, string configSetting, string expected)
        {
            filePath = TestPath(filePath);
            appBase = TestPath(appBase);
            configSetting = TestPath(configSetting);
            expected = TestPath(expected);

            var package = new TestPackage(filePath);
            if (configSetting != null)
                package.Settings["ConfigurationFile"] = configSetting;

            Assert.That(DomainManager.GetConfigFile(appBase, package), Is.EqualTo(expected));
        }

        /// <summary>
        /// Take a valid Linux filePath and make a valid windows filePath out of it
        /// if we are on Windows. Change slashes to backslashes and, if the
        /// filePath starts with a slash, add C: in front of it.
        /// </summary>
        private static string TestPath(string path)
        {
            if (path != null && Path.DirectorySeparatorChar != '/')
            {
                path = path.Replace('/', Path.DirectorySeparatorChar);
                if (path[0] == Path.DirectorySeparatorChar)
                    path = "C:" + path;
            }

            return path;
        }

        private static IEnumerable<TestCaseData> AppDomainData()
        {
            yield return new TestCaseData(new TestPackage(@"C:\path\to\mytest.dll"), @"C:\path\to");
        }
    }
}
