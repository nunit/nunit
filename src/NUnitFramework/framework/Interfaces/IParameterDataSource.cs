// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System.Collections;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// Provides data for a single test parameter.
    /// </summary>
    public interface IParameterDataSource
    {
        /// <summary>
        /// Retrieves a list of arguments which can be passed to the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter of a parameterized test.</param>
        IEnumerable GetData(IParameterInfo parameter);
    }
}
