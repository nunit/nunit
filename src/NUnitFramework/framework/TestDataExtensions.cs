// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Provides fluent extension methods for setting the description, categories and
    /// properties of any <see cref="Internal.TestParameters"/>, such as <see cref="TestFixtureData"/>.
    /// </summary>
    public static class TestDataExtensions
    {
        extension<T>(T testData)
            where T : Internal.TestParameters
        {
            /// <summary>
            /// Sets the description, replacing any description set previously.
            /// </summary>
            /// <param name="description">The description.</param>
            /// <returns>The modified instance.</returns>
            public T SetDescription(string description)
            {
                testData.Properties.Set(PropertyNames.Description, description);
                return testData;
            }

            /// <summary>
            /// Adds a category. Calling this more than once adds multiple categories.
            /// </summary>
            /// <param name="category">The name of the category.</param>
            /// <returns>The modified instance.</returns>
            public T SetCategory(string category)
            {
                testData.Properties.Add(PropertyNames.Category, category);
                return testData;
            }

            /// <summary>
            /// Adds a named property with a <see cref="string"/> value.
            /// </summary>
            /// <param name="propName">The name of the property.</param>
            /// <param name="propValue">The value of the property.</param>
            /// <returns>The modified instance.</returns>
            public T SetProperty(string propName, string propValue)
            {
                testData.Properties.Add(propName, propValue);
                return testData;
            }

            /// <summary>
            /// Adds a named property with an <see cref="int"/> value.
            /// </summary>
            /// <param name="propName">The name of the property.</param>
            /// <param name="propValue">The value of the property.</param>
            /// <returns>The modified instance.</returns>
            public T SetProperty(string propName, int propValue)
            {
                testData.Properties.Add(propName, propValue);
                return testData;
            }

            /// <summary>
            /// Adds a named property with a <see cref="double"/> value.
            /// </summary>
            /// <param name="propName">The name of the property.</param>
            /// <param name="propValue">The value of the property.</param>
            /// <returns>The modified instance.</returns>
            public T SetProperty(string propName, double propValue)
            {
                testData.Properties.Add(propName, propValue);
                return testData;
            }
        }
    }
}
