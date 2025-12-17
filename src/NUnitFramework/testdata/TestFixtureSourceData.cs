// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.TestData.TestUtilities;

namespace NUnit.TestData.TestFixtureSourceData
{
    public abstract class TestFixtureSourceTest
    {
        private readonly string _arg;
        private readonly string _expected;

        public TestFixtureSourceTest(string arg, string expected)
        {
            _arg = arg;
            _expected = expected;
        }

        [Test]
        public void CheckSource()
        {
            Assert.That(_arg, Is.EqualTo(_expected));
        }
    }

    public abstract class TestFixtureSourceDivideTest
    {
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;

        public TestFixtureSourceDivideTest(int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        [Test]
        public void CheckSource()
        {
            Assert.That(_x / _y, Is.EqualTo(_z));
        }
    }

    [TestFixtureSource(nameof(StaticField))]
    public class StaticField_SameClass : TestFixtureSourceTest
    {
        public StaticField_SameClass(string arg) : base(arg, "StaticFieldInClass")
        {
        }

        private static readonly object[] StaticField = new object[] { "StaticFieldInClass" };
    }

    [TestFixtureSource(nameof(StaticProperty))]
    public class StaticProperty_SameClass : TestFixtureSourceTest
    {
        public StaticProperty_SameClass(string arg) : base(arg, "StaticPropertyInClass")
        {
        }

        public StaticProperty_SameClass(string arg, string expected) : base(arg, expected)
        {
        }

        public static object[] StaticProperty => [new object[] { "StaticPropertyInClass" }];
    }

    [TestFixtureSource(nameof(StaticProperty))]
    public class StaticProperty_InheritedClass : StaticProperty_SameClass
    {
        public StaticProperty_InheritedClass(string arg) : base(arg, "StaticPropertyInClass")
        {
        }
    }

    [TestFixtureSource(nameof(StaticMethod))]
    public class StaticMethod_SameClass : TestFixtureSourceTest
    {
        public StaticMethod_SameClass(string arg) : base(arg, "StaticMethodInClass")
        {
        }

        private static object[] StaticMethod()
        {
            return new object[] { new object[] { "StaticMethodInClass" } };
        }
    }

    [TestFixtureSource(nameof(StaticAsyncMethod))]
    public class StaticAsyncMethod_SameClass : TestFixtureSourceTest
    {
        public StaticAsyncMethod_SameClass(string arg) : base(arg, nameof(StaticAsyncMethod))
        {
        }

        private static Task<object[]> StaticAsyncMethod()
        {
            return Task.FromResult(new object[] { new object[] { nameof(StaticAsyncMethod) } });
        }
    }

    [TestFixtureSource(nameof(StaticAsyncEnumerableMethod))]
    public class StaticAsyncEnumerableMethod_SameClass : TestFixtureSourceTest
    {
        public StaticAsyncEnumerableMethod_SameClass(string arg) : base(arg, nameof(StaticAsyncEnumerableMethod))
        {
        }

        private static IAsyncEnumerable<object> StaticAsyncEnumerableMethod()
        {
            var result = new object[] { new object[] { nameof(StaticAsyncEnumerableMethod) } };
            return result.AsAsyncEnumerable();
        }
    }

    [TestFixtureSource(nameof(StaticAsyncEnumerableMethodReturningTask))]
    public class StaticAsyncEnumerableMethodReturningTask_SameClass : TestFixtureSourceTest
    {
        public StaticAsyncEnumerableMethodReturningTask_SameClass(string arg) : base(arg, nameof(StaticAsyncEnumerableMethodReturningTask))
        {
        }

        private static Task<IAsyncEnumerable<object>> StaticAsyncEnumerableMethodReturningTask()
        {
            var result = new object[] { new object[] { nameof(StaticAsyncEnumerableMethodReturningTask) } };
            return Task.FromResult(result.AsAsyncEnumerable());
        }
    }

