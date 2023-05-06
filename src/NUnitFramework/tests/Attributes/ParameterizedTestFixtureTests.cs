// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture(45, 45, 90)]
    [TestFixture(null, null, null)]
    public class NullableParameterizedTestFixture
    {
        private readonly int? _one;
        private readonly int? _two;
        private readonly int? _expected;

        public NullableParameterizedTestFixture(int? one, int? two, int? expected)
        {
            _one = one;
            _two = two;
            _expected = expected;
        }

        [Test]
        public void TestAddition()
        {
            if(_one.HasValue && _two.HasValue && _expected.HasValue)
            {
                Assert.That(_one.Value + _two.Value, Is.EqualTo(_expected.Value));
            }
            else
            {
                Assert.That(_one, Is.Null);
                Assert.That(_two, Is.Null);
            }
        }
    }

    [TestFixture("hello", "hello", "goodbye")]
    [TestFixture("zip", "zip")]
    [TestFixture(42, 42, 99)]
    [TestFixture(null, null, "null test")]
    [TestFixture(default(string), default(string), "typed null test")]
    public class ParameterizedTestFixture
    {
        private readonly string? eq1;
        private readonly string? eq2;
        private readonly string? neq;

        public ParameterizedTestFixture(string? eq1, string? eq2, string? neq)
        {
            this.eq1 = eq1;
            this.eq2 = eq2;
            this.neq = neq;
        }

        public ParameterizedTestFixture(string? eq1, string? eq2)
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
            Assert.That(eq2, Is.EqualTo(eq1));
            if (eq1 is not null && eq2 is not null)
                Assert.That(eq2.GetHashCode(), Is.EqualTo(eq1.GetHashCode()));
        }

        [Test]
        public void TestInequality()
        {
            Assert.That(neq, Is.Not.EqualTo(eq1));
            if (eq1 is not null && neq is not null)
                Assert.That(neq.GetHashCode(), Is.Not.EqualTo(eq1.GetHashCode()));
        }
    }

    public class ParameterizedTestFixtureNamingTests
    {
        private TestSuite fixture;

        [SetUp]
        public void MakeFixture()
        {
            fixture = TestBuilder.MakeFixture(typeof(NUnit.TestData.ParameterizedTestFixture));
        }

        [Test]
        public void TopLevelSuiteIsNamedCorrectly()
        {
            Assert.That(fixture.Name, Is.EqualTo("ParameterizedTestFixture"));
            Assert.That(fixture.FullName, Is.EqualTo("NUnit.TestData.ParameterizedTestFixture"));
        }

        [Test]
        public void SuiteHasCorrectNumberOfInstances()
        {
            Assert.That(fixture.Tests, Has.Count.EqualTo(2));
        }

        [Test]
        public void FixtureInstancesAreNamedCorrectly()
        {
            var names = new List<string>();
            var fullnames = new List<string>();
            foreach (Test test in fixture.Tests)
            {
                names.Add(test.Name);
                fullnames.Add(test.FullName);
            }

            Assert.That(names, Is.EquivalentTo(new[] {
                "ParameterizedTestFixture(1)", "ParameterizedTestFixture(2)" }));
            Assert.That(fullnames, Is.EquivalentTo(new[] {
                "NUnit.TestData.ParameterizedTestFixture(1)", "NUnit.TestData.ParameterizedTestFixture(2)" }));
        }

        [Test]
        public void MethodWithoutParamsIsNamedCorrectly()
        {
            TestSuite instance = (TestSuite)fixture.Tests[0];
            Test? method = TestFinder.Find("MethodWithoutParams", instance, false);
            Assert.That(method, Is.Not.Null );
            Assert.That(method.FullName, Is.EqualTo(instance.FullName + ".MethodWithoutParams"));
        }

        [Test]
        public void MethodWithParamsIsNamedCorrectly()
        {
            TestSuite instance = (TestSuite)fixture.Tests[0];
            TestSuite? method = (TestSuite?)TestFinder.Find("MethodWithParams", instance, false);
            Assert.That(method, Is.Not.Null);
            
            Test testcase = (Test)method.Tests[0];
            Assert.That(testcase.Name, Is.EqualTo("MethodWithParams(10,20)"));
            Assert.That(testcase.FullName, Is.EqualTo(instance.FullName + ".MethodWithParams(10,20)"));
        }
    }

    public class ParameterizedTestFixtureTests
    {
        [Test]
        public void CanSpecifyCategory()
        {
            Test fixture = TestBuilder.MakeFixture(typeof(NUnit.TestData.TestFixtureWithSingleCategory));
            Assert.That(fixture.Properties.Get(PropertyNames.Category), Is.EqualTo("XYZ"));
        }

        [Test]
        public void CanSpecifyMultipleCategories()
        {
            Test fixture = TestBuilder.MakeFixture(typeof(NUnit.TestData.TestFixtureWithMultipleCategories));
            Assert.That(fixture.Properties[PropertyNames.Category], Is.EqualTo(new[] { "X", "Y", "Z" }));
        }

        [Test]
        public void NullArgumentForOrdinaryValueTypeParameterDoesNotThrowNullReferenceException()
        {
            Test fixture = TestBuilder.MakeFixture(typeof(NUnit.TestData.TestFixtureWithNullArgumentForOrdinaryValueTypeParameter));
            Assert.That(fixture.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(fixture.Properties.Get(PropertyNames.SkipReason), Is.EqualTo("No suitable constructor was found"));
        }

        [Test]
        public void NullArgumentForGenericParameterDoesNotThrowNullReferenceException()
        {
            Test parameterizedFixture = TestBuilder.MakeFixture(typeof(NUnit.TestData.TestFixtureWithNullArgumentForGenericParameter<>));
            ITest fixture = parameterizedFixture.Tests.Single();

            Assert.That(fixture.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(fixture.Properties.Get(PropertyNames.SkipReason), Is.EqualTo(
                "Fixture type contains generic parameters. You must either provide Type arguments or specify constructor arguments that allow NUnit to deduce the Type arguments."));
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
            Assert.That(_someType.Namespace, Is.EqualTo("System"));
        }
    }
}
