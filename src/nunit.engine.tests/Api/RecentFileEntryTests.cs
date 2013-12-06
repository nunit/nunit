// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Collections;
using System.Text;
using NUnit.Framework;

namespace NUnit.Engine.Api.Tests
{
    [TestFixture]
    public class RecentFileEntryTests
    {
        private RecentFileEntry entry;
        private static readonly string entryPath = "a/b/c";
		private static readonly string entryPathWithComma = @"D:\test\test, and further research\program1\program1.exe";
		private static Version entryVersion = new Version("1.2");
		private static Version currentVersion = Environment.Version;

		[Test]
		public void CanCreateFromSimpleFileName()
		{
			entry = new RecentFileEntry( entryPath );
			Assert.AreEqual( entryPath, entry.Path );
			Assert.AreEqual( currentVersion, entry.CLRVersion );
		}

		[Test]
		public void CanCreateFromFileNameAndVersion()
		{
			entry = new RecentFileEntry( entryPath, entryVersion );
			Assert.AreEqual( entryPath, entry.Path );
			Assert.AreEqual( entryVersion, entry.CLRVersion );
		}

        [Test]
        public void EntryCanDisplayItself()
        {
			entry = new RecentFileEntry( entryPath, entryVersion );
			Assert.AreEqual(
                entryPath + RecentFileEntry.Separator + entryVersion.ToString(),
                entry.ToString());
        }

		[Test]
		public void CanParseSimpleFileName()
		{
			entry = RecentFileEntry.Parse(entryPath);
			Assert.AreEqual(entryPath, entry.Path);
			Assert.AreEqual(currentVersion, entry.CLRVersion);
		}

		[Test]
		public void CanParseSimpleFileNameWithComma()
		{
			entry = RecentFileEntry.Parse(entryPathWithComma);
			Assert.AreEqual(entryPathWithComma, entry.Path);
			Assert.AreEqual(currentVersion, entry.CLRVersion);
		}

		[Test]
		public void CanParseFileNamePlusVersionString()
		{
			string text = entryPath + RecentFileEntry.Separator + entryVersion.ToString();
			entry = RecentFileEntry.Parse(text);
			Assert.AreEqual(entryPath, entry.Path);
			Assert.AreEqual(entryVersion, entry.CLRVersion);
		}

		[Test]
		public void CanParseFileNameWithCommaPlusVersionString()
		{
			string text = entryPathWithComma + RecentFileEntry.Separator + entryVersion.ToString();
			entry = RecentFileEntry.Parse(text);
			Assert.AreEqual(entryPathWithComma, entry.Path);
			Assert.AreEqual(entryVersion, entry.CLRVersion);
		}
	}
}
