// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

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
        /// <summary>
        /// The ConstraintBuilder holding the elements recognized so far
        /// </summary>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        // Disregarding naming convention for back-compat
        protected readonly ConstraintBuilder builder;
#pragma warning restore IDE1006
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
            return builder.Resolve().ToString()!;
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
        public T Append<T>(T constraint)
            where T : Constraint
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
        public ConstraintExpression Not => Append(new NotOperator());

        /// <summary>
        /// Returns a ConstraintExpression that negates any
        /// following constraint.
        /// </summary>
        public ConstraintExpression No => Append(new NotOperator());

        #endregion

        #region All

        /// <summary>
        /// Returns a ConstraintExpression, which will apply
        /// the following constraint to all members of a collection,
        /// succeeding if all of them succeed.
        /// </summary>
        public ConstraintExpression All => Append(new AllOperator());

        #endregion

        #region Some

        /// <summary>
        /// Returns a ConstraintExpression, which will apply
        /// the following constraint to all members of a collection,
        /// succeeding if at least one of them succeeds.
        /// </summary>
        public ConstraintExpression Some => Append(new SomeOperator());

        #endregion

        #region None

        /// <summary>
        /// Returns a ConstraintExpression, which will apply
        /// the following constraint to all members of a collection,
        /// succeeding if all of them fail.
        /// </summary>
        public ConstraintExpression None => Append(new NoneOperator());

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
            return Append(new PropOperator(name));
        }

        #endregion

        #region Length

        /// <summary>
        /// Returns a new ConstraintExpression, which will apply the following
        /// constraint to the Length property of the object being tested.
        /// </summary>
        public ResolvableConstraintExpression Length => Property("Length");

        #endregion

        #region Count

        /// <summary>
        /// Returns a new ConstraintExpression, which will apply the following
        /// constraint to the Count property of the object being tested.
        /// </summary>
        public ResolvableConstraintExpression Count => Property("Count");

        #endregion

        #region Message

        /// <summary>
        /// Returns a new ConstraintExpression, which will apply the following
        /// constraint to the Message property of the object being tested.
        /// </summary>
        public ResolvableConstraintExpression Message => Property("Message");

        #endregion

        #region InnerException

        /// <summary>
        /// Returns a new ConstraintExpression, which will apply the following
        /// constraint to the InnerException property of the object being tested.
        /// </summary>
        public ResolvableConstraintExpression InnerException => Property("InnerException");

        #endregion

        #region Attribute

        /// <summary>
        /// Returns a new AttributeConstraint checking for the
        /// presence of a particular attribute on an object.
        /// </summary>
        public ResolvableConstraintExpression Attribute(Type expectedType)
        {
            return Append(new AttributeOperator(expectedType));
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
        public ConstraintExpression With => Append(new WithOperator());

        #endregion

        #region Matches

        /// <summary>
        /// Returns the constraint provided as an argument - used to allow custom
        /// custom constraints to easily participate in the syntax.
        /// </summary>
        public Constraint Matches(IResolveConstraint constraint)
        {
            return Append((Constraint)constraint.Resolve());
        }

        /// <summary>
        /// Returns the constraint provided as an argument - used to allow custom
        /// custom constraints to easily participate in the syntax.
        /// </summary>
        public Constraint Matches<TActual>(Predicate<TActual> predicate)
        {
            return Append(new PredicateConstraint<TActual>(predicate));
        }

        #endregion

        #region Null

        /// <summary>
        /// Returns a constraint that tests for null
        /// </summary>
        public NullConstraint Null => Append(new NullConstraint());

        #endregion

        #region Null

        /// <summary>
        /// Returns a constraint that tests for default value
        /// </summary>
        public DefaultConstraint Default => Append(new DefaultConstraint());

        #endregion

        #region True

        /// <summary>
        /// Returns a constraint that tests for True
        /// </summary>
        public TrueConstraint True => Append(new TrueConstraint());

        #endregion

        #region False

        /// <summary>
        /// Returns a constraint that tests for False
        /// </summary>
        public FalseConstraint False => Append(new FalseConstraint());

        #endregion

        #region Positive

        /// <summary>
        /// Returns a constraint that tests for a positive value
        /// </summary>
        public GreaterThanConstraint Positive => Append(new GreaterThanConstraint(0));

        #endregion

        #region Negative

        /// <summary>
        /// Returns a constraint that tests for a negative value
        /// </summary>
        public LessThanConstraint Negative => Append(new LessThanConstraint(0));

        #endregion

        #region Zero

        /// <summary>
        /// Returns a constraint that tests if item is equal to zero
        /// </summary>
        public EqualConstraint Zero => Append(new EqualConstraint(0));

        #endregion

        #region NaN

        /// <summary>
        /// Returns a constraint that tests for NaN
        /// </summary>
        public NaNConstraint NaN => Append(new NaNConstraint());

        #endregion

        #region MultipleOf, Even, Odd

        /// <summary>
        /// Returns a constraint that tests for a number to be a multiple of another.
        /// </summary>
        public MultipleOfConstraint MultipleOf(int multiple) => Append(new MultipleOfConstraint(multiple));

        /// <summary>
        /// Returns a constraint that tests for a number to be even (i.e. a multiple of two)
        /// </summary>
        public MultipleOfConstraint Even => MultipleOf(2);

        /// <summary>
        /// Returns a constraint that tests for a number to be odd.
        /// </summary>
        public MultipleOfConstraint Odd => Not.Even;

        #endregion

        #region Empty

        /// <summary>
        /// Returns a constraint that tests for empty
        /// </summary>
        public EmptyConstraint Empty => Append(new EmptyConstraint());

        #endregion

        #region WhiteSpace

        /// <summary>
        /// Returns a constraint that tests for white-space
        /// </summary>
        public WhiteSpaceConstraint WhiteSpace => Append(new WhiteSpaceConstraint());

        #endregion

        #region Unique

        /// <summary>
        /// Returns a constraint that tests whether a collection
        /// contains all unique items.
        /// </summary>
        public UniqueItemsConstraint Unique => Append(new UniqueItemsConstraint());

        #endregion

        /// <summary>
        /// Returns a constraint that tests whether an object graph is serializable in XML format.
        /// </summary>
        public XmlSerializableConstraint XmlSerializable => Append(new XmlSerializableConstraint());

        #region EqualTo

        /// <summary>
        /// Returns a constraint that tests two items for equality
        /// </summary>
        public EqualConstraint EqualTo(object? expected)
        {
            return Append(new EqualConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two items for equality
        /// </summary>
        public EqualConstraint<T> EqualTo<T>(T? expected)
        {
            return Append(new EqualConstraint<T>(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two strings for equality
        /// </summary>
        public EqualStringConstraint EqualTo(string? expected)
        {
            return Append(new EqualStringConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two date time offset instances for equality
        /// </summary>
        public EqualDateTimeOffsetConstraint EqualTo(DateTimeOffset expected)
        {
            return Append(new EqualDateTimeOffsetConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two date time instances for equality
        /// </summary>
        public EqualTimeBaseConstraint<DateTime> EqualTo(DateTime expected)
        {
            return Append(new EqualTimeBaseConstraint<DateTime>(expected, x => x.Ticks));
        }

        /// <summary>
        /// Returns a constraint that tests two timespan instances for equality
        /// </summary>
        public EqualTimeBaseConstraint<TimeSpan> EqualTo(TimeSpan expected)
        {
            return Append(new EqualTimeBaseConstraint<TimeSpan>(expected, x => x.Ticks));
        }

        /// <summary>
        /// Returns a constraint that tests two numbers for equality
        /// </summary>
        public EqualNumericConstraint<double> EqualTo(double expected)
        {
            return Append(new EqualNumericConstraint<double>(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two numbers for equality
        /// </summary>
        public EqualNumericConstraint<float> EqualTo(float expected)
        {
            return Append(new EqualNumericConstraint<float>(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two numbers for equality
        /// </summary>
        public EqualNumericConstraint<decimal> EqualTo(decimal expected)
        {
            return Append(new EqualNumericConstraint<decimal>(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two numbers for equality
        /// </summary>
        public EqualNumericConstraint<long> EqualTo(long expected)
        {
            return Append(new EqualNumericConstraint<long>(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two numbers for equality
        /// </summary>
        public EqualNumericConstraint<int> EqualTo(int expected)
        {
            return Append(new EqualNumericConstraint<int>(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two numbers for equality
        /// </summary>
        public EqualNumericConstraint<short> EqualTo(short expected)
        {
            return Append(new EqualNumericConstraint<short>(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two numbers for equality
        /// </summary>
        public EqualNumericConstraint<byte> EqualTo(byte expected)
        {
            return Append(new EqualNumericConstraint<byte>(expected));
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
#pragma warning disable CS3001 // Argument type is not CLS-compliant

        /// <summary>
        /// Returns a constraint that tests two numbers for equality
        /// </summary>
        public EqualNumericConstraint<ulong> EqualTo(ulong expected)
        {
            return Append(new EqualNumericConstraint<ulong>(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two numbers for equality
        /// </summary>
        public EqualNumericConstraint<uint> EqualTo(uint expected)
        {
            return Append(new EqualNumericConstraint<uint>(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two numbers for equality
        /// </summary>
        public EqualNumericConstraint<ushort> EqualTo(ushort expected)
        {
            return Append(new EqualNumericConstraint<ushort>(expected));
        }

        /// <summary>
        /// Returns a constraint that tests two numbers for equality
        /// </summary>
        public EqualNumericConstraint<sbyte> EqualTo(sbyte expected)
        {
            return Append(new EqualNumericConstraint<sbyte>(expected));
        }

#pragma warning restore CS3001 // Argument type is not CLS-compliant
#pragma warning restore CS3002 // Return type is not CLS-compliant

        #endregion

        #region SameAs

        /// <summary>
        /// Returns a constraint that tests that two references are the same object
        /// </summary>
        public SameAsConstraint SameAs(object? expected)
        {
            return Append(new SameAsConstraint(expected));
        }

        #endregion

        #region GreaterThan

        /// <summary>
        /// Returns a constraint that tests whether the
        /// actual value is greater than the supplied argument
        /// </summary>
        public GreaterThanConstraint GreaterThan(object expected)
        {
            return Append(new GreaterThanConstraint(expected));
        }

        #endregion

        #region GreaterThanOrEqualTo

        /// <summary>
        /// Returns a constraint that tests whether the
        /// actual value is greater than or equal to the supplied argument
        /// </summary>
        public GreaterThanOrEqualConstraint GreaterThanOrEqualTo(object expected)
        {
            return Append(new GreaterThanOrEqualConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that tests whether the
        /// actual value is greater than or equal to the supplied argument
        /// </summary>
        public GreaterThanOrEqualConstraint AtLeast(object expected)
        {
            return Append(new GreaterThanOrEqualConstraint(expected));
        }

        #endregion

        #region LessThan

        /// <summary>
        /// Returns a constraint that tests whether the
        /// actual value is less than the supplied argument
        /// </summary>
        public LessThanConstraint LessThan(object expected)
        {
            return Append(new LessThanConstraint(expected));
        }

        #endregion

        #region LessThanOrEqualTo

        /// <summary>
        /// Returns a constraint that tests whether the
        /// actual value is less than or equal to the supplied argument
        /// </summary>
        public LessThanOrEqualConstraint LessThanOrEqualTo(object expected)
        {
            return Append(new LessThanOrEqualConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that tests whether the
        /// actual value is less than or equal to the supplied argument
        /// </summary>
        public LessThanOrEqualConstraint AtMost(object expected)
        {
            return Append(new LessThanOrEqualConstraint(expected));
        }

        #endregion

        #region TypeOf

        /// <summary>
        /// Returns a constraint that tests whether the actual
        /// value is of the exact type supplied as an argument.
        /// </summary>
        public ExactTypeConstraint TypeOf(Type expectedType)
        {
            return Append(new ExactTypeConstraint(expectedType));
        }

        /// <summary>
        /// Returns a constraint that tests whether the actual
        /// value is of the exact type supplied as an argument.
        /// </summary>
        public ExactTypeConstraint<TExpected> TypeOf<TExpected>()
        {
            return Append(new ExactTypeConstraint<TExpected>());
        }

        #endregion

        #region InstanceOf

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is of the type supplied as an argument or a derived type.
        /// </summary>
        public InstanceOfTypeConstraint InstanceOf(Type expectedType)
        {
            return Append(new InstanceOfTypeConstraint(expectedType));
        }

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is of the type supplied as an argument or a derived type.
        /// </summary>
        public InstanceOfTypeConstraint<TExpected> InstanceOf<TExpected>()
        {
            return Append(new InstanceOfTypeConstraint<TExpected>());
        }

        #endregion

        #region AssignableFrom

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is assignable from the type supplied as an argument.
        /// </summary>
        public AssignableFromConstraint AssignableFrom(Type expectedType)
        {
            return Append(new AssignableFromConstraint(expectedType));
        }

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is assignable from the type supplied as an argument.
        /// </summary>
        public AssignableFromConstraint<TExpected> AssignableFrom<TExpected>()
        {
            return Append(new AssignableFromConstraint<TExpected>());
        }

        #endregion

        #region AssignableTo

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is assignable from the type supplied as an argument.
        /// </summary>
        public AssignableToConstraint AssignableTo(Type expectedType)
        {
            return Append(new AssignableToConstraint(expectedType));
        }

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is assignable from the type supplied as an argument.
        /// </summary>
        public AssignableToConstraint<TExpected> AssignableTo<TExpected>()
        {
            return Append(new AssignableToConstraint<TExpected>());
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
            return Append(new CollectionEquivalentConstraint(expected));
        }

        #endregion

        #region SubsetOf

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is a subset of the collection supplied as an argument.
        /// </summary>
        public CollectionSubsetConstraint SubsetOf(IEnumerable expected)
        {
            return Append(new CollectionSubsetConstraint(expected));
        }

        #endregion

        #region SupersetOf

        /// <summary>
        /// Returns a constraint that tests whether the actual value
        /// is a superset of the collection supplied as an argument.
        /// </summary>
        public CollectionSupersetConstraint SupersetOf(IEnumerable expected)
        {
            return Append(new CollectionSupersetConstraint(expected));
        }

        #endregion

        #region Ordered

        /// <summary>
        /// Returns a constraint that tests whether a collection is ordered
        /// </summary>
        public CollectionOrderedConstraint Ordered => Append(new CollectionOrderedConstraint());

        #endregion

        #region Member

        /// <summary>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </summary>
        public SomeItemsConstraint Member(object? expected)
        {
            return Append(new SomeItemsConstraint(new EqualConstraint(expected)));
        }

        /// <summary>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </summary>
        public SomeItemsConstraint<T> Member<T>(T? expected)
        {
            return Append(new SomeItemsConstraint<T>(new EqualConstraint<T>(expected)));
        }

        #endregion

        #region Contains

        /// <summary>
        /// <para>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </para>
        /// <para>
        /// To search for a substring instead of a collection element, use the
        /// <see cref="Contains(string)"/> overload.
        /// </para>
        /// </summary>
        public SomeItemsConstraint Contains(object? expected)
        {
            return Append(new SomeItemsConstraint(new EqualConstraint(expected)));
        }

        /// <summary>
        /// <para>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </para>
        /// <para>
        /// To search for a substring instead of a collection element, use the
        /// <see cref="Contains(string)"/> overload.
        /// </para>
        /// </summary>
        public SomeItemsConstraint<T> Contains<T>(T? expected)
        {
            return Append(new SomeItemsConstraint<T>(new EqualConstraint<T>(expected)));
        }

        /// <summary>
        /// <para>
        /// Returns a new ContainsConstraint. This constraint
        /// will, in turn, make use of the appropriate second-level
        /// constraint, depending on the type of the actual argument.
        /// </para>
        /// <para>
        /// To search for a collection element instead of a substring, use the
        /// <see cref="Contains(object)"/> overload.
        /// </para>
        /// </summary>
        public ContainsConstraint Contains(string? expected)
        {
            return Append(new ContainsConstraint(expected));
        }

        /// <summary>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </summary>
        public SomeItemsConstraint Contain(object? expected)
        {
            return Contains(expected);
        }

        /// <summary>
        /// Returns a new <see cref="SomeItemsConstraint"/> checking for the
        /// presence of a particular object in the collection.
        /// </summary>
        public SomeItemsConstraint<T> Contain<T>(T? expected)
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
        public ContainsConstraint Contain(string? expected)
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
            return Append(new DictionaryContainsKeyConstraint(expected));
        }

        /// <summary>
        /// Returns a new DictionaryContainsValueConstraint checking for the
        /// presence of a particular value in the Dictionary value collection.
        /// </summary>
        /// <param name="expected">The value to be matched in the Dictionary value collection</param>
        public DictionaryContainsValueConstraint ContainValue(object? expected)
        {
            return Append(new DictionaryContainsValueConstraint(expected));
        }
        #endregion

        #region StartsWith

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value starts with the substring supplied as an argument.
        /// </summary>
        public StartsWithConstraint StartWith(string expected)
        {
            return Append(new StartsWithConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value starts with the substring supplied as an argument.
        /// </summary>
        public StartsWithConstraint StartsWith(string expected)
        {
            return Append(new StartsWithConstraint(expected));
        }

        #endregion

        #region EndsWith

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value ends with the substring supplied as an argument.
        /// </summary>
        public EndsWithConstraint EndWith(string expected)
        {
            return Append(new EndsWithConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value ends with the substring supplied as an argument.
        /// </summary>
        public EndsWithConstraint EndsWith(string expected)
        {
            return Append(new EndsWithConstraint(expected));
        }

        #endregion

        #region Matches

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value matches the regular expression supplied as an argument.
        /// </summary>
        public RegexConstraint Match([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
        {
            return Append(new RegexConstraint(pattern));
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value matches the regular expression supplied as an argument.
        /// </summary>
        public RegexConstraint Match(Regex regex)
        {
            return Append(new RegexConstraint(regex));
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value matches the regular expression supplied as an argument.
        /// </summary>
        public RegexConstraint Matches([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
        {
            return Append(new RegexConstraint(pattern));
        }

        /// <summary>
        /// Returns a constraint that succeeds if the actual
        /// value matches the regular expression supplied as an argument.
        /// </summary>
        public RegexConstraint Matches(Regex regex)
        {
            return Append(new RegexConstraint(regex));
        }

        #endregion

        #region SamePath

        /// <summary>
        /// Returns a constraint that tests whether the path provided
        /// is the same as an expected path after canonicalization.
        /// </summary>
        public SamePathConstraint SamePath(string expected)
        {
            return Append(new SamePathConstraint(expected));
        }

        #endregion

        #region SubPath

        /// <summary>
        /// Returns a constraint that tests whether the path provided
        /// is the a subpath of the expected path after canonicalization.
        /// </summary>
        public SubPathConstraint SubPathOf(string expected)
        {
            return Append(new SubPathConstraint(expected));
        }

        #endregion

        #region SamePathOrUnder

        /// <summary>
        /// Returns a constraint that tests whether the path provided
        /// is the same path or under an expected path after canonicalization.
        /// </summary>
        public SamePathOrUnderConstraint SamePathOrUnder(string expected)
        {
            return Append(new SamePathOrUnderConstraint(expected));
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
            return Append(new RangeConstraint(from, to));
        }

        #endregion

        #region Exist

        /// <summary>
        /// Returns a constraint that succeeds if the value
        /// is a file or directory and it exists.
        /// </summary>
        public Constraint Exist => Append(new FileOrDirectoryExistsConstraint());

        #endregion

        #region AnyOf

        /// <summary>
        /// Returns a constraint that tests if an item is equal to any of parameters
        /// </summary>
        /// <param name="expected">Expected values</param>
        public AnyOfConstraint AnyOf(params object?[]? expected)
        {
            if (expected is null)
            {
                expected = new object?[] { null };
            }

            return Append(new AnyOfConstraint(expected));
        }

        /// <summary>
        /// Returns a constraint that tests if an item is equal to any of expected values
        /// </summary>
        /// <param name="expected">Expected values</param>
        public AnyOfConstraint AnyOf(ICollection expected)
        {
            return Append(new AnyOfConstraint(expected));
        }

        #endregion

        #region ItemAt

        /// <summary>
        /// Returns a new IndexerConstraintExpression, which will
        /// apply any following constraint to that indexer value.
        /// </summary>
        /// <param name="indexArgs">Index accessor values.</param>
        public ConstraintExpression ItemAt(params object[] indexArgs)
        {
            return Append(new IndexerOperator(indexArgs));
        }

        #endregion
    }
}
