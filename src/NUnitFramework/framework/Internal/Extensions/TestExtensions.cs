// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Extensions
{
    internal static class TestExtensions
    {
        public static bool HasLifeCycle(this ITest? test, LifeCycle lifeCycle)
        {
            while (test is not null)
            {
                if (test is TestFixture fixture)
                    return fixture.LifeCycle == lifeCycle;

                test = test.Parent;
            }

            return lifeCycle != LifeCycle.InstancePerTestCase;
        }
    }
}
