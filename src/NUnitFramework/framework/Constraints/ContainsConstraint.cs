// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    // TODO Needs tests
    /// <summary>
    /// ContainsConstraint tests a whether a string contains a substring
    /// or a collection contains an object. It postpones the decision of
    /// which test to use until the type of the actual argument is known.
    /// This allows testing whether a string is contained in a collection
    /// or as a substring of another string using the same syntax.
    /// </summary>
    public class ContainsConstraint : Constraint
    {
        private readonly object _expected;
        private Constraint? _realConstraint;
        private bool _ignoreCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainsConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value contained within the string/collection.</param>
        public ContainsConstraint(object expected)
        {
            _expected = expected;
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get
            {
                if (_realConstraint is not null)
                {
                    return _realConstraint.Description;
                }

                var description = "containing " + MsgUtils.FormatValue(_expected);

                if (_ignoreCase)
                    description += ", ignoring case";

                return description;
            }
        }

        /// <summary>
        /// Flag the constraint to ignore case and return self.
        /// </summary>
        public ContainsConstraint IgnoreCase
        {
            get { _ignoreCase = true; return this; }
        }

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (actual is string)
            {
                StringConstraint constraint = new SubstringConstraint((string)_expected);
                if (_ignoreCase)
                    constraint = constraint.IgnoreCase;
                _realConstraint = constraint;
            }
            else
            {
                var itemConstraint = new EqualConstraint(_expected);
                if (_ignoreCase)
                    itemConstraint = itemConstraint.IgnoreCase;
                _realConstraint = new SomeItemsConstraint(itemConstraint);
            }

            return _realConstraint.ApplyTo(actual);
        }
    }
}
