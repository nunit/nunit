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
using System.Collections;

namespace NUnit.Framework.Syntax
{
    public class PropertyExistsTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<propertyexists X>";
            StaticSyntax = Has.Property("X");
            BuilderSyntax = Builder().Property("X");
        }
    }

    public class PropertyExistsTest_AndFollows : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<and <propertyexists X> <equal 7>>";
            StaticSyntax = Has.Property("X").And.EqualTo(7);
            BuilderSyntax = Builder().Property("X").And.EqualTo(7);
        }
    }

    public class PropertyTest_ConstraintFollows : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<property X <greaterthan 5>>";
            StaticSyntax = Has.Property("X").GreaterThan(5);
            BuilderSyntax = Builder().Property("X").GreaterThan(5);
        }
    }

    public class PropertyTest_NotFollows : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<property X <not <greaterthan 5>>>";
            StaticSyntax = Has.Property("X").Not.GreaterThan(5);
            BuilderSyntax = Builder().Property("X").Not.GreaterThan(5);
        }
    }

    public class LengthTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<property Length <greaterthan 5>>";
            StaticSyntax = Has.Length.GreaterThan(5);
            BuilderSyntax = Builder().Length.GreaterThan(5);
        }
    }

    public class CountTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<property Count <equal 5>>";
            StaticSyntax = Has.Count.EqualTo(5);
            BuilderSyntax = Builder().Count.EqualTo(5);
        }
    }

    public class MessageTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = @"<property Message <startswith ""Expected"">>";
            StaticSyntax = Has.Message.StartsWith("Expected");
            BuilderSyntax = Builder().Message.StartsWith("Expected");
        }
    }

    public class PropertySyntaxVariations
    {
        private readonly int[] ints = new int[] { 1, 2, 3 };

        [Test]
        public void ExistenceTest()
        {
            Assert.That(ints, Has.Property("Length"));
            Assert.That(ints, Has.Length);
        }

        [Test]
        public void SeparateConstraintTest()
        {
            Assert.That(ints, Has.Property("Length").EqualTo(3));
            Assert.That(ints, Has.Length.EqualTo(3));
        }
    }
}
