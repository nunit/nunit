// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using static NUnit.Framework.TestContext;

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

        /// <summary>
        /// Returns all property values in the hierarchy of a given name
        /// The returned values include the name of the level they are found at.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PropertyValueHierarchyItem> PropertyValues(this ITest test, string property)
        {
            ITest? currentTest = test;
            do
            {
                if (currentTest.Properties.TryGet(property, out System.Collections.IList? propValues))
                {
                    yield return new PropertyValueHierarchyItem(currentTest.Name, propValues);
                }

                currentTest = currentTest.Parent;
            }
            while (currentTest is not null);
        }
    }
}
