// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Attributes
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
            if (_one.HasValue && _two.HasValue && _expected.HasValue)
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
        private readonly string? _eq1;
        private readonly string? _eq2;
        private readonly string? _neq;

        public ParameterizedTestFixture(string? eq1, string? eq2, string? neq)
        {
            _eq1 = eq1;
            _eq2 = eq2;
            _neq = neq;
        }

        public ParameterizedTestFixture(string? eq1, string? eq2)
            : this(eq1, eq2, null)
        {
        }

        public ParameterizedTestFixture(int eq1, int eq2, int neq)
        {
            _eq1 = eq1.ToString();
            _eq2 = eq2.ToString();
            _neq = neq.ToString();
        }

        [Test]
        public void TestEquality()
        {
            Assert.That(_eq2, Is.EqualTo(_eq1));
            if (_eq1 is not null && _eq2 is not null)
                Assert.That(_eq2.GetHashCode(), Is.EqualTo(_eq1.GetHashCode()));
        }

        [Test]
        public void TestInequality()
        {
            Assert.That(_neq, Is.Not.EqualTo(_eq1));
            if (_eq1 is not null && _neq is not null)
                Assert.That(_neq.GetHashCode(), Is.Not.EqualTo(_eq1.GetHashCode()));
        }
    }

    public class ParameterizedTestFixtureNamingTests
    {
        private TestSuite _fixture;

        [SetUp]
        public void MakeFixture()
        {
            _fixture = TestBuilder.MakeFixture(typeof(NUnit.TestData.ParameterizedTestFixture));
        }

        [Test]
        public void TopLevelSuiteIsNamedCorrectly()
        {
            Assert.That(_fixture.Name, Is.EqualTo("ParameterizedTestFixture"));
            Assert.That(_fixture.FullName, Is.EqualTo("NUnit.TestData.ParameterizedTestFixture"));
        }

        [Test]
        public void SuiteHasCorrectNumberOfInstances()
        {
            Assert.That(_fixture.Tests, Has.Count.EqualTo(2));
        }

        [Test]
        public void FixtureInstancesAreNamedCorrectly()
        {
            var names = new List<string>();
            var fullnames = new List<string>();
            foreach (Test test in _fixture.Tests)
            {
                names.Add(test.Name);
                fullnames.Add(test.FullName);
            }

            Assert.That(names, Is.EquivalentTo(new[]
            {
                "ParameterizedTestFixture(1)", "ParameterizedTestFixture(2)"
            }));
            Assert.That(fullnames, Is.EquivalentTo(new[]
            {
                "NUnit.TestData.ParameterizedTestFixture(1)", "NUnit.TestData.ParameterizedTestFixture(2)"
            }));
        }

        [Test]
        public void MethodWithoutParamsIsNamedCorrectly()
        {
            TestSuite instance = (TestSuite)_fixture.Tests[0];
            Test? method = TestFinder.Find("MethodWithoutParams", instance, false);
            Assert.That(method, Is.Not.Null);
            Assert.That(method.FullName, Is.EqualTo(instance.FullName + ".MethodWithoutParams"));
        }

        [Test]
        public void MethodWithParamsIsNamedCorrectly()
        {
            TestSuite instance = (TestSuite)_fixture.Tests[0];
            TestSuite? method = (TestSuite?)TestFinder.Find("MethodWithParams", instance, false);
            Assert.That(method, Is.Not.Null);

            Test testcase = (Test)method.Tests[0];
            Assert.That(testcase.Name, Is.EqualTo("MethodWithParams(10,20)"));
            Assert.That(testcase.FullName, Is.EqualTo(instance.FullName + ".MethodWithParams(10,20)"));
        }
    }

    public class AnotherParameterizedTestFixtureNamingTests
    {
        private TestSuite _fixture;

        [OneTimeSetUp]
        public void MakeFixture()
        {
            _fixture = TestBuilder.MakeFixture(typeof(AnotherParameterizedTestFixture));
        }

        [Test]
        public void TopLevelSuiteIsNamedCorrectly()
        {
            Assert.That(_fixture.Name, Is.EqualTo(nameof(AnotherParameterizedTestFixture)));
            Assert.That(_fixture.FullName, Is.EqualTo($"NUnit.TestData.{nameof(AnotherParameterizedTestFixture)}"));
        }

        [Test]
        public void SuiteHasCorrectNumberOfInstances()
        {
            Assert.That(_fixture.Tests, Has.Count.EqualTo(2));
        }

        [Test]
        public void FixtureInstancesAreNamedCorrectly()
        {
            var names = new List<string>();
            var fullnames = new List<string>();
            foreach (Test test in _fixture.Tests)
            {
                names.Add(test.Name);
                fullnames.Add(test.FullName);
            }

            const string fixtureName1 = $"{nameof(AnotherParameterizedTestFixture)}({AnotherParameterizedTestFixture.DisplayParameterValue1})";
            const string fixtureName2 = $"{nameof(AnotherParameterizedTestFixture)}({AnotherParameterizedTestFixture.DisplayParameterValue2})";

            Assert.That(names, Is.EquivalentTo(new[] { fixtureName1, fixtureName2 }));
            Assert.That(fullnames, Is.EquivalentTo(new[] { $"NUnit.TestData.{fixtureName1}", $"NUnit.TestData.{fixtureName2}" }));
        }

        [Test]
        public void MethodWithParamsIsNamedCorrectly()
        {
            TestSuite instance = (TestSuite)_fixture.Tests[0];
            TestSuite? method = (TestSuite?)TestFinder.Find(nameof(AnotherParameterizedTestFixture.TestCase), instance, false);
            Assert.That(method, Is.Not.Null);

            const string testName = $"{nameof(AnotherParameterizedTestFixture.TestCase)}({AnotherParameterizedTestFixture.DisplayParameterValue2})";

            Test testcase = (Test)method.Tests[0];
            Assert.That(testcase.Name, Is.EqualTo(testName));
            Assert.That(testcase.FullName, Is.EqualTo($"{instance.FullName}.{testName}"));
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
