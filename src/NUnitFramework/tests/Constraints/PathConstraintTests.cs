// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Summary description for PathConstraintTests.
    /// </summary>]
    [TestFixture, Platform("Win")]
    public class SamePathTest_Windows : StringConstraintTests
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new SamePathConstraint( @"C:\folder1\file.tmp" ).IgnoreCase;
            ExpectedDescription = @"Path matching ""C:\folder1\file.tmp""";
            StringRepresentation = "<samepath \"C:\\folder1\\file.tmp\" ignorecase>";
        }

        static object[] SuccessData = new object[]
            { 
                @"C:\folder1\file.tmp", 
                @"C:\Folder1\File.TMP",
                @"C:\folder1\.\file.tmp",
                @"C:\folder1\folder2\..\file.tmp",
                @"C:\FOLDER1\.\folder2\..\File.TMP",
                @"C:/folder1/file.tmp"
            };
        static object[] FailureData = new object[]
            { 
                new TestCaseData( @"C:\folder2\file.tmp", "\"C:\\folder2\\file.tmp\"" ),
                new TestCaseData( @"C:\folder1\.\folder2\..\file.temp", "\"C:\\folder1\\.\\folder2\\..\\file.temp\"" )
            };

        [Test]
        public void RootPathEquality()
        {
            Assert.That("c:\\", Is.SamePath("C:\\junk\\..\\").IgnoreCase);
        }
    }

    [TestFixture, Platform("Unix")]
    public class SamePathTest_Linux : StringConstraintTests
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new SamePathConstraint(@"/folder1/folder2").RespectCase;
            ExpectedDescription = @"Path matching ""/folder1/folder2""";
            StringRepresentation = @"<samepath ""/folder1/folder2"" respectcase>";
        }

        static object[] SuccessData = new object[]
            { 
                @"/folder1/folder2", 
                @"/folder1/folder2/",
                @"/folder1/./folder2",
                @"/folder1/./folder2/",
                @"/folder1/junk/../folder2",
                @"/folder1/junk/../folder2/",
                @"/folder1/./junk/../folder2",
                @"/folder1/./junk/../folder2/",
                @"\folder1\folder2",
                @"\folder1\folder2\"
            };
        static object[] FailureData = new object[]
            { 
                new TestCaseData( "folder1/folder2", "\"folder1/folder2\""),
                new TestCaseData( "//folder1/folder2", "\"//folder1/folder2\""),
                new TestCaseData( @"/junk/folder2", "\"/junk/folder2\"" ),
                new TestCaseData( @"/folder1/./junk/../file.temp", "\"/folder1/./junk/../file.temp\"" ),
                new TestCaseData( @"/Folder1/FOLDER2", "\"/Folder1/FOLDER2\"" ),
                new TestCaseData( @"/FOLDER1/./junk/../FOLDER2", "\"/FOLDER1/./junk/../FOLDER2\"" )
            };

        [Test]
        public void RootPathEquality()
        {
            Assert.That("/", Is.SamePath("/junk/../"));
        }
    }

    [TestFixture, Platform("Win")]
    public class SubPathTest_Windows : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new SubPathConstraint(@"C:\folder1\folder2").IgnoreCase;
            ExpectedDescription = @"Subpath of ""C:\folder1\folder2""";
            StringRepresentation = @"<subpath ""C:\folder1\folder2"" ignorecase>";
        }

        static object[] SuccessData = new object[]
            {
                @"C:\folder1\folder2\folder3",
                @"C:\folder1\.\folder2\folder3",
                @"C:\folder1\junk\..\folder2\folder3",
                @"C:\FOLDER1\.\junk\..\Folder2\temp\..\Folder3",
                @"C:/folder1/folder2/folder3",
            };
        static object[] FailureData = new object[]
            {
                new TestCaseData(@"C:\folder1\folder3", "\"C:\\folder1\\folder3\""),
                new TestCaseData(@"C:\folder1\.\folder2\..\file.temp", "\"C:\\folder1\\.\\folder2\\..\\file.temp\""),
                new TestCaseData(@"C:\folder1\folder2", "\"C:\\folder1\\folder2\""),
                new TestCaseData(@"C:\Folder1\Folder2", "\"C:\\Folder1\\Folder2\""),
                new TestCaseData(@"C:\folder1\.\folder2", "\"C:\\folder1\\.\\folder2\""),
                new TestCaseData(@"C:\folder1\junk\..\folder2", "\"C:\\folder1\\junk\\..\\folder2\""),
                new TestCaseData(@"C:\FOLDER1\.\junk\..\Folder2", "\"C:\\FOLDER1\\.\\junk\\..\\Folder2\""),
                new TestCaseData(@"C:/folder1/folder2", "\"C:/folder1/folder2\"")
            };

        [Test]
        public void SubPathOfRoot()
        {
            Assert.That("C:\\junk\\file.temp", new SubPathConstraint("C:\\"));
        }
    }

    [TestFixture, Platform("Unix")]
    public class SubPathTest_Linux : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new SubPathConstraint(@"/folder1/folder2").RespectCase;
            ExpectedDescription = @"Subpath of ""/folder1/folder2""";
            StringRepresentation = @"<subpath ""/folder1/folder2"" respectcase>";
        }

        static object[] SuccessData = new object[]
            {
                @"/folder1/folder2/folder3",
                @"/folder1/./folder2/folder3",
                @"/folder1/junk/../folder2/folder3",
                @"\folder1\folder2\folder3",
            };
        static object[] FailureData = new object[]
            {
                new TestCaseData("/Folder1/Folder2", "\"/Folder1/Folder2\""),
                new TestCaseData("/FOLDER1/./junk/../Folder2", "\"/FOLDER1/./junk/../Folder2\""),
                new TestCaseData("/FOLDER1/./junk/../Folder2/temp/../Folder3", "\"/FOLDER1/./junk/../Folder2/temp/../Folder3\""),
                new TestCaseData("/folder1/folder3", "\"/folder1/folder3\""),
                new TestCaseData("/folder1/./folder2/../folder3", "\"/folder1/./folder2/../folder3\""),
                new TestCaseData("/folder1", "\"/folder1\""),
                new TestCaseData("/folder1/folder2", "\"/folder1/folder2\""),
                new TestCaseData("/folder1/./folder2", "\"/folder1/./folder2\""),
                new TestCaseData("/folder1/junk/../folder2", "\"/folder1/junk/../folder2\""),
                new TestCaseData(@"\folder1\folder2", "\"\\folder1\\folder2\"")
            };

        [Test]
        public void SubPathOfRoot()
        {
            Assert.That("/junk/file.temp", new SubPathConstraint("/"));
        }
    }

    [TestFixture, Platform("Win")]
    public class SamePathOrUnderTest_Windows : StringConstraintTests
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new SamePathOrUnderConstraint( @"C:\folder1\folder2" ).IgnoreCase;
            ExpectedDescription = @"Path under or matching ""C:\folder1\folder2""";
            StringRepresentation = @"<samepathorunder ""C:\folder1\folder2"" ignorecase>";
        }

        static object[] SuccessData = new object[]
            {
                @"C:\folder1\folder2",
                @"C:\Folder1\Folder2",
                @"C:\folder1\.\folder2",
                @"C:\folder1\junk\..\folder2",
                @"C:\FOLDER1\.\junk\..\Folder2",
                @"C:/folder1/folder2",
                @"C:\folder1\folder2\folder3",
                @"C:\folder1\.\folder2\folder3",
                @"C:\folder1\junk\..\folder2\folder3",
                @"C:\FOLDER1\.\junk\..\Folder2\temp\..\Folder3",
                @"C:/folder1/folder2/folder3",
            };
        static object[] FailureData = new object[]
            {
                new TestCaseData( @"C:\folder1\folder3", "\"C:\\folder1\\folder3\"" ),
                new TestCaseData( @"C:\folder1\.\folder2\..\file.temp", "\"C:\\folder1\\.\\folder2\\..\\file.temp\"" )
            };
    }

    [TestFixture, Platform("Unix")]
    public class SamePathOrUnderTest_Linux : StringConstraintTests
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new SamePathOrUnderConstraint( @"/folder1/folder2"  ).RespectCase;
            ExpectedDescription = @"Path under or matching ""/folder1/folder2""";
            StringRepresentation = @"<samepathorunder ""/folder1/folder2"" respectcase>";
        }

        static object[] SuccessData = new object[]
            {
                @"/folder1/folder2",
                @"/folder1/./folder2",
                @"/folder1/junk/../folder2",
                @"\folder1\folder2",
                @"/folder1/folder2/folder3",
                @"/folder1/./folder2/folder3",
                @"/folder1/junk/../folder2/folder3",
                @"\folder1\folder2\folder3",
            };
        static object[] FailureData = new object[]
            {
                new TestCaseData( "/Folder1/Folder2", "\"/Folder1/Folder2\"" ),
                new TestCaseData( "/FOLDER1/./junk/../Folder2", "\"/FOLDER1/./junk/../Folder2\"" ),
                new TestCaseData( "/FOLDER1/./junk/../Folder2/temp/../Folder3", "\"/FOLDER1/./junk/../Folder2/temp/../Folder3\"" ),
                new TestCaseData( "/folder1/folder3", "\"/folder1/folder3\"" ),
                new TestCaseData( "/folder1/./folder2/../folder3", "\"/folder1/./folder2/../folder3\"" ),
                new TestCaseData( "/folder1", "\"/folder1\"" )
            };
    }
}