    [TestFixtureSource(nameof(InstanceField))]
    public class InstanceField_SameClass : TestFixtureSourceTest
    {
        public InstanceField_SameClass(string arg) : base(arg, "InstanceFieldInClass")
        {
        }

#pragma warning disable IDE1006 // Naming Styles
        private readonly object[] InstanceField = new object[] { "InstanceFieldInClass" };
#pragma warning restore IDE1006 // Naming Styles
    }

    [TestFixtureSource(nameof(InstanceProperty))]
    public class InstanceProperty_SameClass : TestFixtureSourceTest
    {
        public InstanceProperty_SameClass(string arg) : base(arg, "InstancePropertyInClass")
        {
        }

        private object[] InstanceProperty =>
            new object[]
            {
                new object[] { "InstancePropertyInClass" }
            };
    }

    [TestFixtureSource(nameof(InstanceMethod))]
    public class InstanceMethod_SameClass : TestFixtureSourceTest
    {
        public InstanceMethod_SameClass(string arg) : base(arg, "InstanceMethodInClass")
        {
        }

        private object[] InstanceMethod()
        {
            return new object[] { new object[] { "InstanceMethodInClass" } };
        }
    }

    [TestFixtureSource(typeof(SourceData), "StaticField")]
    public class StaticField_DifferentClass : TestFixtureSourceTest
    {
        public StaticField_DifferentClass(string arg) : base(arg, "StaticField")
        {
        }
    }

    [TestFixtureSource(typeof(SourceData), "StaticProperty")]
    public class StaticProperty_DifferentClass : TestFixtureSourceTest
    {
        public StaticProperty_DifferentClass(string arg) : base(arg, "StaticProperty")
        {
        }
    }

    [TestFixtureSource(typeof(SourceData), "StaticMethod")]
    public class StaticMethod_DifferentClass : TestFixtureSourceTest
    {
        public StaticMethod_DifferentClass(string arg) : base(arg, "StaticMethod")
        {
        }
    }

    [TestFixtureSource(typeof(SourceData_IEnumerable))]
    public class IEnumerableSource : TestFixtureSourceTest
    {
        public IEnumerableSource(string arg) : base(arg, nameof(SourceData_IEnumerable))
        {
        }
    }

    [TestFixtureSource(nameof(MyData))]
    public class SourceReturnsObjectArray : TestFixtureSourceDivideTest
    {
        public SourceReturnsObjectArray(int x, int y, int z) : base(x, y, z)
        {
        }

        private static IEnumerable MyData()
        {
            yield return new object[] { 12, 4, 3 };
            yield return new object[] { 12, 3, 4 };
            yield return new object[] { 12, 6, 2 };
        }
    }

    [TestFixtureSource(nameof(MyData))]
    public class SourceReturnsFixtureParameters : TestFixtureSourceDivideTest
    {
        public SourceReturnsFixtureParameters(int x, int y, int z) : base(x, y, z)
        {
        }

        private static IEnumerable MyData()
        {
            yield return new TestFixtureParameters(12, 4, 3);
            yield return new TestFixtureParameters(12, 3, 4);
            yield return new TestFixtureParameters(12, 6, 2);
        }
    }

    [TestFixture]
    [TestFixtureSource(nameof(MyData))]
    public class ExtraTestFixtureAttributeIsIgnored : TestFixtureSourceDivideTest
    {
        public ExtraTestFixtureAttributeIsIgnored(int x, int y, int z) : base(x, y, z)
        {
        }

        private static IEnumerable MyData()
        {
            yield return new object[] { 12, 4, 3 };
            yield return new object[] { 12, 3, 4 };
            yield return new object[] { 12, 6, 2 };
        }
    }

    [TestFixture]
    [TestFixtureSource(nameof(MyData))]
    [TestFixtureSource(nameof(MoreData), Category = "Extra")]
    [TestFixture(12, 12, 1)]
    public class TestFixtureMayUseMultipleSourceAttributes : TestFixtureSourceDivideTest
    {
        public TestFixtureMayUseMultipleSourceAttributes(int n, int d, int q) : base(n, d, q)
        {
        }

