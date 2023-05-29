// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using BenchmarkDotNet.Attributes;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Abstractions;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework;

[MemoryDiagnoser]
public class CompositeWorkItemBenchmark
{
    private Dictionary<string, object> _loaderOptions;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _loaderOptions = new Dictionary<string, object>
        {
            { "ProcessModel", "InProcess" },
            { "DomainUsage", "None" },
            { "ShadowCopyFiles", false },
            { "TestParametersDictionary", "" },
            { "NumberOfTestWorkers", 0 },
            { "SynchronousEvents", "false" },
            { "RandomSeed", 878248866 },
            { "LOAD", new List<string> { "NUnit.Framework.TestWithTestActionAttribute" } },
            { "WorkDirectory", Path.GetTempPath() },
        };
    }

    [Benchmark]
    public void TestActionUsage()
    {
        DefaultTestAssemblyBuilder builder = new DefaultTestAssemblyBuilder();
        var test = builder.Build(typeof(TestWithTestActionAttribute).Assembly, _loaderOptions);
        WorkItem workItem = WorkItemBuilder.CreateWorkItem(test, TestFilter.Empty, new DebuggerProxy(), recursive: true);
        var context = new TestExecutionContext();
        context.Dispatcher = new SuperSimpleDispatcher();
        workItem.InitializeContext(context);
        workItem.Execute();
    }
}

[SomeTestAttr]
[TestFixtureSource(nameof(FixtureArgs))]
public class TestWithTestActionAttribute
{
    private static readonly object[] FixtureArgs = Enumerable.Range(0, 100)
                                                             .Select(a => new object[] { $"a{a:000}" })
                                                             .ToArray();

    public TestWithTestActionAttribute(string arg) { }

    [Test]
    public void Test1([Range(0, 10)] int x) { }
}

[AttributeUsage(AttributeTargets.Class)]
public class SomeTestAttrAttribute : Attribute, ITestAction
{
    private readonly object _data = Enumerable.Range(0, 1000000).ToArray();

    public SomeTestAttrAttribute()
    {
    }

    public void BeforeTest(ITest test) { }

    public void AfterTest(ITest test) { }

    public ActionTargets Targets { get; }
}

internal sealed class SuperSimpleDispatcher : IWorkItemDispatcher
{
    public int LevelOfParallelism => 0;

    public void Start(WorkItem topLevelWorkItem) => topLevelWorkItem.Execute();

    public void Dispatch(WorkItem work) => work.Execute();

    public void CancelRun(bool force) => throw new NotImplementedException();
}
