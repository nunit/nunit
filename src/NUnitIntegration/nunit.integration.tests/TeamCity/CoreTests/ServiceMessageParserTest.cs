// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;
using NUnit.Integration.Tests.TeamCity.Core;

namespace NUnit.Integration.Tests.TeamCity.CoreTests
{
    [TestFixture]
    public sealed class ServiceMessageParserTest
    {
        [Test]
        public void BrokenStream0()
        {
            var sb = new StringBuilder();
            for (int c = char.MinValue; c < char.MaxValue; sb.Append((char)c++))
            {                
            }

            for (int c = char.MinValue; c < char.MaxValue; sb.Append((char)c++))
            {                
            }

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStream1()
        {
            var sb = new StringBuilder();
            sb.Append("##te");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStream2()
        {
            var sb = new StringBuilder();
            sb.Append("##teamcity");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStream3()
        {
            var sb = new StringBuilder();
            sb.Append("##teamcity[]");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStream4()
        {
            var sb = new StringBuilder();
            sb.Append("##teamcity[ ]");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStream5()
        {
            var sb = new StringBuilder();
            sb.Append("##teamcity[aa ]");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStream6()
        {
            var sb = new StringBuilder();
            sb.Append("##teamcity[aa '");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStream7()
        {
            var sb = new StringBuilder();
            sb.Append("##teamcity[aa ");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStream8()
        {
            var sb = new StringBuilder();
            sb.Append("##teamcity[aa");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStream9()
        {
            var sb = new StringBuilder();
            sb.Append("##teamcity[aa Z");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStreamA()
        {
            var sb = new StringBuilder();
            sb.Append("##teamcity[aa Z =");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStreamB()
        {
            var sb = new StringBuilder();
            sb.Append("##teamcity[aa Z = '");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStreamC()
        {
            var sb = new StringBuilder();
            sb.Append("##teamcity[aa Z = 'fddd");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStreamD()
        {
            var sb = new StringBuilder();
            sb.Append("##teamcity[aa Z = 'fddd '   ");

            Assert.IsFalse(new ServiceMessageParser().ParseServiceMessages(new StringWrapper(sb.ToString())).ToArray().Any());
        }

        [Test]
        public void BrokenStreamE()
        {
            var sr = new StringWrapper("##teamcity[name 'a'] ");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(1, result.Length);

            var msg = result[0];
            Assert.AreEqual("name", msg.Name);
            Assert.AreEqual(0, msg.Attributes.Count());
            Assert.AreEqual("a", msg.DefaultValue);
        }

        [Test]
        public void BrokenStreamF()
        {
            var sr = new StringWrapper("##teamcity[name 'a']\r\n");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(1, result.Length);

            var msg = result[0];
            Assert.AreEqual("name", msg.Name);
            Assert.AreEqual(0, msg.Attributes.Count());
            Assert.AreEqual("a", msg.DefaultValue);
        }

        [Test]
        public void BrokenStreamCombinatorics()
        {
            var result = new List<int>();
            const string Text = "##teamcity[name 'a'] ##teamcity[name 'a'] ##teamcity[name 'a']  ##teamcity[name 'a'] ##teamcity[name a  =  'a' b='qqq|n'  ]\t##teamcity[name a='z']\r\n##teamcity[name a='z']\r##teamcity[name a='z']##teamcity[package Id='CommonServiceLocator' Version='1.0' Authors='Microsoft' Description='The Common Service Locator library contains a shared interface for service location which application and framework developers can reference. The library provides an abstraction over IoC containers and service locators. Using the library allows an application to indirectly access the capabilities without relying on hard references. The hope is that using this library, third-party applications and frameworks can begin to leverage IoC/Service Location without tying themselves down to a specific implementation.' IsLatestVersion='true' LastUpdated='2011-10-21T16:34:09Z' LicenseUrl='http://commonservicelocator.codeplex.com/license' PackageHash='RJjv0yxm+Fk/ak/CVMTGr0ng7g/nudkVYos4eQrIDpth3BdE1j7J2ddRm8FXtOoIZbgDqTU6hKq5zoackwL3HQ==' PackageHashAlgorithm='SHA512' PackageSize='37216' ProjectUrl='http://commonservicelocator.codeplex.com/' RequireLicenseAcceptance='false' TeamCityBuildId='42' TeamCityDownloadUrl='/repository/download/bt/42:id/null']";
            for (int i = 0; i < Text.Length; i++)
            {
                for (int j = i; j < Text.Length; j++)
                {
                    string input = Text.Substring(i, Text.Length - j);
                    var count = new ServiceMessageParser().ParseServiceMessages(input).ToArray().Length;
                    result.Add(count);
                }
            }
            Assert.IsTrue(result.Contains(9));
        }

        [Test]
        public void BrokenStreamI()
        {
            const string Text = @"##teamcity[package Id='CommonServiceLocator' Version='1.0' Authors='Microsoft' Description='The Common Service Locator library contains a shared interface for service location which application and framework developers can reference. The library provides an abstraction over IoC containers and service locators. Using the library allows an application to indirectly access the capabilities without relying on hard references. The hope is that using this library, third-party applications and frameworks can begin to leverage IoC/Service Location without tying themselves down to a specific implementation.' IsLatestVersion='true' LastUpdated='2011-10-21T16:34:09Z' LicenseUrl='http://commonservicelocator.codeplex.com/license' PackageHash='RJjv0yxm+Fk/ak/CVMTGr0ng7g/nudkVYos4eQrIDpth3BdE1j7J2ddRm8FXtOoIZbgDqTU6hKq5zoackwL3HQ==' PackageHashAlgorithm='SHA512' PackageSize='37216' ProjectUrl='http://commonservicelocator.codeplex.com/' RequireLicenseAcceptance='false' TeamCityBuildId='42' TeamCityDownloadUrl='/repository/download/bt/42:id/null'];";
            new ServiceMessageParser().ParseServiceMessages(new StringWrapper(Text)).Where(x => true).OrderBy(x => x.Name).ToArray();
            new ServiceMessageParser().ParseServiceMessages(new StringWrapper(Text.Trim())).Where(x => true).OrderBy(x => x.Name).ToArray();
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Text.Trim().ToCharArray()));
            new ServiceMessageParser().ParseServiceMessages(new StreamReader(memoryStream)).Where(x => true).OrderBy(x => x.Name).ToArray();
        }

        [Test]
        public void BrokenStreamH()
        {
            const string Text = @"                 ##teamcity[package Id='CommonServiceLocator' Version='1.0' Authors='Microsoft' Description='The Common Service Locator library contains a shared interface for service location which application and framework developers can reference. The library provides an abstraction over IoC containers and service locators. Using the library allows an application to indirectly access the capabilities without relying on hard references. The hope is that using this library, third-party applications and frameworks can begin to leverage IoC/Service Location without tying themselves down to a specific implementation.' IsLatestVersion='true' LastUpdated='2011-10-21T16:34:09Z' LicenseUrl='http://commonservicelocator.codeplex.com/license' PackageHash='RJjv0yxm+Fk/ak/CVMTGr0ng7g/nudkVYos4eQrIDpth3BdE1j7J2ddRm8FXtOoIZbgDqTU6hKq5zoackwL3HQ==' PackageHashAlgorithm='SHA512' PackageSize='37216' ProjectUrl='http://commonservicelocator.codeplex.com/' RequireLicenseAcceptance='false' TeamCityBuildId='42' TeamCityDownloadUrl='/repository/download/bt/42:id/null']                  ";
            var bytes = Encoding.UTF8.GetBytes(Text.ToCharArray());
            var stream = new MemoryStream(bytes);
            var rdr = new StreamReader(stream, Encoding.UTF8);
            var result = new ServiceMessageParser().ParseServiceMessages(rdr).ToArray();
            Assert.IsTrue(result.Length == 1);
        }

        [Test]
        public void BrokenStreamJ()
        {
            const string Text = @"                 ##teamcity[package Id='CommonServiceLocator' Version='1.0' Authors='Microsoft' Description='The Common Service Locator library contains a shared interface for service location which application and framework developers can reference. The library provides an abstraction over IoC containers and service locators. Using the library allows an application to indirectly access the capabilities without relying on hard references. The hope is that using this library, third-party applications and frameworks can begin to leverage IoC/Service Location without tying themselves down to a specific implementation.' IsLatestVersion='true' LastUpdated='2011-10-21T16:34:09Z' LicenseUrl='http://commonservicelocator.codeplex.com/license' PackageHash='RJjv0yxm+Fk/ak/CVMTGr0ng7g/nudkVYos4eQrIDpth3BdE1j7J2ddRm8FXtOoIZbgDqTU6hKq5zoackwL3HQ==' PackageHashAlgorithm='SHA512' PackageSize='37216' ProjectUrl='http://commonservicelocator.codeplex.com/' RequireLicenseAcceptance='false' TeamCityBuildId='42' TeamCityDownloadUrl='/repository/download/bt/42:id/null']                  ";

            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, Text);

                using (var stream = File.OpenRead(path))
                {
                    var rdr = new StreamReader(stream, Encoding.UTF8);
                    var result = new ServiceMessageParser().ParseServiceMessages(rdr).ToArray();
                    Assert.IsTrue(result.Length == 1);
                }
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Test]
        public void ShouldParseServiceSimpleMessage()
        {
            var sr = new StringWrapper("##teamcity[name 'a']");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(1, result.Length);

            var msg = result[0];
            Assert.AreEqual("name", msg.Name);
            Assert.AreEqual(0, msg.Attributes.Count());
            Assert.AreEqual("a", msg.DefaultValue);
        }

        [Test]
        public void ShouldParseServiceSimpleMessagesMulti()
        {
            var sr = new StringWrapper("##teamcity[name 'a'] ##teamcity[name 'a'] ##teamcity[name 'a']  ##teamcity[name 'a']");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(4, result.Length);

            foreach (var msg in result)
            {
                Assert.AreEqual("name", msg.Name);
                Assert.AreEqual(0, msg.Attributes.Count());
                Assert.AreEqual("a", msg.DefaultValue);
            }
        }

        [Test]
        public void ShouldParseServiceSimpleMessagesMulti2()
        {
            var sr = new StringWrapper("##teamcity[name 'a']\r ##teamcity[name 'a']\n ##teamcity[name 'a'] \r\n ##teamcity[name 'a']");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(4, result.Length);

            foreach (var msg in result)
            {
                Assert.AreEqual("name", msg.Name);
                Assert.AreEqual(0, msg.Attributes.Count());
                Assert.AreEqual("a", msg.DefaultValue);
            }
        }

        [Test]
        public void ShouldParseServiceSimpleMessageDecode()
        {
            var sr = new StringWrapper("##teamcity[name '\"|'|n|r|x|l|p|||[|]']");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(1, result.Length);

            var msg = result[0];
            Assert.AreEqual("name", msg.Name);
            Assert.AreEqual(0, msg.Attributes.Count());
            Assert.AreEqual("\"'\n\r\u0085\u2028\u2029|[]", msg.DefaultValue);
        }

        [Test]
        public void ShouldParseServiceComplexMessage0()
        {
            var sr = new StringWrapper("##teamcity[name a='a' b='z']");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(1, result.Length);

            var msg = result[0];
            Assert.AreEqual("name", msg.Name);
            Assert.AreEqual(2, msg.Attributes.Count());
            Assert.AreEqual(new HashSet<string> { "a", "b" }, new HashSet<string>(msg.Attributes));
            Assert.AreEqual(null, msg.DefaultValue);
            Assert.AreEqual("a", msg.GetAttribute("a"));
            Assert.AreEqual("z", msg.GetAttribute("b"));
        }

        [Test]
        public void ShouldParseServiceComplexMessage1()
        {
            var sr = new StringWrapper("##teamcity[name a='a']");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(1, result.Length);

            var msg = result[0];
            Assert.AreEqual("name", msg.Name);
            Assert.AreEqual(1, msg.Attributes.Count());
            Assert.AreEqual(new HashSet<string> { "a" }, new HashSet<string>(msg.Attributes));
            Assert.AreEqual(null, msg.DefaultValue);
            Assert.AreEqual("a", msg.GetAttribute("a"));
        }

        [Test]
        public void ShouldParseServiceComplexMessage2()
        {
            var sr = new StringWrapper("##teamcity[name    a='a'     b='z'   ]");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(1, result.Length);

            var msg = result[0];
            Assert.AreEqual("name", msg.Name);
            Assert.AreEqual(2, msg.Attributes.Count());
            Assert.AreEqual(new HashSet<string> { "a", "b" }, new HashSet<string>(msg.Attributes));
            Assert.AreEqual(null, msg.DefaultValue);
            Assert.AreEqual("a", msg.GetAttribute("a"));
            Assert.AreEqual("z", msg.GetAttribute("b"));
        }

        [Test]
        public void ShouldParseServiceComplexMessage3()
        {
            var sr = new StringWrapper("  ##teamcity[name a  =  'a'   ]  ");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(1, result.Length);

            var msg = result[0];
            Assert.AreEqual("name", msg.Name);
            Assert.AreEqual(1, msg.Attributes.Count());
            Assert.AreEqual(new HashSet<string> { "a" }, new HashSet<string>(msg.Attributes));
            Assert.AreEqual(null, msg.DefaultValue);
            Assert.AreEqual("a", msg.GetAttribute("a"));
        }

        [Test]
        public void ShouldParseServiceComplexMessageMulti()
        {
            var sr = new StringWrapper("  ##teamcity[name a='z']##teamcity[name a='z']##teamcity[name a='z']  ");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(3, result.Length);

            foreach (var msg in result)
            {
                Assert.AreEqual("name", msg.Name);
                Assert.AreEqual(1, msg.Attributes.Count());
                Assert.AreEqual(new HashSet<string> { "a" }, new HashSet<string>(msg.Attributes));
                Assert.AreEqual(null, msg.DefaultValue);
                Assert.AreEqual("z", msg.GetAttribute("a"));
            }
        }

        [Test]
        public void ShouldParseServiceComplexMessageMulti2()
        {
            var sr = new StringWrapper("  ##teamcity[name a='z']\r  ##teamcity[name a='z']\n##teamcity[name a='z']  ");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(3, result.Length);

            foreach (var msg in result)
            {
                Assert.AreEqual("name", msg.Name);
                Assert.AreEqual(1, msg.Attributes.Count());
                Assert.AreEqual(new HashSet<string> { "a" }, new HashSet<string>(msg.Attributes));
                Assert.AreEqual(null, msg.DefaultValue);
                Assert.AreEqual("z", msg.GetAttribute("a"));
            }
        }

        [Test]
        public void ShouldParseServiceMulti()
        {
            var sr = new StringWrapper("  ##teamcity[name a='z']\r##teamcity[zzz 'z']\n##teamcity[name a='z']  ");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(3, result.Length);

            var msg = result[0];
            Assert.AreEqual("name", msg.Name);
            Assert.AreEqual(1, msg.Attributes.Count());
            Assert.AreEqual(new HashSet<string> { "a" }, new HashSet<string>(msg.Attributes));
            Assert.AreEqual(null, msg.DefaultValue);
            Assert.AreEqual("z", msg.GetAttribute("a"));

            msg = result[1];
            Assert.AreEqual("zzz", msg.Name);
            Assert.AreEqual(0, msg.Attributes.Count());
            Assert.AreEqual("z", msg.DefaultValue);

            msg = result[2];
            Assert.AreEqual("name", msg.Name);
            Assert.AreEqual(1, msg.Attributes.Count());
            Assert.AreEqual(new HashSet<string> { "a" }, new HashSet<string>(msg.Attributes));
            Assert.AreEqual(null, msg.DefaultValue);
            Assert.AreEqual("z", msg.GetAttribute("a"));
        }

        [Test]
        public void ShouldParseServiceComplexMessageEscaping()
        {
            var sr = new StringWrapper("##teamcity[name    a='1\"|'|n|r|x|l|p|||[|]'     b='2\"|'|n|r|x|l|p|||[|]'   ]");
            var result = new ServiceMessageParser().ParseServiceMessages(sr).ToArray();
            Assert.AreEqual(1, result.Length);

            var msg = result[0];
            Assert.AreEqual("name", msg.Name);
            Assert.AreEqual(2, msg.Attributes.Count());
            Assert.AreEqual(new HashSet<string> { "a", "b" }, new HashSet<string>(msg.Attributes));
            Assert.AreEqual(null, msg.DefaultValue);
            Assert.AreEqual("1\"'\n\r\u0085\u2028\u2029|[]", msg.GetAttribute("a"));
            Assert.AreEqual("2\"'\n\r\u0085\u2028\u2029|[]", msg.GetAttribute("b"));
        }

        [Test]
        public void ShouldParseString()
        {
            var result = new ServiceMessageParser().ParseServiceMessages("##teamcity[name    a='1\"|'|n|r|x|l|p|||[|]'     b='2\"|'|n|r|x|l|p|||[|]'   ]").ToArray();
            Assert.AreEqual(1, result.Length);

            var msg = result[0];
            Assert.AreEqual("name", msg.Name);
            Assert.AreEqual(2, msg.Attributes.Count());
            Assert.AreEqual(new HashSet<string> { "a", "b" }, new HashSet<string>(msg.Attributes));
            Assert.AreEqual(null, msg.DefaultValue);
            Assert.AreEqual("1\"'\n\r\u0085\u2028\u2029|[]", msg.GetAttribute("a"));
            Assert.AreEqual("2\"'\n\r\u0085\u2028\u2029|[]", msg.GetAttribute("b"));
        }
    }
}