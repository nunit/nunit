// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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
using System.Collections;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// ConstraintExpression represents a compound constraint in the
    /// process of being constructed from a series of syntactic elements.
    ///
    /// Individual elements are appended to the expression as they are
    /// reorganized. When a constraint is appended, it is returned as the
    /// value of the operation so that modifiers may be applied. However,
    /// any partially built expression is attached to the constraint for
    /// later resolution. When an operator is appended, the partial
    /// expression is returned. If it's a self-resolving operator, then
    /// a ResolvableConstraintExpression is returned.
    /// </summary>
    public class ConstraintExpression
    {
        #region Instance Fields

        /// <summary>
        /// The ConstraintBuilder holding the elements recognized so far
        /// </summary>
        protected readonly ConstraintBuilder builder;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintExpression"/> class.
        /// </summary>
        public ConstraintExpression() : this(new ConstraintBuilder())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintExpression"/>
        /// class passing in a ConstraintBuilder, which may be pre-populated.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public ConstraintExpression(ConstraintBuilder builder)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));
            this.builder = builder;
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Returns a string representation of the expression as it
        /// currently stands. This should only be used for testing,
        /// since it has the side-effect of resolving the expression.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return builder.Resolve().ToString();
        }

        #endregion

        #region Append Methods

        /// <summary>
        /// Appends an operator to the expression and returns the
        /// resulting expression itself.
        /// </summary>
        public ConstraintExpression Append(ConstraintOperator op)
        {
            builder.Append(op);
            return this;
        }

        /// <summary>
        /// Appends a self-resolving operator to the expression and
        /// returns a new ResolvableConstraintExpression.
        /// </summary>
        public ResolvableConstraintExpression Append(SelfResolvingOperator op)
        {
            builder.Append(op);
            return new ResolvableConstraintExpression(builder);
        }

        /// <summary>
        /// Appends a constraint to the expression and returns that
        /// constraint, which is associated with the current state
        /// of the expression being built. Note that the constraint
        /// is not reduced at this time. For example, if there
        /// is a NotOperator on the stack we don't reduce and
        /// return a NotConstraint. The original constraint must
        /// be returned because it may support modifiers that
        /// are yet to be applied.
        /// </summary>
        public Constraint Append(Constraint constraint)
        {
            builder.Append(constraint);
            return constraint;
        }

        #endregion

        #region Not

        /// <summary>
        /// Returns a ConstraintExpression that negates any
        /// following constraint.
        /// </summary>
        public ConstraintExpression Not
        {
            get { return this.Append(new NotOperator()); }
        }

        /// <summary>
        /// Returns a ConstraintExpression that negates any
        /// following constraint.
        /// </summary>
        public ConstraintExpression No
        {
            get { return this.Append(new NotOperator()); }
        }

        #endregion

        #region All

        /// <summary>
        /// Returns a ConstraintExpression, which will apply
        /// the following constraint to all members of a collection,
        /// succeeding if all of them succeed.
        /// </summary>
        public ConstraintExpression All
        {
            get { return this.Append(new AllOperator()); }
        }

        #endregion

        #region Some

        /// <summary>
        /// Returns a ConstraintExpression, which will apply
        /// the following constraint to all members of a collection,
        /// succeeding if at least one of them succeeds.
        /// </summary>
        public ConstraintExpression Some
        {
            get { return this.Append(new SomeOperator()); }
        }

        #endregion

        #region None

        /// <summary>
        /// Returns a ConstraintExpression, which will apply
        /// the following constraint to all members of a collection,
        /// succeeding if all of them fail.
        /// </summary>
        public ConstraintExpression None
        {
            get { return this.Append(new NoneOperator()); }
        }

        #endregion

        #region Exactly(n)

        /// <summary>
        /// Returns a ConstraintExpression, which will apply
        /// the following constraint to all members of a collection,
        /// succeeding only if a specified number of them succeed.
        /// </summary>
        public ItemsConstraintExpression Exactly(int expectedCount)
        {
            builder.Append(new ExactCountOperator(expectedCount));
            return new ItemsConstraintExpression(builder);
        }

        #endregion

        #region One

        /// <summary>
        /// Returns a <see cref="ItemsConstraintExpression"/>, which will
        /// apply the following constraint to a collection of length one, succeeding
        /// only if exactly one of them succeeds.
        /// </summary>
        public ItemsConstraintExpression One
        {
            get
            {
                builder.Append(new ExactCountOperator(1));
                return new ItemsConstraintExpression(builder);
            }

        }

        #endregion

        #region Property

        /// <summary>
        /// Returns a new PropertyConstraintExpression, which will either
        /// test for the existence of the named property on the object
        /// being tested or apply any following constraint to that property.
        /// </summary>
        public ResolvableConstraintExpression Property(string name)
        {
            return this.Append(new PropOperator(name));
        }

        #endregion

        #region Length

        /// <summary>
        /// Returns a new ConstraintExpression, which will apply the following
        /// constraint to the Length property of the object being tested.
        /// </summary>
        public ResolvableConstraintExpression Length
        {
            get { return Property("Length"); }
        }

        #endregion

        #region Count

        /// <summary>
        /// Returns a new ConstraintExpression, which will apply the following
        /// constraint to the Count property of the object being tested.
        /// </summary>
        public ResolvableConstraintExpression Count
        {
            get { return Property("Count"); }
        }

        #endregion

        #region Message

        /// <summary>
        /// Returns a new ConstraintExpression, which will apply the following
        /// constraint to the Message property of the object being tested.
        /// </summary>
        public ResolvableConstraintExpression Message
        {
            get { return Property("Message"); }
        }

        #endregion

        #region InnerException

        /// <summary>
        /// Returns a new ConstraintExpression, which will apply the following
        /// constraint to the InnerException property of the object being tested.
        /// </summary>
        public ResolvableConstraintExpression InnerException
        {
            get { return Property("InnerException"); }
        }

        #endregion

        #region Attribute

        /// <summary>
        /// Returns a new AttributeConstraint checking for the
        /// presence of a particular attribute on an object.
        /// </summary>
        public ResolvableConstraintExpression Attribute(Type expectedType)
        {
            return this.Append(new AttributeOperator(expectedType));
        }

        /// <summary>
        /// Returns a new AttributeConstraint checking for the
        /// presence of a particular attribute on an object.
        /// </summary>
        public ResolvableConstraintExpression Attribute<TExpected>()
        {
            return Attribute(typeof(TExpected));
        }

        #endregion

        #region With

        /// <summary>
        /// With is currently a NOP - reserved for future use.
        /// </summary>
        public ConstraintExpression With
        {
            get { return this.Append(new WithOperator()); }
        }

        #endregion

        #region Matches

        /// <summary>
        /// Returns the constraint provided as an argument - used to allow custom
        /// custom constraints to easily participate in the syntax.
        /// </summary>
        public Constraint Matches(IResolveConstraint constraint)
        {
            return this.Append((Constraint)constraint.Resolve());
        }

        /// <summary>
        /// Returns the constraint provided as an argument - used to allow custom
        /// custom constraints to easily participate in the syntax.
        /// </summary>
        public Constraint Matches<TActual>(Predicate<TActual> predicate)
        {
            return this.Append(new PredicateConstraint<TActual>(predicate));
        }

        #endregion

        #region Null

        /// <summary>
        /// Returns a constraint that tests for null
        /// </summary>
        public NullConstraint Null
        {
            get { return (NullConstraint)this.Append(new NullConstraint()); }
        }

        #endregion

        #region True

        /// <summary>
        /// Returns a constraint that tests for True
        /// </summary>
        public TrueConstraint True
        {
            get { return (TrueConstraint)this.Append(new TrueConstraint()); }
        }

        #endregion

        #region False

        /// <summary>
        /// Returns a constraint that tests for False
        /// </summary>
        public FalseConstraint False
        {
            get { return (FalseConstraint)this.Append(new FalseConstraint()); }
        }

        #endregion

        #region Positive

        /// <summary>
        /// Returns a constraint that tests for a positive value
        /// </summary>
        public GreaterThanConstraint Positive
        {
            get { return (GreaterThanConstraint)this.Append(new GreaterThanConstraint(0)); }
        }

        #endregion

        #region Negative

        /// <summary>
        /// Returns a constraint that tests for a negative value
        /// </summary>
        public LessThanConstraint Negative
        {
            get { return (LessThanConstraint)this.Append(new LessThanConstraint(0)); }
        }

        #endregion

        #region Zero

        /// <summary>
        /// Returns a constraint that tests if item is equal to zero
        /// </summary>
        public EqualConstraint Zero
        {
            get { return (EqualConstraint)this.Append(new EqualConstraint(0)); }
        }

        #endregion

        #region NaN

        /// <summary>
        /// Returns a constraint that tests for NaN
        /// </summary>
        public NaNConstraint NaN
        {
            get { return (NaNConstraint)this.Append(new NaNConstraint()); }
        }

        #endregion

        #region Empty

        /// <summary>
        /// Returns a constraint that tests for empty
        /// </summary>
        public EmptyConstraint Empty
        {
            get { return (EmptyConstraint)this.Append(new EmptyConstraint()); }
        }

        #endregion

        #region Unique

        /// <summary>
        /// Returns a constraint that tests whether a collection
        /// contains all unique items.
        /// </summary>
        public UniqueItemsConstraint Unique
        {
            get { return (UniqueItemsConstraint)this.Append(new UniqueItemsConstraint()); }
        }

        #endregion

        #region BinarySerializable

