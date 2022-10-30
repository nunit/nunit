// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable
namespace NUnit.Framework.Internal.Extensions
{
    internal static class TestExtensions
    {
        public static bool HasLifeCycle(this ITest? test, LifeCycle lifeCycle)
        {
            while (test != null)
            {
                if (test is TestFixture fixture)
                    return fixture.LifeCycle == lifeCycle;

                test = test.Parent;
            }

            return lifeCycle != LifeCycle.InstancePerTestCase;
        }
    }
}