        private static IEnumerable MyData()
        {
            yield return new object[] { 12, 4, 3 };
            yield return new object[] { 12, 3, 4 };
            yield return new object[] { 12, 6, 2 };
        }

        private static readonly object[] MoreData = new object[]
        {
            new object[] { 12, 1, 12 },
            new object[] { 12, 2, 6 }
        };
    }

    [TestFixtureSource(nameof(MyData))]
    public class TestFixtureSourceMayUseParamsArguments
    {
        public TestFixtureSourceMayUseParamsArguments(params int[] parameters)
        {
            Parameters = parameters;
        }

        public int[] Parameters { get; }

        [Test]
        public void Test()
        {
            Assert.That(Parameters, Is.Not.Null);
            for (int i = 0; i < Parameters.Length; i++)
                Assert.That(Parameters[i], Is.EqualTo(i + 1));
        }
        private static IEnumerable MyData()
        {
            yield return new object[] { 1, 2, 3 };
            yield return new object[] { };
            yield return new object[] { new int[] { 1, 2, 3, 4 } };
        }
    }

    [TestFixtureSource(nameof(MyOptionalData))]
    public class TestFixtureSourceMayUseOptionalArguments
    {
        public TestFixtureSourceMayUseOptionalArguments(int arg1, int arg2 = 42)
        {
            Parameters = [arg1, arg2];
        }

        public int[] Parameters { get; }

        [Test]
        public void Test()
        {
            Assert.That(Parameters, Is.Not.Null);
            Assert.That(Parameters[0], Is.EqualTo(1));
            Assert.That(Parameters[1], Is.EqualTo(2).Or.EqualTo(42));
        }

        private static IEnumerable MyOptionalData()
        {
            yield return new object[] { 1 };
            yield return new object[] { 1, 2 };
        }
    }

    [TestFixtureSource(nameof(MyData))]
    public class TestFixtureSourceMayUseOptionalAndParamsArguments
    {
        public TestFixtureSourceMayUseOptionalAndParamsArguments(int arg1, int arg2 = 42, params int[] args)
        {
            Parameters = [arg1, arg2, .. args];
        }

        public int[] Parameters { get; }

        [Test]
        public void Test()
        {
            Assert.That(Parameters, Is.Not.Null);
            Assert.That(Parameters, Is.All.Not.Default);
        }

        private static IEnumerable MyData()
        {
            yield return new object[] { 1 };
            yield return new object[] { 1, 2 };
            yield return new object[] { 1, 2, 3, 4, 5 };
        }
    }

    [TestFixtureSource(nameof(MyData))]
    public class TestFixtureSourceInvalidValuesForOptionalArguments
    {
        public TestFixtureSourceInvalidValuesForOptionalArguments(int arg1, int arg2 = 42)
        {
            Parameters = [arg1, arg2];
        }

        public int[] Parameters { get; }

        [Test]
        public void Test()
        {
            Assert.Pass();
        }

        private static IEnumerable MyData()
        {
            yield return new object[] { };
            yield return new object[] { 1, 2, 3 };
            yield return new object[] { 1.3, 3.7 };
        }
    }

    [TestFixtureSource(nameof(IgnoredData))]
    public class IndividualInstancesMayBeIgnored : TestFixtureSourceTest
    {
        public IndividualInstancesMayBeIgnored(string arg) : base(arg, "IgnoredData")
        {
        }

        private static IEnumerable IgnoredData()
        {
            yield return new TestFixtureData("GoodData");
            yield return new TestFixtureData("IgnoredData").Ignore("There must be a reason");
            yield return new TestFixtureData("MoreGoodData");
        }
    }

