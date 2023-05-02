// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using System.Collections.Generic;

namespace NUnit.Framework.Syntax
{
    // NOTE: The tests in this file ensure that the various
    // syntactic elements work together to create a
    // DelayedConstraint object with the proper values.

    public class AfterTest_SimpleConstraint : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<after 1000 <equal 10>>";
            StaticSyntax = Is.EqualTo(10).After(1000);
            BuilderSyntax = Builder().EqualTo(10).After(1000);
        }
    }

    public class AfterMinutesTest_SimpleConstraint : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<after 60000 <equal 10>>";
            StaticSyntax = Is.EqualTo(10).After(1).Minutes;
            BuilderSyntax = Builder().EqualTo(10).After(1).Minutes;
        }
    }

    public class AfterSecondsTest_SimpleConstraint : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<after 20000 <equal 10>>";
            StaticSyntax = Is.EqualTo(10).After(20).Seconds;
            BuilderSyntax = Builder().EqualTo(10).After(20).Seconds;
        }
    }

    public class AfterMillisecondsTest_SimpleConstraint : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<after 500 <equal 10>>";
            StaticSyntax = Is.EqualTo(10).After(500).MilliSeconds;
            BuilderSyntax = Builder().EqualTo(10).After(500).MilliSeconds;
        }
    }


    public class AfterTest_PropertyTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<after 1000 <property X <equal 10>>>";
            StaticSyntax = Has.Property("X").EqualTo(10).After(1000);
            BuilderSyntax = Builder().Property("X").EqualTo(10).After(1000);
        }
    }

    public class AfterTest_AndOperator : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<after 1000 <and <greaterthan 0> <lessthan 10>>>";
            StaticSyntax = Is.GreaterThan(0).And.LessThan(10).After(1000);
            BuilderSyntax = Builder().GreaterThan(0).And.LessThan(10).After(1000);
        }
    }

    public abstract class AfterSyntaxTests
    {
        protected bool Flag;
        protected int Num;
        protected object? Ob1, Ob2, Ob3;
        protected List<object> List;
        protected string Greeting;

        [SetUp]
        public void InitializeValues()
        {
            this.Flag = false;
            this.Num = 0;
            this.Ob1 = new object();
            this.Ob2 = new object();
            this.Ob3 = new object();
            this.List = new List<object>();
            this.List.Add(1);
            this.List.Add(2);
            this.List.Add(3);
            this.Greeting = "hello";

            new Thread(ModifyValuesAfterDelay).Start();
        }

        private void ModifyValuesAfterDelay()
        {
            Thread.Sleep(100);

            this.Flag = true;
            this.Num = 1;
            this.Ob1 = Ob2;
            this.Ob3 = null;
            this.List.Add(4);
            this.Greeting += "world";
        }
    }

    public class AfterSyntaxUsingAnonymousDelegates : AfterSyntaxTests
    {
        [Test]
        public void TrueTest()
        {
            Assert.That(delegate { return Flag; }, Is.True.After(5000, 200));
        }

        [Test]
        public void EqualToTest()
        {
            Assert.That(delegate { return Num; }, Is.EqualTo(1).After(5000, 200));
        }

        [Test]
        public void SameAsTest()
        {
            Assert.That(delegate { return Ob1; }, Is.SameAs(Ob2).After(5000, 200));
        }

        [Test]
        public void GreaterTest()
        {
            Assert.That(delegate { return Num; }, Is.GreaterThan(0).After(5000,200));
        }

        [Test]
        public void HasMemberTest()
        {
            Assert.That(delegate { return List; }, Has.Member(4).After(5000, 200));
        }

        [Test]
        public void NullTest()
        {
            Assert.That(delegate { return Ob3; }, Is.Null.After(5000, 200));
        }

        [Test]
        public void TextTest()
        {
            Assert.That(delegate { return Greeting; }, Does.EndWith("world").After(5000, 200));
        }
    }

    public class AfterSyntaxUsingLambda : AfterSyntaxTests
    {
        [Test]
        public void TrueTest()
        {
            Assert.That(() => Flag, Is.True.After(5000, 200));
        }

        [Test]
        public void EqualToTest()
        {
            Assert.That(() => Num, Is.EqualTo(1).After(5000, 200));
        }

        [Test]
        public void SameAsTest()
        {
            Assert.That(() => Ob1, Is.SameAs(Ob2).After(5000, 200));
        }

        [Test]
        public void GreaterTest()
        {
            Assert.That(() => Num, Is.GreaterThan(0).After(5000, 200));
        }

        [Test]
        public void HasMemberTest()
        {
            Assert.That(() => List, Has.Member(4).After(5000, 200));
        }

        [Test]
        public void NullTest()
        {
            Assert.That(() => Ob3, Is.Null.After(5000, 200));
        }

        [Test]
        public void TextTest()
        {
            Assert.That(() => Greeting, Does.EndWith("world").After(5000, 200));
        }
    }
}
