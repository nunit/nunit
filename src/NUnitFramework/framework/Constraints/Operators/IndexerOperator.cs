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

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Operator used to test for the presence of a Indexer
    /// on an object and optionally apply further tests to the
    /// value of that indexer.
    /// </summary>
    public class IndexerOperator : PrefixOperator
    {
        private readonly object[] _indexArguments;
        
        /// <summary>
        /// Constructs a IndexerOperator for a particular set of indexer
        /// parameters
        /// </summary>
        public IndexerOperator(params object[] indexArgs)
        {
            _indexArguments = indexArgs;
            
            // Indexer stacks on anything and allows only
            // prefix operators to stack on it.
            this.left_precedence = this.right_precedence = 1;
        }

        /// <summary>
        /// Returns a IndexerConstraint applied to its argument.
        /// </summary>
        public override IConstraint ApplyPrefix(IConstraint constraint)
        {
            return new IndexerConstraint(_indexArguments, constraint);
        }
    }
}
