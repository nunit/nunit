// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
