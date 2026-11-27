// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.HostOwnedReturnTypeFixture;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    [NonParallelizable] // the test data counts invocations in a static field
    public class HostOwnedReturnTypesTests
    {
        private static readonly string[] EnumeratorOwned = { typeof(IEnumerator).FullName! };

        private static Dictionary<string, object> Options(string[]? hostOwnedReturnTypes)
        {
            var options = new Dictionary<string, object>
            {
                [FrameworkPackageSettings.LOAD] = new List<string> { typeof(CoroutineTests).Namespace! },
            };
            if (hostOwnedReturnTypes is not null)
                options[FrameworkPackageSettings.HostOwnedReturnTypes] = hostOwnedReturnTypes;
            return options;
        }

        private static ITest Build(string[]? hostOwnedReturnTypes) =>
            new DefaultTestAssemblyBuilder().Build(Assembly.Load("nunit.testdata"), Options(hostOwnedReturnTypes));

        private static ITestResult Run(string[]? hostOwnedReturnTypes)
        {
            var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            var options = Options(hostOwnedReturnTypes);
            options[FrameworkPackageSettings.NumberOfTestWorkers] = 0;
            runner.Load(Assembly.Load("nunit.testdata"), options);
            return runner.Run(TestListener.NULL, TestFilter.Empty);
        }

        private static IEnumerable<ITest> All(ITest test) => new[] { test }.Concat(test.Tests.SelectMany(All));

        private static IEnumerable<ITestResult> All(ITestResult result) => new[] { result }.Concat(result.Children.SelectMany(All));

        private static ITest Find(ITest root, string fullNameSuffix) =>
            Single(All(root).Where(t => t.FullName.EndsWith(fullNameSuffix, StringComparison.Ordinal)), fullNameSuffix);

        private static ITestResult Find(ITestResult root, string fullNameSuffix) =>
            Single(All(root).Where(r => r.FullName.EndsWith(fullNameSuffix, StringComparison.Ordinal)), fullNameSuffix);

        private static T Single<T>(IEnumerable<T> matches, string fullNameSuffix)
        {
            var list = matches.ToList();
            Assert.That(list, Has.Count.EqualTo(1), $"Tests whose full name ends with '{fullNameSuffix}'");
            return list[0];
        }

        [SetUp]
        public void ResetInvocations() => Coroutine.Invocations = 0;

        [Test]
        public void WithoutSetting_MethodsReturningTheTypeAreNotRunnable()
        {
            var root = Build(null);

            Assert.Multiple(() =>
            {
                Assert.That(Find(root, "CoroutineTests.PlainTest").RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(Find(root, "CoroutineTests.ParameterizedTest(1)").RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(Find(root, "CoroutineTests.SourceTest(1)").RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(Find(root, "CoroutineTests.IgnoredTest").RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(Find(root, ".CoroutineSetUpFixture").RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(Find(root, ".CoroutineTearDownFixture").RunState, Is.EqualTo(RunState.NotRunnable));
                Assert.That(Find(root, ".TestsUnderSetUpFixture").Parent!.RunState, Is.EqualTo(RunState.NotRunnable));
            });
        }

        [Test]
        public void WithSetting_MethodsReturningTheTypeAreAccepted()
        {
            var root = Build(EnumeratorOwned);

            Assert.Multiple(() =>
            {
                Assert.That(Find(root, "CoroutineTests.PlainTest").RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(Find(root, "CoroutineTests.ParameterizedTest(1)").RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(Find(root, "CoroutineTests.ParameterizedTest(2)").RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(Find(root, "CoroutineTests.SourceTest(1)").RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(Find(root, "CoroutineTests.IgnoredTest").RunState, Is.EqualTo(RunState.Ignored));
                Assert.That(Find(root, ".CoroutineSetUpFixture").RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(Find(root, ".CoroutineTearDownFixture").RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(Find(root, ".TestsUnderSetUpFixture").Parent!.RunState, Is.EqualTo(RunState.Runnable));
            });
        }

        [Test]
        public void WithSetting_OtherSignatureChecksStillApply()
        {
            var test = Find(Build(EnumeratorOwned), "CoroutineTests.TooManyArguments(1,2)");

            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(test.Properties.Get(PropertyNames.SkipReason), Does.StartWith("Too many arguments provided"));
        }

        [Test]
        public void WithSetting_OtherReturnTypesAreStillRejected()
        {
            var root = Build(new[] { typeof(string).FullName! });

            Assert.That(Find(root, "CoroutineTests.PlainTest").RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void GenericTypeDefinitionCoversConstructedTypes()
        {
            var root = Build(new[] { typeof(IEnumerator<>).FullName! });

            Assert.Multiple(() =>
            {
                Assert.That(Find(root, "GenericCoroutineTests.GenericEnumeratorTest").RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(Find(root, "CoroutineTests.PlainTest").RunState, Is.EqualTo(RunState.NotRunnable));
            });
        }

        [Test]
        public void SettingOfTheWrongTypeMakesTheAssemblyInvalid()
        {
            var options = Options(null);
            options[FrameworkPackageSettings.HostOwnedReturnTypes] = typeof(IEnumerator).FullName!;

            var root = new DefaultTestAssemblyBuilder().Build(Assembly.Load("nunit.testdata"), options);

            Assert.That(root.RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(root.Properties.Get(PropertyNames.SkipReason), Does.Contain(nameof(FrameworkPackageSettings.HostOwnedReturnTypes)));
        }

        [Test]
        public void TypesNUnitRunsItselfAreIgnored()
        {
            var root = Build(new[] { typeof(void).FullName!, typeof(System.Threading.Tasks.Task).FullName! });

            Assert.Multiple(() =>
            {
                Assert.That(Find(root, "CoroutineTests.VoidTest").RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(Find(root, "CoroutineTests.TaskTest").RunState, Is.EqualTo(RunState.Runnable));
                Assert.That(Find(root, "CoroutineTests.PlainTest").RunState, Is.EqualTo(RunState.NotRunnable));
            });

            var result = Run(new[] { typeof(System.Threading.Tasks.Task).FullName! });
            Assert.That(Find(result, "CoroutineTests.TaskTest").ResultState, Is.EqualTo(ResultState.Success));
        }

        [Test]
        public void NullSettingIsTreatedAsNotSet()
        {
            var options = Options(null);
            options[FrameworkPackageSettings.HostOwnedReturnTypes] = null!;

            var root = new DefaultTestAssemblyBuilder().Build(Assembly.Load("nunit.testdata"), options);

            Assert.That(Find(root, "CoroutineTests.PlainTest").RunState, Is.EqualTo(RunState.NotRunnable));
            Assert.That(Find(root, "CoroutineTests.VoidTest").RunState, Is.EqualTo(RunState.Runnable));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestWithExpectedResultIsRunAndComparedAsBefore(bool withSetting)
        {
            NUnit.TestData.HostOwnedReturnTypeExpectedResult.ExpectedResultTests.Invocations = 0;
            var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            var options = new Dictionary<string, object>
            {
                [FrameworkPackageSettings.LOAD] = new List<string> { typeof(NUnit.TestData.HostOwnedReturnTypeExpectedResult.ExpectedResultTests).FullName! },
                [FrameworkPackageSettings.NumberOfTestWorkers] = 0,
            };
            if (withSetting)
                options[FrameworkPackageSettings.HostOwnedReturnTypes] = EnumeratorOwned;
            runner.Load(Assembly.Load("nunit.testdata"), options);

            var result = Find(runner.Run(TestListener.NULL, TestFilter.Empty), "ExpectedResultTests.ReturnsNull()");

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(NUnit.TestData.HostOwnedReturnTypeExpectedResult.ExpectedResultTests.Invocations, Is.EqualTo(1));
            });
        }

        [Test]
        public void SettingDoesNotLeakIntoTheNextBuild()
        {
            Build(EnumeratorOwned);

            Assert.That(Find(Build(null), "CoroutineTests.PlainTest").RunState, Is.EqualTo(RunState.NotRunnable));
        }

        [Test]
        public void NUnitRunnerDoesNotInvokeHostOwnedMethods()
        {
            var result = Run(EnumeratorOwned);

            Assert.Multiple(() =>
            {
                Assert.That(Coroutine.Invocations, Is.Zero);
                Assert.That(Find(result, "CoroutineTests.PlainTest").ResultState, Is.EqualTo(ResultState.NotRunnable));
                Assert.That(Find(result, "CoroutineTests.PlainTest").Message, Does.StartWith("NUnit cannot run a test method returning 'System.Collections.IEnumerator'"));
                Assert.That(Find(result, "CoroutineTests.ParameterizedTest(1)").ResultState, Is.EqualTo(ResultState.NotRunnable));
                Assert.That(Find(result, "CoroutineTests.IgnoredTest").ResultState, Is.EqualTo(ResultState.Ignored));
                Assert.That(Find(result, "CoroutineTests.VoidTest").ResultState, Is.EqualTo(ResultState.Success));
                Assert.That(Find(result, "GenericCoroutineTests.GenericEnumeratorTest").ResultState, Is.EqualTo(ResultState.NotRunnable));
                Assert.That(Find(result, "CoroutineSetUpFixture.VoidTest").Message, Does.Contain("setup or teardown method returning 'System.Collections.IEnumerator'"));
                Assert.That(Find(result, "CoroutineSetUpFixture.VoidTest").ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(Find(result, "CoroutineTearDownFixture.VoidTest").ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(Find(result, "TestsUnderSetUpFixture.VoidTest").ResultState.Status, Is.EqualTo(TestStatus.Failed));
            });
        }

        [Test]
        public void TestWithClearedExpectedResultIsNotRunnableInsteadOfPassing()
        {
            var result = Find(Run(null), "DummyExpectedResultTests.NeverRuns()");

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.NotRunnable));
                Assert.That(Coroutine.Invocations, Is.Zero);
            });
        }
    }
}
