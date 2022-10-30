// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System.Collections.Generic;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// ParameterDataSourceProvider supplies individual argument values for
    /// single parameters using attributes implementing IParameterDataSource.
    /// </summary>
    public class ParameterDataSourceProvider : IParameterDataProvider
    {
        #region IParameterDataProvider Members

        /// <summary>
        /// Determines whether any data is available for a parameter.
        /// </summary>
        /// <param name="parameter">The parameter of a parameterized test</param>
        public bool HasDataFor(IParameterInfo parameter)
        {
            return parameter.IsDefined<IParameterDataSource>(false);
        }

        /// <summary>
        /// Retrieves data for use with the supplied parameter.
        /// </summary>
        /// <param name="parameter">The parameter of a parameterized test</param>
        public IEnumerable GetDataFor(IParameterInfo parameter)
        {
            var data = new List<object?>();

            foreach (IParameterDataSource source in parameter.GetCustomAttributes<IParameterDataSource>(false))
            {
                foreach (object? item in source.GetData(parameter))
                    data.Add(item);
            }

            return data;
        }
        #endregion
    }
}
