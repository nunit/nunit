using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// <see cref="ExactlyOneConstraint"/> applies another constraint to
    /// a singular item in a collection, succeeding only if the singular 
    /// item in the collection succeeds.
    /// </summary>
    public class ExactlyOneConstraint : Constraint
    {
        private readonly IConstraint _itemConstraint;
        
        /// <summary>
        /// Construct a standalone <see cref="ExactlyOneConstraint"/>.
        /// </summary>
        public ExactlyOneConstraint()
        { }

        /// <summary>
        /// Construct an <see cref="ExactlyOneConstraint"/> on top of
        /// an existing constraint.
        /// </summary>
        /// <param name="itemConstraint"></param>
        public ExactlyOneConstraint(IConstraint itemConstraint)
            : base(itemConstraint)
        {
            Guard.ArgumentNotNull(itemConstraint, "itemConstraint");

            _itemConstraint = itemConstraint.Resolve();
        }

        /// <summary>
        /// Apply the constraint to the singular item in the collection, 
        /// succeeding only if the singular item passes.
        /// </summary>
        /// <typeparam name="TActual"></typeparam>
        /// <param name="actual"></param>
        /// <returns></returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (!(actual is IEnumerable))
                throw new ArgumentException("The constraint provided is not an IEnumerable", "actual");

            IEnumerable actualEnumerable = (IEnumerable)actual;

            int count = actualEnumerable.Cast<object>().Count();

            if (_itemConstraint == null)
            {
                //Only checking the count of actualEnumerable.
                if (count == 1)
                {
                    return new ConstraintResult(this, "1", true);
                }
                else
                {
                    return new ConstraintResult(this, count, false);
                }
            }
            else
            {
                if (count > 0)
                {
                    int successfulItems = 0;

                    foreach(object item in actualEnumerable)
                    {
                        if (_itemConstraint.ApplyTo(item).IsSuccess)
                            successfulItems++;
                    }

                    return new ConstraintResult(this, actual, (successfulItems == 1));
                }
                else
                {
                    return new ConstraintResult(this, actual, false);
                }
                
            }
        }

        /// <summary>
        /// The <see cref="Description"/> of what this constraint tests,
        /// for use in messages and in the <see cref="ConstraintResult"/>.
        /// </summary>
        public override string Description
        {
            get
            {
                if (_itemConstraint == null)
                {
                    return "length of 1";
                }
                else
                {
                    return PrefixConstraint.FormatDescription("one item", _itemConstraint);
                }
            }
        }
    }
}
