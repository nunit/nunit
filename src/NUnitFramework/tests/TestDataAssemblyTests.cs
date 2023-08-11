// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Diagnostics;
using System.Reflection;

namespace NUnit.Framework.Tests
{
    public static class TestDataAssemblyTests
    {
        [Test(Description = "Needed because we test stack traces and don’t want to have to put [MethodImpl(MethodImplOptions.NoInlining)] on everything involved.")]
        public static void AssemblyIsNotOptimized()
        {
            Assert.That(Assembly.Load(new AssemblyName("nunit.testdata")),
                Has.Attribute<DebuggableAttribute>().With.Property("IsJITOptimizerDisabled").EqualTo(true));
        }
    }
}
