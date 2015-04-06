// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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
using System.IO;
using NUnit.Framework;

namespace NUnit.Engine.Internal.Tests
{
	[TestFixture]
	public class PathUtilTests : PathUtils
	{
		[Test]
		public void CheckDefaults()
		{
			Assert.AreEqual( Path.DirectorySeparatorChar, PathUtils.DirectorySeparatorChar );
			Assert.AreEqual( Path.AltDirectorySeparatorChar, PathUtils.AltDirectorySeparatorChar );
		}
	}

	// Local Assert extension
	internal class Assert : NUnit.Framework.Assert
	{
		public static void SamePathOrUnder( string path1, string path2 )
		{
			string msg = "\r\n\texpected: Same path or under <{0}>\r\n\t but was: <{1}>";
			Assert.IsTrue( PathUtils.SamePathOrUnder( path1, path2 ), msg, path1, path2 );
		}

		public static void NotSamePathOrUnder( string path1, string path2 )
		{
			string msg = "\r\n\texpected: Not same path or under <{0}>\r\n\t but was: <{1}>";
			Assert.IsFalse( PathUtils.SamePathOrUnder( path1, path2 ), msg, path1, path2 );
		}
	}

	[TestFixture]
	public class PathUtilTests_Windows : PathUtils
	{
		[OneTimeSetUp]
		public static void SetUpUnixSeparators()
		{
			PathUtils.DirectorySeparatorChar = '\\';
			PathUtils.AltDirectorySeparatorChar = '/';
		}

		[OneTimeTearDown]
		public static void RestoreDefaultSeparators()
		{
			PathUtils.DirectorySeparatorChar = System.IO.Path.DirectorySeparatorChar;
			PathUtils.AltDirectorySeparatorChar = System.IO.Path.AltDirectorySeparatorChar;
		}

		[Test]
		public void Canonicalize()
		{
			Assert.AreEqual( @"C:\folder1\file.tmp",
				PathUtils.Canonicalize( @"C:\folder1\.\folder2\..\file.tmp" ) );
			Assert.AreEqual( @"folder1\file.tmp",
				PathUtils.Canonicalize( @"folder1\.\folder2\..\file.tmp" ) );
			Assert.AreEqual( @"folder1\file.tmp", 
				PathUtils.Canonicalize( @"folder1\folder2\.\..\file.tmp" ) );
			Assert.AreEqual( @"file.tmp", 
				PathUtils.Canonicalize( @"folder1\folder2\..\.\..\file.tmp" ) );
			Assert.AreEqual( @"file.tmp", 
				PathUtils.Canonicalize( @"folder1\folder2\..\..\..\file.tmp" ) );
		}

