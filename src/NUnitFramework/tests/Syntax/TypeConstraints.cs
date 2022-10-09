// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Syntax
{
    [TestFixture]
    public class ExactTypeTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<typeof System.String>";
            StaticSyntax = Is.TypeOf(typeof(string));
            BuilderSyntax = Builder().TypeOf(typeof(string));
        }
    }

    [TestFixture]
    public class InstanceOfTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<instanceof System.String>";
            StaticSyntax = Is.InstanceOf(typeof(string));
            BuilderSyntax = Builder().InstanceOf(typeof(string));
        }
    }

    [TestFixture]
    public class AssignableFromTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<assignablefrom System.String>";
            StaticSyntax = Is.AssignableFrom(typeof(string));
            BuilderSyntax = Builder().AssignableFrom(typeof(string));
        }
    }

    [TestFixture]
    public class AssignableToTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<assignableto System.String>";
            StaticSyntax = Is.AssignableTo(typeof(string));
            BuilderSyntax = Builder().AssignableTo(typeof(string));
        }
    }

    [TestFixture]
    public class AttributeTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<attributeexists NUnit.Framework.TestFixtureAttribute>";
            StaticSyntax = Has.Attribute(typeof(TestFixtureAttribute));
            BuilderSyntax = Builder().Attribute(typeof(TestFixtureAttribute));
        }
    }

    [TestFixture]
    public class AttributeTestWithFollowingConstraint : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = @"<attribute NUnit.Framework.TestFixtureAttribute <property Description <not <null>>>>";
            StaticSyntax = Has.Attribute(typeof(TestFixtureAttribute)).Property("Description").Not.Null;
            BuilderSyntax = Builder().Attribute(typeof(TestFixtureAttribute)).Property("Description").Not.Null;
        }
    }

    [TestFixture]
    public class ExactTypeTest_Generic : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<typeof System.String>";
            StaticSyntax = Is.TypeOf<string>();
            BuilderSyntax = Builder().TypeOf<string>();
        }
    }

    [TestFixture]
    public class InstanceOfTest_Generic : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<instanceof System.String>";
            StaticSyntax = Is.InstanceOf<string>();
            BuilderSyntax = Builder().InstanceOf<string>();
        }
    }

    [TestFixture]
    public class AssignableFromTest_Generic : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<assignablefrom System.String>";
            StaticSyntax = Is.AssignableFrom<string>();
            BuilderSyntax = Builder().AssignableFrom<string>();
        }
    }

    [TestFixture]
    public class AssignableToTest_Generic : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<assignableto System.String>";
            StaticSyntax = Is.AssignableTo<string>();
            BuilderSyntax = Builder().AssignableTo<string>();
        }
    }

    [TestFixture]
    public class AttributeTest_Generic : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<attributeexists NUnit.Framework.TestFixtureAttribute>";
            StaticSyntax = Has.Attribute<TestFixtureAttribute>();
            BuilderSyntax = Builder().Attribute<TestFixtureAttribute>();
        }
    }
}
