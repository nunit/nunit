// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints.Comparers
{
    /// <summary>
    /// Result of the Equal comparison method.
    /// </summary>
    internal enum EqualMethodResult
    {
        /// <summary>
        /// Method does not support the instances being compared.
        /// </summary>
        TypesNotSupported,

        /// <summary>
        /// Method is appropriate for the data types, but doesn't support the specified tolerance.
        /// </summary>
        ToleranceNotSupported,

        /// <summary>
        /// Method is appropriate and the items are considered equal.
        /// </summary>
        ComparedEqual,

        /// <summary>
        /// Method is appropriate and the items are consisdered different.
        /// </summary>
        ComparedNotEqual
    }
}
