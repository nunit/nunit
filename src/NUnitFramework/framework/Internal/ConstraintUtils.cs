// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Provides methods to support consistent checking in constraints.
    /// </summary>
    internal static class ConstraintUtils
    {
        /// <summary>
        /// Requires that the provided object is actually of the type required.
        /// </summary>
        /// <param name="actual">The object to verify.</param>
        /// <param name="paramName">Name of the parameter as passed into the checking method.</param>
        /// <param name="allowNull">
        /// If <see langword="true"/> and <typeparamref name="T"/> can be null, returns null rather than throwing when <paramref name="actual"/> is null.
        /// If <typeparamref name="T"/> cannot be null, this parameter is ignored.</param>
        /// <typeparam name="T">The type to require.</typeparam>
        public static T RequireActual<T>(object actual, string paramName, bool allowNull = false)
        {
            if (TypeHelper.TryCast(actual, out T result) && (allowNull || result != null))
            {
                return result;
            }

            var actualDisplay = actual == null ? "null" : actual.GetType().Name;
            throw new ArgumentException($"Expected: {typeof(T).Name} But was: {actualDisplay}", paramName);
        }
    }
}
