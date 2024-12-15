// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace NUnit.Framework;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
internal class NUnitJobAttribute : Attribute, IConfigSource
{
    public NUnitJobAttribute()
    {
        var arg = new MsBuildArgument("/p:AssemblyConfiguration=Release");
        var job = new Job(Job.Default).WithArguments([arg]);

        Config = DefaultConfig.Instance.AddJob(job);
    }

    public IConfig Config { get; }
}
