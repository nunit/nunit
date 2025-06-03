// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using NUnit.Framework;

[assembly: SimpleJob(RuntimeMoniker.Net462)]
[assembly: SimpleJob(RuntimeMoniker.Net80)]
[assembly: HideColumns("StdDev", "RatioSD", "Job")]

BenchmarkSwitcher.FromAssembly(typeof(FrameworkControllerBenchmark).Assembly).Run(args);
