// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class TestExtensions
    {
        public static bool HasLifeCycle(this ITest test, LifeCycle lifeCycle)
        {
            while (test != null)
            {
                if (test is TestFixture fixture && fixture.LifeCycle == LifeCycle.InstancePerTestCase)
                    return lifeCycle == LifeCycle.InstancePerTestCase;

                test = test.Parent;
            }
            return lifeCycle != LifeCycle.InstancePerTestCase;
        }
    }
}
