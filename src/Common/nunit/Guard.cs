using System;

#if NUNIT_ENGINE || CORE_ENGINE
namespace NUnit.Engine
#elif NUNIT_FRAMEWORK
namespace NUnit.Framework
#else
namespace NUnit.Common
#endif
{
    /// <summary>
    /// Class used to guard against unexpected argument values
    /// or operations by throwing an appropriate exception.
    /// </summary>
    static class Guard
    {
        /// <summary>
        /// Throws an exception if an argument is null
        /// </summary>
        /// <param name="value">The value to be tested</param>
        /// <param name="name">The name of the argument</param>
        public static void ArgumentNotNull(object value, string name)
        {
            if (value == null)
                throw new ArgumentNullException("Argument " + name + " must not be null", name);
        }

        /// <summary>
        /// Throws an exception if a string argument is null or empty
        /// </summary>
        /// <param name="value">The value to be tested</param>
        /// <param name="name">The name of the argument</param>
        public static void ArgumentNotNullOrEmpty(string value, string name)
        {
            ArgumentNotNull(value, name);

            if (value == string.Empty)
                throw new ArgumentException("Argument " + name +" must not be the empty string", name);
        }

        /// <summary>
        /// Throws an ArgumentException if the specified condition is not met.
        /// </summary>
        /// <param name="condition">The condition that must be met</param>
        /// <param name="message">The exception message to be used</param>
        /// <param name="paramName">The name of the argument</param>
        public static void ArgumentValid(bool condition, string message, string paramName)
        {
            if (!condition)
                throw new ArgumentException(message, paramName);
        }

        /// <summary>
        /// Throws an InvalidOperationException if the specified condition is not met.
        /// </summary>
        /// <param name="condition">The condition that must be met</param>
        /// <param name="message">The exception message to be used</param>
        public static void OperationValid(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
    }
}
