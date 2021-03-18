// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Syntax
{
    #region Not
    public class NotTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<not <null>>";
            StaticSyntax = Is.Not.Null;
            BuilderSyntax = Builder().Not.Null;
        }
    }

    public class NotTest_Cascaded : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<not <not <not <null>>>>";
            StaticSyntax = Is.Not.Not.Not.Null;
            BuilderSyntax = Builder().Not.Not.Not.Null;
        }
    }
    #endregion

    #region All
    public class AllTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<all <greaterthan 0>>";
            StaticSyntax = Is.All.GreaterThan(0);
            BuilderSyntax = Builder().All.GreaterThan(0);
        }
    }
    #endregion

    #region Some
    public class SomeTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<some <equal 3>>";
            StaticSyntax = Has.Some.EqualTo(3);
            BuilderSyntax = Builder().Some.EqualTo(3);
        }
    }

    public class SomeTest_BeforeBinaryOperators : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<some <or <and <greaterthan 0> <lessthan 100>> <equal 999>>>";
            StaticSyntax = Has.Some.GreaterThan(0).And.LessThan(100).Or.EqualTo(999);
            BuilderSyntax = Builder().Some.GreaterThan(0).And.LessThan(100).Or.EqualTo(999);
        }
    }

    public class SomeTest_NestedSome : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<some <some <lessthan 100>>>";
            StaticSyntax = Has.Some.With.Some.LessThan(100);
            BuilderSyntax = Builder().Some.With.Some.LessThan(100);
        }
        
    }

    public class SomeTest_UseOfAndSome : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<and <some <greaterthan 0>> <some <lessthan 100>>>";
            StaticSyntax = Has.Some.GreaterThan(0).And.Some.LessThan(100);
            BuilderSyntax = Builder().Some.GreaterThan(0).And.Some.LessThan(100);
        }
    }
    #endregion

    #region None
    public class NoneTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<none <lessthan 0>>";
            StaticSyntax = Has.None.LessThan(0);
            BuilderSyntax = Builder().None.LessThan(0);
        }
    }
    #endregion

    #region Exactly
    public class Exactly_WithoutConstraint : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<exactcount>";
            StaticSyntax = Has.Exactly(3).Items;
            BuilderSyntax = Builder().Exactly(3).Items;
        }
    }

    public class Exactly_WithConstraint : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<exactcount <lessthan 0>>";
            StaticSyntax = Has.Exactly(3).Items.LessThan(0);
            BuilderSyntax = Builder().Exactly(3).Items.LessThan(0);
        }
    }

    public class Exactly_WithConstraint_BeforeBinaryOperators : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<exactcount <or <lessthan 0> <and <greaterthan 10> <lessthan 20>>>>";
            StaticSyntax = Has.Exactly(3).Items.LessThan(0).Or.GreaterThan(10).And.LessThan(20);
            BuilderSyntax = Builder().Exactly(3).Items.LessThan(0).Or.GreaterThan(10).And.LessThan(20);
        }
    }

    public class Exactly_WithConstraint_BeforeAndAfterBinaryOperators : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<and <exactcount <lessthan 0>> <exactcount <greaterthan 10>>>";
            StaticSyntax = Has.Exactly(3).Items.LessThan(0).And.Exactly(3).Items.GreaterThan(10);
            BuilderSyntax = Builder().Exactly(3).Items.LessThan(0).And.Exactly(3).Items.GreaterThan(10);
        }
    }
    #endregion

    #region One

    public class One_WithoutConstraint : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<exactcount>";
            StaticSyntax = Has.One.Items;
            BuilderSyntax = Builder().One.Items;
        }
    }

    public class One_WithConstraint : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<exactcount <lessthan 0>>";
            StaticSyntax = Has.One.Items.LessThan(0);
            BuilderSyntax = Builder().One.Items.LessThan(0);
        }
    }

    public class One_WithConstraint_BeforeBinaryOperators : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<exactcount <or <lessthan 0> <and <greaterthan 10> <lessthan 20>>>>";
            StaticSyntax = Has.One.Items.LessThan(0).Or.GreaterThan(10).And.LessThan(20);
            BuilderSyntax = Builder().One.Items.LessThan(0).Or.GreaterThan(10).And.LessThan(20);
        }
    }

    public class One_WithConstraint_BeforeAndAfterBinaryOperators : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<and <exactcount <lessthan 0>> <exactcount <greaterthan 10>>>";
            StaticSyntax = Has.One.Items.LessThan(0).And.One.Items.GreaterThan(10);
            BuilderSyntax = Builder().One.Items.LessThan(0).And.One.Items.GreaterThan(10);
        }
    }

    #endregion

    #region And
    public class AndTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<and <greaterthan 5> <lessthan 10>>";
            StaticSyntax = Is.GreaterThan(5).And.LessThan(10);
            BuilderSyntax = Builder().GreaterThan(5).And.LessThan(10);
        }
    }

    public class AndTest_ThreeAndsWithNot : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<and <not <null>> <and <not <lessthan 5>> <not <greaterthan 10>>>>";
            StaticSyntax = Is.Not.Null.And.Not.LessThan(5).And.Not.GreaterThan(10);
            BuilderSyntax = Builder().Not.Null.And.Not.LessThan(5).And.Not.GreaterThan(10);
        }
    }
    #endregion

    #region Or
    public class OrTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<or <lessthan 5> <greaterthan 10>>";
            StaticSyntax = Is.LessThan(5).Or.GreaterThan(10);
            BuilderSyntax = Builder().LessThan(5).Or.GreaterThan(10);
        }
    }

    public class OrTest_ThreeOrs : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<or <lessthan 5> <or <greaterthan 10> <equal 7>>>";
            StaticSyntax = Is.LessThan(5).Or.GreaterThan(10).Or.EqualTo(7);
            BuilderSyntax = Builder().LessThan(5).Or.GreaterThan(10).Or.EqualTo(7);
        }
    }
    #endregion

    #region Binary Operator Precedence
    public class AndIsEvaluatedBeforeFollowingOr : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<or <and <lessthan 100> <greaterthan 0>> <equal 999>>";
            StaticSyntax = Is.LessThan(100).And.GreaterThan(0).Or.EqualTo(999);
            BuilderSyntax = Builder().LessThan(100).And.GreaterThan(0).Or.EqualTo(999);
        }
    }

    public class AndIsEvaluatedBeforePrecedingOr : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<or <equal 999> <and <greaterthan 0> <lessthan 100>>>";
            StaticSyntax = Is.EqualTo(999).Or.GreaterThan(0).And.LessThan(100);
            BuilderSyntax = Builder().EqualTo(999).Or.GreaterThan(0).And.LessThan(100);
        }
    }
    #endregion

    public class OperatorPrecedenceTests
    {
        class A
        {
            B B
            {
                get { return new B(); }
            }

            string X
            {
                get { return "X in A"; }
            }

            string Y
            {
                get { return "Y in A"; }
            }
        }

        class B
        {
            string X
            {
                get { return "X in B"; }
            }

            string Y
            {
                get { return "Y in B"; }
            }
        }

        [Test]
        public void WithTests()
        {
            A a = new A();
            Assert.That(a, Has.Property("X").EqualTo("X in A")
                          .And.Property("Y").EqualTo("Y in A"));
            Assert.That(a, Has.Property("X").EqualTo("X in A")
                          .And.Property("B").Property("X").EqualTo("X in B"));
            Assert.That(a, Has.Property("X").EqualTo("X in A")
                          .And.Property("B").With.Property("X").EqualTo("X in B"));
            Assert.That(a, Has.Property("B").Property("X").EqualTo("X in B")
                          .And.Property("B").Property("Y").EqualTo("Y in B"));
            Assert.That(a, Has.Property("B").Property("X").EqualTo("X in B")
                          .And.Property("B").With.Property("Y").EqualTo("Y in B"));
            Assert.That(a, Has.Property("B").With.Property("X").EqualTo("X in B")
                                            .And.Property("Y").EqualTo("Y in B"));
        }

        [Test]
        public void SomeTests()
        {
            string[] array = new string[] { "a", "aa", "x", "xy", "xyz" };
            //Assert.That(array, Has.Some.StartsWith("a").And.Some.Length.EqualTo(3));
            Assert.That(array, Has.None.StartsWith("a").And.Length.EqualTo(3));
            Assert.That(array, Has.Some.StartsWith("x").And.Length.EqualTo(3));
        }
    }
}
