using System;

namespace NUnit.Engine
{
    /// <summary>
    /// The exception that is thrown if a valid test engine is not found
    /// </summary>
    [Serializable]
    public class NUnitEngineNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitEngineNotFoundException"/> class.
        /// </summary>
        public NUnitEngineNotFoundException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitEngineNotFoundException"/> class.
        /// </summary>
        /// <param name="minVersion">The minimum version.</param>
        /// <param name="maxVersion">The maximum version.</param>
        internal NUnitEngineNotFoundException(Version minVersion, Version maxVersion = null)
            :base(CreateMessage(minVersion, maxVersion))
        {
        }

        private static string CreateMessage(Version minVersion = null, Version maxVersion = null)
        {
            if (maxVersion == null || maxVersion == TestEngineActivator.DefaultMaximumVersion)
            {
                return string.Format("{0} with a version greater than or equal to {1} not found",
                    TestEngineActivator.DefaultTypeName, minVersion ?? TestEngineActivator.DefaultMinimumVersion);
            }

            return string.Format("{0} with a version greater than or equal to {1} and less than or equal to {2} not found",
                TestEngineActivator.DefaultTypeName, minVersion ?? TestEngineActivator.DefaultMinimumVersion, maxVersion);
        }
    }
}