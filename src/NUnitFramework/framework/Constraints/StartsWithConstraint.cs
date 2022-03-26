// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// StartsWithConstraint can test whether a string starts
    /// with an expected substring.
    /// </summary>
    public class StartsWithConstraint : StringConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartsWithConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected string</param>
        public StartsWithConstraint(string expected) : base(expected) 
        {
            descriptionText = "String starting with";
        }

        /// <summary>
        /// Test whether the constraint is matched by the actual value.
        /// This is a template method, which calls the IsMatch method
        /// of the derived class.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns></returns>
        protected override bool Matches(string actual)
        {
            if (this.caseInsensitive)
                return actual != null && actual.StartsWith(expected, System.StringComparison.CurrentCultureIgnoreCase);
            else
                return actual != null && actual.StartsWith(expected);
        }
    }
}
