// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.TestUtilities
{
    internal static class TestSuiteExtensions
    {
        public static TestSuite Containing(this TestSuite theSuite, string name)
        {
            return theSuite.Containing(new TestSuite(name));
        }

        public static TestSuite Containing(this TestSuite theSuite, params Type[] types)
        {
            foreach (var type in types)
                theSuite.Add(TestBuilder.MakeFixture(type));

            return theSuite;
        }

        public static TestSuite Containing(this TestSuite theSuite, params Test[] tests)
        {
            foreach (var test in tests)
                theSuite.Add(test);

            return theSuite;
        }

        public static TestSuite Parallelizable(this TestSuite theSuite)
        {
            theSuite.Properties.Set(PropertyNames.ParallelScope, ParallelScope.Self);

            return theSuite;
        }

        public static TestSuite NonParallelizable(this TestSuite theSuite)
        {
            theSuite.Properties.Set(PropertyNames.ParallelScope, ParallelScope.None);

            return theSuite;
        }
    }
}