#if !NETSTANDARD1_6
        /// <summary>
        /// Returns a constraint that tests whether an object graph is serializable in binary format.
        /// </summary>
        public BinarySerializableConstraint BinarySerializable
        {
            get { return (BinarySerializableConstraint)this.Append(new BinarySerializableConstraint()); }
        }
#endif

        #endregion

        #region XmlSerializable

#if !NETSTANDARD1_6
        /// <summary>
        /// Returns a constraint that tests whether an object graph is serializable in XML format.
        /// </summary>
        public XmlSerializableConstraint XmlSerializable
        {
            get { return (XmlSerializableConstraint)this.Append(new XmlSerializableConstraint()); }
        }
#endif

        #endregion

        #region EqualTo

        /// <summary>
        /// Returns a constraint that tests two items for equality
        /// </summary>
        public EqualConstraint EqualTo(object expected)
        {
            return (EqualConstraint)this.Append(new EqualConstraint(expected));
        }

        #endregion

        #region SameAs

        /// <summary>
        /// Returns a constraint that tests that two references are the same object
        /// </summary>
        public SameAsConstraint SameAs(object expected)
        {
            return (SameAsConstraint)this.Append(new SameAsConstraint(expected));
        }

        #endregion

        #region GreaterThan

        /// <summary>
        /// Returns a constraint that tests whether the
        /// actual value is greater than the supplied argument
        /// </summary>
        public GreaterThanConstraint GreaterThan(object expected)
        {
            return (GreaterThanConstraint)this.Append(new GreaterThanConstraint(expected));
        }

        #endregion

        #region GreaterThanOrEqualTo

        /// <summary>
        /// Returns a constraint that tests whether the
        /// actual value is greater than or equal to the supplied argument
        /// </summary>
        public GreaterThanOrEqualConstraint GreaterThanOrEqualTo(object expected)
        {
            return (GreaterThanOrEqualConstraint)this.Append(new GreaterThanOrEqualConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that tests whether the
        /// actual value is greater than or equal to the supplied argument
        /// </summary>
        public GreaterThanOrEqualConstraint AtLeast(object expected)
        {
            return (GreaterThanOrEqualConstraint)this.Append(new GreaterThanOrEqualConstraint(expected));
        }

        #endregion

        #region LessThan

        /// <summary>
        /// Returns a constraint that tests whether the
        /// actual value is less than the supplied argument
        /// </summary>
        public LessThanConstraint LessThan(object expected)
        {
            return (LessThanConstraint)this.Append(new LessThanConstraint(expected));
        }

        #endregion

        #region LessThanOrEqualTo

        /// <summary>
        /// Returns a constraint that tests whether the
        /// actual value is less than or equal to the supplied argument
        /// </summary>
        public LessThanOrEqualConstraint LessThanOrEqualTo(object expected)
        {
            return (LessThanOrEqualConstraint)this.Append(new LessThanOrEqualConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that tests whether the
        /// actual value is less than or equal to the supplied argument
        /// </summary>
        public LessThanOrEqualConstraint AtMost(object expected)
        {
            return (LessThanOrEqualConstraint)this.Append(new LessThanOrEqualConstraint(expected));
        }

        #endregion

        #region TypeOf

        /// <summary>
        /// Returns a constraint that tests whether the actual
        /// value is of the exact type supplied as an argument.
        /// </summary>
        public ExactTypeConstraint TypeOf(Type expectedType)
        {
            return (ExactTypeConstraint)this.Append(new ExactTypeConstraint(expectedType));
        }

        /// <summary>
        /// Returns a constraint that tests whether the actual
        /// value is of the exact type supplied as an argument.
        /// </summary>
        public ExactTypeConstraint TypeOf<TExpected>()
        {
            return (ExactTypeConstraint)this.Append(new ExactTypeConstraint(typeof(TExpected)));
        }

        #endregion

        #region InstanceOf

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is of the type supplied as an argument or a derived type.
        /// </summary>
        public InstanceOfTypeConstraint InstanceOf(Type expectedType)
        {
            return (InstanceOfTypeConstraint)this.Append(new InstanceOfTypeConstraint(expectedType));
        }

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is of the type supplied as an argument or a derived type.
        /// </summary>
        public InstanceOfTypeConstraint InstanceOf<TExpected>()
        {
            return (InstanceOfTypeConstraint)this.Append(new InstanceOfTypeConstraint(typeof(TExpected)));
        }

        #endregion

        #region AssignableFrom

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is assignable from the type supplied as an argument.
        /// </summary>
        public AssignableFromConstraint AssignableFrom(Type expectedType)
        {
            return (AssignableFromConstraint)this.Append(new AssignableFromConstraint(expectedType));
        }

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is assignable from the type supplied as an argument.
        /// </summary>
        public AssignableFromConstraint AssignableFrom<TExpected>()
        {
            return (AssignableFromConstraint)this.Append(new AssignableFromConstraint(typeof(TExpected)));
        }

        #endregion

        #region AssignableTo

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is assignable from the type supplied as an argument.
        /// </summary>
        public AssignableToConstraint AssignableTo(Type expectedType)
        {
            return (AssignableToConstraint)this.Append(new AssignableToConstraint(expectedType));
        }

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is assignable from the type supplied as an argument.
        /// </summary>
        public AssignableToConstraint AssignableTo<TExpected>()
        {
            return (AssignableToConstraint)this.Append(new AssignableToConstraint(typeof(TExpected)));
        }

        #endregion

        #region EquivalentTo

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is a collection containing the same elements as the
        /// collection supplied as an argument.
        /// </summary>
        public CollectionEquivalentConstraint EquivalentTo(IEnumerable expected)
        {
            return (CollectionEquivalentConstraint)this.Append(new CollectionEquivalentConstraint(expected));
        }

        #endregion

        #region SubsetOf

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is a subset of the collection supplied as an argument.
        /// </summary>
        public CollectionSubsetConstraint SubsetOf(IEnumerable expected)
        {
            return (CollectionSubsetConstraint)this.Append(new CollectionSubsetConstraint(expected));
        }

        #endregion

        #region SupersetOf

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is a superset of the collection supplied as an argument.
        /// </summary>
        public CollectionSupersetConstraint SupersetOf(IEnumerable expected)
        {
            return (CollectionSupersetConstraint)this.Append(new CollectionSupersetConstraint(expected));
        }

        #endregion

        #region Ordered

        /// <summary>
        /// Returns a constraint that tests whether a collection is ordered
        /// </summary>
        public CollectionOrderedConstraint Ordered
        {
            get { return (CollectionOrderedConstraint)this.Append(new CollectionOrderedConstraint()); }
        }

        #endregion

        #region Member

        /// <summary>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </summary>
        public SomeItemsConstraint Member(object expected)
        {
            return (SomeItemsConstraint)this.Append(new SomeItemsConstraint(new EqualConstraint(expected)));
        }

        #endregion

        #region Contains

        /// <summary>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </summary>
        public SomeItemsConstraint Contains(object expected)
        {
            return (SomeItemsConstraint)this.Append(new SomeItemsConstraint(new EqualConstraint(expected)));
        }

        /// <summary>
        /// Returns a new ContainsConstraint. This constraint
        /// will, in turn, make use of the appropriate second-level
        /// constraint, depending on the type of the actual argument.
        /// This overload is only used if the item sought is a string,
        /// since any other type implies that we are looking for a
        /// collection member.
        /// </summary>
        public ContainsConstraint Contains(string expected)
        {
            return (ContainsConstraint)this.Append(new ContainsConstraint(expected));
        }

        /// <summary>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </summary>
        public SomeItemsConstraint Contain(object expected)
        {
            return Contains(expected);
        }

        /// <summary>
        /// Returns a new ContainsConstraint. This constraint
        /// will, in turn, make use of the appropriate second-level
        /// constraint, depending on the type of the actual argument.
        /// This overload is only used if the item sought is a string,
        /// since any other type implies that we are looking for a
        /// collection member.
        /// </summary>
        public ContainsConstraint Contain(string expected)
        {
            return Contains(expected);
        }

        #endregion

        #region DictionaryContains
        /// <summary>
        /// Returns a new DictionaryContainsKeyConstraint checking for the
        /// presence of a particular key in the Dictionary key collection.
        /// </summary>
        /// <param name="expected">The key to be matched in the Dictionary key collection</param>
        public DictionaryContainsKeyConstraint ContainKey(object expected)
        {
            return (DictionaryContainsKeyConstraint)this.Append(new DictionaryContainsKeyConstraint(expected));
        }

        /// <summary>
        /// Returns a new DictionaryContainsValueConstraint checking for the
        /// presence of a particular value in the Dictionary value collection.
        /// </summary>
        /// <param name="expected">The value to be matched in the Dictionary value collection</param>
        public DictionaryContainsValueConstraint ContainValue(object expected)
        {
            return (DictionaryContainsValueConstraint)this.Append(new DictionaryContainsValueConstraint(expected));
        }
        #endregion

        #region StringContaining

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value contains the substring supplied as an argument.
        /// </summary>
        [Obsolete("Deprecated, use Contains")]
        public SubstringConstraint StringContaining(string expected)
        {
            return (SubstringConstraint)this.Append(new SubstringConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value contains the substring supplied as an argument.
        /// </summary>
        [Obsolete("Deprecated, use Contains")]
        public SubstringConstraint ContainsSubstring(string expected)
        {
            return (SubstringConstraint)this.Append(new SubstringConstraint(expected));
        }

        #endregion

        #region StartsWith

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value starts with the substring supplied as an argument.
        /// </summary>
        public StartsWithConstraint StartWith(string expected)
        {
            return (StartsWithConstraint)this.Append(new StartsWithConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value starts with the substring supplied as an argument.
        /// </summary>
        public StartsWithConstraint StartsWith(string expected)
        {
            return (StartsWithConstraint)this.Append(new StartsWithConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value starts with the substring supplied as an argument.
        /// </summary>
        [Obsolete("Deprecated, use Does.StartWith or StartsWith")]
        public StartsWithConstraint StringStarting(string expected)
        {
            return (StartsWithConstraint)this.Append(new StartsWithConstraint(expected));
        }

        #endregion

        #region EndsWith

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value ends with the substring supplied as an argument.
        /// </summary>
        public EndsWithConstraint EndWith(string expected)
        {
            return (EndsWithConstraint)this.Append(new EndsWithConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value ends with the substring supplied as an argument.
        /// </summary>
        public EndsWithConstraint EndsWith(string expected)
        {
            return (EndsWithConstraint)this.Append(new EndsWithConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value ends with the substring supplied as an argument.
        /// </summary>
        [Obsolete("Deprecated, use Does.EndWith or EndsWith")]
        public EndsWithConstraint StringEnding(string expected)
        {
            return (EndsWithConstraint)this.Append(new EndsWithConstraint(expected));
        }

        #endregion

        #region Matches

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value matches the regular expression supplied as an argument.
        /// </summary>
        public RegexConstraint Match(string pattern)
        {
            return (RegexConstraint)this.Append(new RegexConstraint(pattern));
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value matches the regular expression supplied as an argument.
        /// </summary>
        public RegexConstraint Matches(string pattern)
        {
            return (RegexConstraint)this.Append(new RegexConstraint(pattern));
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value matches the regular expression supplied as an argument.
        /// </summary>
        [Obsolete("Deprecated, use Does.Match or Matches")]
        public RegexConstraint StringMatching(string pattern)
        {
            return (RegexConstraint)this.Append(new RegexConstraint(pattern));
        }

        #endregion

        #region SamePath

        /// <summary>
        /// Returns a constraint that tests whether the path provided
        /// is the same as an expected path after canonicalization.
        /// </summary>
        public SamePathConstraint SamePath(string expected)
        {
            return (SamePathConstraint)this.Append(new SamePathConstraint(expected));
        }

        #endregion

        #region SubPath

        /// <summary>
        /// Returns a constraint that tests whether the path provided
        /// is the a subpath of the expected path after canonicalization.
        /// </summary>
        public SubPathConstraint SubPathOf(string expected)
        {
            return (SubPathConstraint)this.Append(new SubPathConstraint(expected));
        }

        #endregion

        #region SamePathOrUnder

        /// <summary>
        /// Returns a constraint that tests whether the path provided
        /// is the same path or under an expected path after canonicalization.
        /// </summary>
        public SamePathOrUnderConstraint SamePathOrUnder(string expected)
        {
            return (SamePathOrUnderConstraint)this.Append(new SamePathOrUnderConstraint(expected));
        }

        #endregion

        #region InRange
        /// <summary>
        /// Returns a constraint that tests whether the actual value falls
        /// inclusively within a specified range.
        /// </summary>
        /// <param name="from">Inclusive beginning of the range.</param>
        /// <param name="to">Inclusive end of the range.</param>
        public RangeConstraint InRange(object from, object to)
        {
            return (RangeConstraint)this.Append(new RangeConstraint(from, to));
        }

        #endregion

        #region Exist

        /// <summary>
        /// Returns a constraint that succeeds if the value
        /// is a file or directory and it exists.
        /// </summary>
        public Constraint Exist
        {
            get { return Append(new FileOrDirectoryExistsConstraint()); }
        }

        #endregion

        #region AnyOf

        /// <summary>
        /// Returns a constraint that tests if an item is equal to any of parameters
        /// </summary>
        /// <param name="expected">Expected values</param>
        public Constraint AnyOf(params object[] expected)
        {
            if (expected == null)
            {
                expected = new object[] { null };
            }

            return Append(new AnyOfConstraint(expected));
        }

        #endregion
    }
}
