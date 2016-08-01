// ****************************************************************
// Copyright 2009, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using NUnit.Framework;

namespace NUnit.Core.Tests
{
    [TestFixture("hello", "hello", "goodbye")]
    [TestFixture("zip", "zip")]
    [TestFixture(42, 42, 99)]
    public class ParameterizedTestFixture
    {
        private string eq1;
        private string eq2;
        private string neq;
        
        public ParameterizedTestFixture(string eq1, string eq2, string neq)
        {
            this.eq1 = eq1;
            this.eq2 = eq2;
            this.neq = neq;
        }

        public ParameterizedTestFixture(string eq1, string eq2)
            : this(eq1, eq2, null) { }

        public ParameterizedTestFixture(int eq1, int eq2, int neq)
        {
            this.eq1 = eq1.ToString();
            this.eq2 = eq2.ToString();
            this.neq = neq.ToString();
        }

        [Test]
        public void TestEquality()
        {
            Assert.AreEqual(eq1, eq2);
            if (eq1 != null && eq2 != null)
                Assert.AreEqual(eq1.GetHashCode(), eq2.GetHashCode());
        }

        [Test]
        public void TestInequality()
        {
            Assert.AreNotEqual(eq1, neq);
            if (eq1 != null && neq != null)
                Assert.AreNotEqual(eq1.GetHashCode(), neq.GetHashCode());
        }
    }

#if CLR_2_0 || CLR_4_0
    [TestFixture("A", null)]
    [TestFixture(null, "A")]
    [TestFixture(null, null)]
    public class ParameterizedTestFixtureWithNullArguments
    {
        string a;
        string b;

        public ParameterizedTestFixtureWithNullArguments(string a, string b)
        {
            this.a = a;
            this.b = b;
        }

        [Test]
        public void TestMethod() 
        {
            Assert.That(a == null || b == null);
        }
    }
#endif

    [TestFixture(42)]
    public class ParameterizedTestFixtureWithDataSources
    {
        private int answer;

        internal object[] myData = { new int[] { 6, 7 }, new int[] { 3, 14 } };

        public ParameterizedTestFixtureWithDataSources(int val)
        {
            this.answer = val;
        }

        [Test, TestCaseSource("myData")]
        public void CanAccessTestCaseSource(int x, int y)
        {
            Assert.That(x * y, Is.EqualTo(answer));
        }

        IEnumerable GenerateData()
        {
            for(int i = 1; i <= answer; i++)
                if ( answer%i == 0 )
                    yield return new int[] { i, answer/i  };
        }

        [Test, TestCaseSource("GenerateData")]
        public void CanGenerateDataFromParameter(int x, int y)
        {
            Assert.That(x * y, Is.EqualTo(answer));
        }

        internal int[] intvals = new int[] { 1, 2, 3 };

        [Test]
        public void CanAccessValueSource(
            [ValueSource("intvals")] int x)
        {
            Assert.That(answer % x == 0);
        }
    }

    [TestFixture(typeof(int))]
    [TestFixture(typeof(string))]
    public class ParameterizedTestFixtureWithTypeAsArgument
    {
        private readonly Type _someType;

        public ParameterizedTestFixtureWithTypeAsArgument(Type someType)
        {
            _someType = someType;
        }

        [Test]
        public void MakeSureTypeIsInSystemNamespace()
        {
            Assert.AreEqual("System", _someType.Namespace);
        }
    }
}