		[Test]
		[Platform(Exclude="Linux")]
		public void RelativePath()
		{
			Assert.AreEqual( @"folder2\folder3", PathUtils.RelativePath( 
				@"c:\folder1", @"c:\folder1\folder2\folder3" ) );
			Assert.AreEqual( @"..\folder2\folder3", PathUtils.RelativePath(
				@"c:\folder1", @"c:\folder2\folder3" ) );
			Assert.AreEqual( @"bin\debug", PathUtils.RelativePath(
				@"c:\folder1", @"bin\debug" ) );
			Assert.IsNull( PathUtils.RelativePath( @"C:\folder", @"D:\folder" ),
				"Unrelated paths should return null" );
            Assert.IsNull(PathUtils.RelativePath(@"C:\", @"D:\"),
                "Unrelated roots should return null");
            Assert.IsNull(PathUtils.RelativePath(@"C:", @"D:"),
                "Unrelated roots (no trailing separators) should return null");
            Assert.AreEqual(string.Empty,
                PathUtils.RelativePath(@"C:\folder1", @"C:\folder1"));
            Assert.AreEqual(string.Empty,
                PathUtils.RelativePath(@"C:\", @"C:\"));

            // First filePath consisting just of a root:
            Assert.AreEqual(@"folder1\folder2", PathUtils.RelativePath(
                @"C:\", @"C:\folder1\folder2"));
            
            // Trailing directory separator in first filePath shall be ignored:
            Assert.AreEqual(@"folder2\folder3", PathUtils.RelativePath(
                @"c:\folder1\", @"c:\folder1\folder2\folder3"));
            
            // Case-insensitive behavior, preserving 2nd filePath directories in result:
            Assert.AreEqual(@"Folder2\Folder3", PathUtils.RelativePath(
                @"C:\folder1", @"c:\folder1\Folder2\Folder3"));
            Assert.AreEqual(@"..\Folder2\folder3", PathUtils.RelativePath(
                @"c:\folder1", @"C:\Folder2\folder3"));
        }

		[Test]
		public void SamePathOrUnder()
		{
			Assert.SamePathOrUnder( @"C:\folder1\folder2\folder3", @"c:\folder1\.\folder2\junk\..\folder3" );
			Assert.SamePathOrUnder( @"C:\folder1\folder2\", @"c:\folder1\.\folder2\junk\..\folder3" );
			Assert.SamePathOrUnder( @"C:\folder1\folder2", @"c:\folder1\.\folder2\junk\..\folder3" );
			Assert.SamePathOrUnder( @"C:\folder1\folder2", @"c:\folder1\.\Folder2\junk\..\folder3" );
			Assert.NotSamePathOrUnder( @"C:\folder1\folder2", @"c:\folder1\.\folder22\junk\..\folder3" );
			Assert.NotSamePathOrUnder( @"C:\folder1\folder2ile.tmp", @"D:\folder1\.\folder2\folder3\file.tmp" );
			Assert.NotSamePathOrUnder( @"C:\", @"D:\" );
			Assert.SamePathOrUnder( @"C:\", @"c:\" );
			Assert.SamePathOrUnder( @"C:\", @"c:\bin\debug" );

		}
	}

	[TestFixture]
	public class PathUtilTests_Unix : PathUtils
	{
		[OneTimeSetUp]
		public static void SetUpUnixSeparators()
		{
			PathUtils.DirectorySeparatorChar = '/';
			PathUtils.AltDirectorySeparatorChar = '\\';
		}

		[OneTimeTearDown]
		public static void RestoreDefaultSeparators()
		{
			PathUtils.DirectorySeparatorChar = System.IO.Path.DirectorySeparatorChar;
			PathUtils.AltDirectorySeparatorChar = System.IO.Path.AltDirectorySeparatorChar;
		}

		[Test]
		public void Canonicalize()
		{
			Assert.AreEqual( "/folder1/file.tmp",
				PathUtils.Canonicalize( "/folder1/./folder2/../file.tmp" ) );
			Assert.AreEqual( "folder1/file.tmp",
				PathUtils.Canonicalize( "folder1/./folder2/../file.tmp" ) );
			Assert.AreEqual( "folder1/file.tmp", 
				PathUtils.Canonicalize( "folder1/folder2/./../file.tmp" ) );
			Assert.AreEqual( "file.tmp", 
				PathUtils.Canonicalize( "folder1/folder2/.././../file.tmp" ) );
			Assert.AreEqual( "file.tmp", 
				PathUtils.Canonicalize( "folder1/folder2/../../../file.tmp" ) );
		}

		[Test]
		public void RelativePath()
		{
			Assert.AreEqual( "folder2/folder3", 
				PathUtils.RelativePath(	"/folder1", "/folder1/folder2/folder3" ) );
			Assert.AreEqual( "../folder2/folder3", 
				PathUtils.RelativePath( "/folder1", "/folder2/folder3" ) );
			Assert.AreEqual( "bin/debug", 
				PathUtils.RelativePath( "/folder1", "bin/debug" ) );
			Assert.AreEqual( "../other/folder", 
				PathUtils.RelativePath( "/folder", "/other/folder" ) );
			Assert.AreEqual( "../../d",
				PathUtils.RelativePath( "/a/b/c", "/a/d" ) );
            Assert.AreEqual(string.Empty,
                PathUtils.RelativePath("/a/b", "/a/b"));
            Assert.AreEqual(string.Empty,
                PathUtils.RelativePath("/", "/"));
            
            // First filePath consisting just of a root:
            Assert.AreEqual("folder1/folder2", PathUtils.RelativePath(
                "/", "/folder1/folder2"));
            
            // Trailing directory separator in first filePath shall be ignored:
            Assert.AreEqual("folder2/folder3", PathUtils.RelativePath(
                "/folder1/", "/folder1/folder2/folder3"));
            
            // Case-sensitive behavior:
            Assert.AreEqual("../Folder1/Folder2/folder3",
                PathUtils.RelativePath("/folder1", "/Folder1/Folder2/folder3"),
                "folders differing in case");
        }

		[Test]
		public void SamePathOrUnder()
		{
			Assert.SamePathOrUnder( "/folder1/folder2/folder3", "/folder1/./folder2/junk/../folder3" );
			Assert.SamePathOrUnder( "/folder1/folder2/", "/folder1/./folder2/junk/../folder3" );
			Assert.SamePathOrUnder( "/folder1/folder2", "/folder1/./folder2/junk/../folder3" );
			Assert.NotSamePathOrUnder( "/folder1/folder2", "/folder1/./Folder2/junk/../folder3" );
			Assert.NotSamePathOrUnder( "/folder1/folder2", "/folder1/./folder22/junk/../folder3" );
			Assert.SamePathOrUnder( "/", "/" );
			Assert.SamePathOrUnder( "/", "/bin/debug" );
		}
	}
}
