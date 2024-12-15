// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using BenchmarkDotNet.Running;
using NUnit.Framework;

[assembly: NUnitJob]

BenchmarkSwitcher.FromAssembly(typeof(FrameworkControllerBenchmark).Assembly).Run(args);
