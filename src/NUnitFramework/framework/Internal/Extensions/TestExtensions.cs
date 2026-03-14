// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Retrieves the effective value of a specified property from the test hierarchy, returning a default value if
        /// the property is not found or its value is null.
        /// </summary>
        /// <remarks>This method traverses the test hierarchy, prioritizing method-level properties over
        /// class-level and assembly-level properties. Only the first matching property value is considered.</remarks>
        /// <typeparam name="T">Specifies the type of the property value to retrieve. Must be a non-nullable type.</typeparam>
        /// <param name="test">The test instance from which to retrieve the property value.</param>
        /// <param name="propertyName">The name of the property to search for in the test hierarchy.</param>
        /// <param name="defaultValue">The value to return if the property is not found or its value is null.</param>
        /// <returns>The effective value of the specified property, or the provided default value if the property is not found or
        /// its value is null.</returns>
        public static T GetEffectiveProperty<T>(this ITest test, string propertyName, T defaultValue)
            where T : notnull
        {
            // We are only interested in the first match when walking up the test hierarchy
            // So method before class before assembly.
            PropertyValueHierarchyItem? property = test.PropertyValues(propertyName)
                                                       .FirstOrDefault();

            if (property is null || property.Values.Count == 0)
                return defaultValue;

            object? value = property.Values[property.Values.Count - 1];
            if (value is null)
                return defaultValue;

            if (value is T typedValue)
                return typedValue;

            // Let it throw if the value cannot be converted to the requested type, rather than silently returning a default value.
            return (T)Convert.ChangeType(value, typeof(T))!;
        }
    }
}
