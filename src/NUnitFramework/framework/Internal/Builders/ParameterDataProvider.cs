// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#nullable enable

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// The ParameterDataProvider class implements IParameterDataProvider
    /// and hosts one or more individual providers.
    /// </summary>
    public class ParameterDataProvider : IParameterDataProvider
    {
        private readonly List<IParameterDataProvider> _providers = new List<IParameterDataProvider>();

        /// <summary>
        /// Construct with a collection of individual providers
        /// </summary>
        public ParameterDataProvider(params IParameterDataProvider[] providers)
        {
            _providers.AddRange(providers);
        }

        /// <summary>
        /// Determines whether any data is available for a parameter.
        /// </summary>
        /// <param name="parameter">The parameter of a parameterized test</param>
        public bool HasDataFor(IParameterInfo parameter)
        {
            foreach (var provider in _providers)
                if (provider.HasDataFor(parameter))
                    return true;

            return false;
        }

        /// <summary>
        /// Retrieves data for use with the supplied parameter.
        /// </summary>
        /// <param name="parameter">The parameter of a parameterized test</param>
        public IEnumerable GetDataFor(IParameterInfo parameter)
        {
            foreach (var provider in _providers)
                foreach (object? data in provider.GetDataFor(parameter))
                    yield return data;
        }
    }
}