    [TestFixtureSource(nameof(ExplicitData))]
    public class IndividualInstancesMayBeExplicit : TestFixtureSourceTest
    {
        public IndividualInstancesMayBeExplicit(string arg) : base(arg, "ExplicitData")
        {
        }

        private static IEnumerable ExplicitData()
        {
            yield return new TestFixtureData("GoodData");
            yield return new TestFixtureData("ExplicitData").Explicit("Runs long");
            yield return new TestFixtureData("MoreExplicitData").Explicit();
        }
    }

    #region Test name tests

    [TestFixtureSource(nameof(IndividualInstanceNameTestDataSource))]
    public sealed class IndividualInstanceNameTestDataFixture
    {
        public IndividualInstanceNameTestDataFixture(params object[] args)
        {
        }

        [Test]
        public void Test()
        {
        }

        public static IEnumerable<TestFixtureData> IndividualInstanceNameTestDataSource() =>
            from spec in TestDataSpec.Specs
            select new TestFixtureData(spec.Arguments)
            {
                Properties = // SetProperty does not exist
                    {
                        ["ExpectedTestName"] = { spec.GetFixtureName(nameof(IndividualInstanceNameTestDataFixture)) }
                    }
            }
                .SetArgDisplayNames(spec.ArgDisplayNames);
    }

    #endregion

    [TestFixture]
    public abstract class Issue1118_Root
    {
        protected readonly string Browser;

        protected Issue1118_Root(string browser)
        {
            Browser = browser;
        }

        [SetUp]
        public void Setup()
        {
        }
    }

    [TestFixtureSource(typeof(Issue1118_SourceData))]
    public class Issue1118_Base : Issue1118_Root
    {
        public Issue1118_Base(string browser) : base(browser)
        {
        }

        [TearDown]
        public void Cleanup()
        {
        }
    }

    public class Issue1118_Fixture : Issue1118_Base
    {
        public Issue1118_Fixture(string browser) : base(browser)
        {
        }

        [Test]
        public void DoSomethingOnAWebPageWithSelenium()
        {
        }

        [Test]
        public void DoSomethingElseOnAWebPageWithSelenium()
        {
        }
    }

