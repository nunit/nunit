// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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

            object indexedValue;
            var actualType = actual as Type ?? actual.GetType();

            if (actualType.IsArray)
            {
                var array = actual as Array;
                indexedValue = array?.GetValue(_arguments.Cast<int>().ToArray());
            }
            else
            {
                var getMethod = Reflect.GetDefaultIndexer(actualType, _argumentTypes) ?? throw new ArgumentException($"Default indexer accepting arguments {MsgUtils.FormatCollection(_arguments)} was not found on {actualType}.");

                indexedValue = Reflect.InvokeMethod(getMethod, actual, _arguments);
            }

            return BaseConstraint.ApplyTo(indexedValue);
        }

        /// <summary>
        /// Returns the string representation of the constraint.
        /// </summary>
        protected override string GetStringRepresentation() => $"<indexer {MsgUtils.FormatCollection(_arguments)} {BaseConstraint}>";
    }
}
