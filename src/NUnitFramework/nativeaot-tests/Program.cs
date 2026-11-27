// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NUnitLite;

namespace NUnit.Framework.NativeAotTests
{
    internal static class Program
    {
        private const int ExpectedTestCount = 21;

        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AsyncTests))]
        private static int Main(string[] args)
        {
            // CoreCLR supplies a platform-assembly list even when PublishAot disables dynamic code in its runtimeconfig.
            if (RuntimeFeature.IsDynamicCodeSupported || AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is not null)
            {
                Console.Error.WriteLine("Publish and run the NativeAOT executable; CoreCLR cannot verify native code availability.");
                return 1;
            }

            // Reflection can discover a generic test but cannot generate its native code.
            GC.KeepAlive((Action<int>)new AsyncTests().GenericValueType<int>);
            var runner = new TextRunner(typeof(AsyncTests).Assembly);
            int exitCode = runner.Execute(args);
            if (exitCode != 0)
                return exitCode;

            ResultSummary? summary = runner.Summary;
            if (summary is null || summary.TestCount != ExpectedTestCount || summary.PassCount != ExpectedTestCount)
            {
                Console.Error.WriteLine($"Expected all {ExpectedTestCount} NativeAOT regression cases to pass; " +
                    $"discovered {summary?.TestCount ?? 0}, passed {summary?.PassCount ?? 0}.");
                return 1;
            }

            return 0;
        }
    }
}