    public class Issue1118_SourceData : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return "Firefox";
            yield return "Chrome";
            yield return "Internet Explorer";
        }
    }

    public class GenericFixtureSource
    {
        public static readonly Type[] SourceTypes = [
            typeof(short),
            typeof(int),
            typeof(long)
        ];

        public static readonly object[] SourceValues = [
            (short)1,
            1,
            (long)1
        ];
    }

    [TestFixtureSource(typeof(GenericFixtureSource), nameof(GenericFixtureSource.SourceTypes))]
    public class GenericFixtureSourceWithProperArgsProvided<T>
    {
        [Test]
        public void SomeTest()
        {
        }
    }

    public class GenericFixtureWithTypeAndConstructorArgsSource
    {
        public static readonly ITestFixtureData[] Source =
        {
            new TypedTestFixture<int>(5),
            new TypedTestFixture<object>(new object())
        };

        public class TypedTestFixture<T> : Framework.Internal.TestParameters, ITestFixtureData
        {
            public TypedTestFixture(params object[] arguments)
                : base(arguments)
            {
                TypeArgs = new[] { typeof(T) };
            }

            public Type[] TypeArgs { get; }
        }
    }

    [TestFixtureSource(typeof(GenericFixtureWithTypeAndConstructorArgsSource), nameof(GenericFixtureWithTypeAndConstructorArgsSource.Source))]
    public class GenericFixtureSourceWithTypeAndConstructorArgs<T>
    {
        private readonly T _arg;

        public GenericFixtureSourceWithTypeAndConstructorArgs(T arg)
        {
            _arg = arg;
        }

        [Test]
        public void SomeTest()
        {
            Assert.That(!EqualityComparer<T>.Default.Equals(_arg, default(T)), "constructor argument was not injected");
        }
    }

    public class GenericFixtureWithConstructorArgsSource
    {
        public static readonly TestFixtureData[] Source =
        {
            new TestFixtureData(5),
            new TestFixtureData(new object())
        };
    }

    [TestFixtureSource(typeof(GenericFixtureWithConstructorArgsSource), nameof(GenericFixtureWithConstructorArgsSource.Source))]
    public class GenericFixtureSourceWithConstructorArgs<T>
    {
        private readonly T _arg;

        public GenericFixtureSourceWithConstructorArgs(T arg)
        {
            _arg = arg;
        }

        [Test]
        public void SomeTest()
        {
            Assert.That(!EqualityComparer<T>.Default.Equals(_arg, default(T)), "constructor argument was not injected");
        }
    }

    [TestFixtureSource(typeof(GenericFixtureSource), nameof(GenericFixtureSource.SourceValues), TypeArgs = [typeof(long)])]
    public class GenericFixtureSourceWithExplicitTypeArgs<T>
    {
        private readonly T _arg;

        public GenericFixtureSourceWithExplicitTypeArgs(T arg)
        {
            _arg = arg;
        }

        [Test]
        public void SomeTest()
        {
            Assert.That(_arg, Is.TypeOf<long>());
        }
    }

    #region Source Data Classes

    internal class SourceData_IEnumerable : IEnumerable
    {
        public SourceData_IEnumerable()
        {
        }

        public IEnumerator GetEnumerator()
        {
            yield return nameof(SourceData_IEnumerable);
        }
    }

    internal class SourceData
    {
        private static readonly object[] StaticField = new object[] { nameof(StaticField) };

        private static object[] StaticProperty =>
            new object[]
            {
                new object[] { nameof(StaticProperty) }
            };

        private static object[] StaticMethod()
        {
            return new object[] { new object[] { nameof(StaticMethod) } };
        }
    }

    #endregion

    [TestFixtureSource(nameof(DataSource))]
    public abstract class DataSourcePrivateFieldInBaseClass
    {
        private static readonly int[] DataSource = { 3, 5 };

        protected DataSourcePrivateFieldInBaseClass(int data)
        {
            Data = data;
        }

        protected int Data { get; }
    }

    public class DerivedClassUsingBaseClassDataSource : DataSourcePrivateFieldInBaseClass
    {
        public DerivedClassUsingBaseClassDataSource(int data) : base(data)
        {
        }

        [Test]
        public void IsOdd()
        {
            Assert.That(Data % 2 == 1);
        }
    }

    public class BaseClassUsingDerivedClassDataSource : DataSourcePrivateFieldInBaseClass
    {
        private static readonly int[] DataSource = { 2, 4 };

        public BaseClassUsingDerivedClassDataSource(int data) : base(data)
        {
        }

        [Test]
        public void IsEven()
        {
            Assert.That(Data % 2 == 0);
        }
    }
}

[TestFixtureSource(nameof(MyData))]
public class NoNamespaceTestFixtureSourceWithTwoValues
{
    public NoNamespaceTestFixtureSourceWithTwoValues(int i)
    {
    }

    [Test]
    public void Test()
    {
    }

    private static readonly object[] MyData = { 1, 2 };
}

[TestFixtureSource(nameof(MyData))]
public class NoNamespaceTestFixtureSourceWithSingleValue
{
    public NoNamespaceTestFixtureSourceWithSingleValue(int i)
    {
    }

    [Test]
    public void Test()
    {
    }

    private static readonly object[] MyData = { 1 };
}

[TestFixtureSource(nameof(Data))]
[Parallelizable(ParallelScope.All)]
public class TextFixtureSourceWithParallelizableAttribute
{
    public TextFixtureSourceWithParallelizableAttribute(string arg)
    {
    }

    private static IEnumerable Data()
    {
        yield return new TestFixtureData("a");
        yield return new TestFixtureData("b");
        yield return new TestFixtureData("c");
    }

    [Test]
    public void Test()
    {
        Thread.Sleep(1000);
    }
}
