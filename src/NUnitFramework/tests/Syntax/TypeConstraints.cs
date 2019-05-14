// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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
