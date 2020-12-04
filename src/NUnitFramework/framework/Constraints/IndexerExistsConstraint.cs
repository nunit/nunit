// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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
    /// IndexExistsConstraint tests that an indexer
    /// exists on the object provided through that matches
    /// the parameter type expectations.
    /// </summary>
    public class IndexerExistsConstraint : Constraint
    {
        private readonly Type[] _argumentTypes;
        private readonly object[] _arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexerExistsConstraint"/> class.
        /// </summary>
        /// <param name="indexerArguments">The argument list for the indexer.</param>
        public IndexerExistsConstraint(IEnumerable<object> indexerArguments)
            : base(indexerArguments)
        {
            _arguments = indexerArguments.ToArray();
            _argumentTypes = _arguments.Select(a => a.GetType()).ToArray();
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => $"Default indexer accepting arguments {MsgUtils.FormatCollection(_arguments)}";

        /// <summary>
        /// Test whether the indexer exists on a given object matching
        /// the argument type expectation
        /// </summary>
        /// <param name="actual">The object to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Guard.ArgumentNotNull(actual, nameof(actual));

            var actualType = actual as Type ?? actual.GetType();

            var indexer = Reflect.GetDefaultIndexer(actualType, _argumentTypes);
            return new ConstraintResult(this, actualType, indexer != null);
        }

        /// <summary>
        /// Returns the string representation of the constraint.
        /// </summary>
        /// <returns></returns>
        protected override string GetStringRepresentation() => $"<indexerexists {MsgUtils.FormatCollection(_arguments)}>";
    }
}
