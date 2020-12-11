// ***********************************************************************
// Copyright (c) 2020 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// IndexerConstraint extracts a named property and uses
    /// its value as the actual value for a chained constraint.
    /// </summary>
    public class IndexerConstraint : PrefixConstraint
    {
        private readonly Type[] _argumentTypes;
        private readonly object[] _arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexerConstraint"/> class.
        /// </summary>
        /// <param name="indexerArguments">The argument list for the indexer.</param>
        /// <param name="baseConstraint">The constraint to apply to the indexer.</param>
        public IndexerConstraint(IEnumerable<object> indexerArguments, IConstraint baseConstraint)
            : base(baseConstraint)
        {
            _arguments = indexerArguments.ToArray();
            _argumentTypes = _arguments.Select(a => a.GetType()).ToArray();

            DescriptionPrefix = $"Default indexer accepting arguments {MsgUtils.FormatCollection(_arguments)}";
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Guard.ArgumentNotNull(actual, nameof(actual));

            var actualType = actual as Type ?? actual.GetType();

            var getMethod = Reflect.GetDefaultIndexer(actualType, _argumentTypes) ?? throw new ArgumentException($"Default indexer accepting arguments {MsgUtils.FormatCollection(_arguments)} was not found on {actualType}.");

            var indexReturnedValue = Reflect.InvokeMethod(getMethod, actual, _arguments);
            return BaseConstraint.ApplyTo(indexReturnedValue);
        }

        /// <summary>
        /// Returns the string representation of the constraint.
        /// </summary>
        protected override string GetStringRepresentation() => $"<indexer {MsgUtils.FormatCollection(_arguments)} {BaseConstraint}>";
    }
}
