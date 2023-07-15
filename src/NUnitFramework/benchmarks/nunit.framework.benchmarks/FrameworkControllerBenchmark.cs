// ***********************************************************************
// Copyright (c) 2022 Charlie Poole
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
    private FrameworkController _controller;
    private List<string> _testIds;
    private string _orFilter;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var settings = new Hashtable
        {
            { "ProcessModel", "InProcess" },
            { "DomainUsage", "None" },
            { "ShadowCopyFiles", false },
            { "TestParametersDictionary", "" },
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
