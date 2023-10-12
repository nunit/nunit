// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// The IReflectionInfo interface is implemented by NUnit wrapper objects that perform reflection.
    /// </summary>
    public interface IReflectionInfo
    {
        /// <summary>
        /// Returns an array of custom attributes of the specified type applied to this object
        /// </summary>
        T[] GetCustomAttributes<T>(bool inherit)
            where T : class;

        /// <summary>
        /// Returns a value indicating whether an attribute of the specified type is defined on this object.
        /// </summary>
        bool IsDefined<T>(bool inherit)
            where T : class;
    }
}
