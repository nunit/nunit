// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using BenchmarkDotNet.Attributes;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;
using NUnit.Framework.Tests;

namespace NUnit.Framework;

[InProcess] // need to use so that we get correct test loading
[MemoryDiagnoser]
public class FrameworkControllerBenchmark
{
    private readonly FrameworkController _controller;
    private readonly List<string> _testIds;
    private readonly string _orFilter;

    public FrameworkControllerBenchmark()
    {
        var settings = new Hashtable
        {
            { "ProcessModel", "InProcess" },
            { "DomainUsage", "None" },
            { "ShadowCopyFiles", false },
            { "TestParametersDictionary", string.Empty },
            { "NumberOfTestWorkers", 0 },
            { "SynchronousEvents", "false" },
            { "RandomSeed", 878248866 },
            { "LOAD", new List<string> { "NUnit.Framework" } },
            { "WorkDirectory", Path.GetTempPath() },
        };
        _controller = new FrameworkController(typeof(WorkItemTests).Assembly, "0-", settings);
        _controller.LoadTests();

        _testIds = new List<string>();
        var exploreTests = _controller.Runner.ExploreTests(TestFilter.Empty);
        GatherTestIds(exploreTests.Tests, _testIds);

        _orFilter = "<filter>" + new OrFilter(_testIds.Select(x => (TestFilter)new IdFilter(x)).ToArray()).ToXml(true).OuterXml + "</filter>";
    }

    [Benchmark]
    public int CountTestsEmptyFilter()
    {
        var count = _controller.CountTests("<filter/>");
        return count;
    }

    [Benchmark]
    public int CountTestsWithCategoryFilter()
    {
        var count = _controller.CountTests("<filter><cat>Dummy</cat></filter>");
        return count;
    }

    [Benchmark]
    public int CountTestsWithOrFilterAll()
    {
        var count = _controller.CountTests(_orFilter);
        return count;
    }

    [Benchmark]
    public string LoadTests()
    {
        return _controller.LoadTests();
    }

    private static void GatherTestIds(IList<ITest> tests, List<string> collector)
    {
        foreach (var child in tests)
        {
            collector.Add(child.Id);
            GatherTestIds(child.Tests, collector);
        }
    }
}
