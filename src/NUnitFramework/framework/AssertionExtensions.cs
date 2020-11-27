#nullable enable

namespace NUnit.Framework
{
    /// <summary>
    /// Extension methods to help with Assert and nullability.
    /// </summary>
    public static class AssertionExtensions
    {
        /// <summary>
        /// Assert that this source is not null.
        /// </summary>
        /// <typeparam name="T">The type of instance being handled.</typeparam>
        /// <param name="source">The source instance to check for null.</param>
        /// <returns>The source instance, or throws when null.</returns>
        public static T AssertNotNull<T>(this T? source)
            where T : class
        {
            Assert.NotNull(source);

            return source;
        }

        /// <summary>
        /// Assert that this source is not null.
        /// </summary>
        /// <typeparam name="T">The type of instance being handled.</typeparam>
        /// <param name="source">The source instance to check for null.</param>
        /// <param name="name">The name of the source instance.</param>
        /// <returns>The source instance, or throws when null.</returns>
        public static T AssertNotNull<T>(this T? source, string name)
            where T : class
        {
            Assert.NotNull(source, name);

            return source;
        }
    }
}
