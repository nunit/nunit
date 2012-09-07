using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The FailureSite enum indicates the stage of a test
    /// in which an error or failure occured.
    /// </summary>
    public enum FailureSite
    {
        /// <summary>
        /// Failure in the test itself
        /// </summary>
        Test,

        /// <summary>
        /// Failure in the SetUp method
        /// </summary>
        SetUp,

        /// <summary>
        /// Failure in the TearDown method
        /// </summary>
        TearDown,

        /// <summary>
        /// Failure of a parent test
        /// </summary>
        Parent,

        /// <summary>
        /// Failure of a child test
        /// </summary>
        Child
    }
}
