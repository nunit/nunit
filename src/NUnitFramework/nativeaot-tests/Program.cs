// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using NUnitLite;

namespace NUnit.Framework.NativeAotTests
{
    internal static class Program
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AsyncTests))]
        private static int Main(string[] args)
        {
            // Reflection can discover a generic test but cannot generate its native code.
            GC.KeepAlive((Action<int>)new AsyncTests().GenericValueType<int>);
            return new AutoRun(typeof(AsyncTests).Assembly).Execute(args);
        }
    }
}
