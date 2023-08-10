// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.IO;

namespace NUnit.Framework.Tests.Syntax
{
    public class SamePathTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";
            string defaultCaseSensitivity = Path.DirectorySeparatorChar == '\\'
                ? "ignorecase" : "respectcase";

            ParseTree = $@"<samepath ""{path}"" {defaultCaseSensitivity}>";
            StaticSyntax = Is.SamePath(path);
            BuilderSyntax = Builder().SamePath(path);
        }
    }

    public class SamePathTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = $@"<samepath ""{path}"" ignorecase>";
            StaticSyntax = Is.SamePath(path).IgnoreCase;
            BuilderSyntax = Builder().SamePath(path).IgnoreCase;
        }
    }

    public class NotSamePathTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = $@"<not <samepath ""{path}"" ignorecase>>";
            StaticSyntax = Is.Not.SamePath(path).IgnoreCase;
            BuilderSyntax = Builder().Not.SamePath(path).IgnoreCase;
        }
    }

    public class SamePathTest_RespectCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = $@"<samepath ""{path}"" respectcase>";
            StaticSyntax = Is.SamePath(path).RespectCase;
            BuilderSyntax = Builder().SamePath(path).RespectCase;
        }
    }

    public class NotSamePathTest_RespectCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = $@"<not <samepath ""{path}"" respectcase>>";
            StaticSyntax = Is.Not.SamePath(path).RespectCase;
            BuilderSyntax = Builder().Not.SamePath(path).RespectCase;
        }
    }

    public class SamePathOrUnderTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";
            string defaultCaseSensitivity = Path.DirectorySeparatorChar == '\\'
                ? "ignorecase" : "respectcase";

            ParseTree = $@"<samepathorunder ""{path}"" {defaultCaseSensitivity}>";
            StaticSyntax = Is.SamePathOrUnder(path);
            BuilderSyntax = Builder().SamePathOrUnder(path);
        }
    }

    public class SamePathOrUnderTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = $@"<samepathorunder ""{path}"" ignorecase>";
            StaticSyntax = Is.SamePathOrUnder(path).IgnoreCase;
            BuilderSyntax = Builder().SamePathOrUnder(path).IgnoreCase;
        }
    }

    public class NotSamePathOrUnderTest_IgnoreCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = $@"<not <samepathorunder ""{path}"" ignorecase>>";
            StaticSyntax = Is.Not.SamePathOrUnder(path).IgnoreCase;
            BuilderSyntax = Builder().Not.SamePathOrUnder(path).IgnoreCase;
        }
    }

    public class SamePathOrUnderTest_RespectCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = $@"<samepathorunder ""{path}"" respectcase>";
            StaticSyntax = Is.SamePathOrUnder(path).RespectCase;
            BuilderSyntax = Builder().SamePathOrUnder(path).RespectCase;
        }
    }

    public class NotSamePathOrUnderTest_RespectCase : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            string path = "/path/to/match";

            ParseTree = $@"<not <samepathorunder ""{path}"" respectcase>>";
            StaticSyntax = Is.Not.SamePathOrUnder(path).RespectCase;
            BuilderSyntax = Builder().Not.SamePathOrUnder(path).RespectCase;
        }
    }
}
