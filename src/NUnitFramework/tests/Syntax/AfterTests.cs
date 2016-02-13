// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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

#if !PORTABLE
using System;
using System.Threading;
using System.Collections.Generic;

namespace NUnit.Framework.Syntax
{
    public class AfterTest_SimpleConstraint : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<after 1000 <equal 10>>";
            staticSyntax = Is.EqualTo(10).After(1000);
            builderSyntax = Builder().EqualTo(10).After(1000);
        }
    }

    public class AfterTest_PropertyTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<after 1000 <property X <equal 10>>>";
            staticSyntax = Has.Property("X").EqualTo(10).After(1000);
            builderSyntax = Builder().Property("X").EqualTo(10).After(1000);
        }
    }

    public class AfterTest_AndOperator : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            parseTree = "<after 1000 <and <greaterthan 0> <lessthan 10>>>";
            staticSyntax = Is.GreaterThan(0).And.LessThan(10).After(1000);
            builderSyntax = Builder().GreaterThan(0).And.LessThan(10).After(1000);
        }
    }

    public abstract class AfterSyntaxTests
    {
        protected bool flag;
        protected int num;
        protected object ob1, ob2, ob3;
        protected List<object> list;
        protected string greeting;

        [SetUp]
        public void InitializeValues()
        {
            this.flag = false;
            this.num = 0;
            this.ob1 = new object();
            this.ob2 = new object();
            this.ob3 = new object();
            this.list = new List<object>();
            this.list.Add(1);
            this.list.Add(2);
            this.list.Add(3);
            this.greeting = "hello";

            new Thread(ModifyValuesAfterDelay).Start();
        }

        private void ModifyValuesAfterDelay()
        {
            Thread.Sleep(100);

            this.flag = true;
            this.num = 1;
            this.ob1 = ob2;
            this.ob3 = null;
            this.list.Add(4);
            this.greeting += "world";
        }
    }

    public class AfterSyntaxUsingAnonymousDelegates : AfterSyntaxTests
    {
        [Test]
        public void TrueTest()
        {
            Assert.That(delegate { return flag; }, Is.True.After(5000, 200));
        }

        [Test]
        public void EqualToTest()
        {
            Assert.That(delegate { return num; }, Is.EqualTo(1).After(5000, 200));
        }

        [Test]
        public void SameAsTest()
        {
            Assert.That(delegate { return ob1; }, Is.SameAs(ob2).After(5000, 200));
        }

        [Test]
        public void GreaterTest()
        {
            Assert.That(delegate { return num; }, Is.GreaterThan(0).After(5000,200));
        }

        [Test]
        public void HasMemberTest()
        {
            Assert.That(delegate { return list; }, Has.Member(4).After(5000, 200));
        }

        [Test]
        public void NullTest()
        {
            Assert.That(delegate { return ob3; }, Is.Null.After(5000, 200));
        }

        [Test]
        public void TextTest()
        {
            Assert.That(delegate { return greeting; }, Does.EndWith("world").After(5000, 200));
        }
    }

    public class AfterSyntaxUsingLambda : AfterSyntaxTests
    {
        [Test]
        public void TrueTest()
        {
            Assert.That(() => flag, Is.True.After(5000, 200));
        }

        [Test]
        public void EqualToTest()
        {
            Assert.That(() => num, Is.EqualTo(1).After(5000, 200));
        }

        [Test]
        public void SameAsTest()
        {
            Assert.That(() => ob1, Is.SameAs(ob2).After(5000, 200));
        }

        [Test]
        public void GreaterTest()
        {
            Assert.That(() => num, Is.GreaterThan(0).After(5000, 200));
        }

        [Test]
        public void HasMemberTest()
        {
            Assert.That(() => list, Has.Member(4).After(5000, 200));
        }

        [Test]
        public void NullTest()
        {
            Assert.That(() => ob3, Is.Null.After(5000, 200));
        }

        [Test]
        public void TextTest()
        {
            Assert.That(() => greeting, Does.EndWith("world").After(5000, 200));
        }
    }
}
#endif