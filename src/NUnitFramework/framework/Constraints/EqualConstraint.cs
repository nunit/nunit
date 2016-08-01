// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// EqualConstraint is able to compare an actual value with the
    /// expected value provided in its constructor. Two objects are 
    /// considered equal if both are null, or if both have the same 
    /// value. NUnit has special semantics for some object types.
    /// </summary>
    public class EqualConstraint : Constraint
    {
        #region Static and Instance Fields

        private readonly object _expected;

        private Tolerance _tolerance = Tolerance.Default;

        /// <summary>
        /// NUnitEqualityComparer used to test equality.
        /// </summary>
        private NUnitEqualityComparer _comparer = new NUnitEqualityComparer();

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EqualConstraint"/> class.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        public EqualConstraint(object expected)
            : base(expected)
        {
            AdjustArgumentIfNeeded(ref expected);

            _expected = expected;
            ClipStrings = true;
        }
        #endregion

        #region Properties

        // TODO: Remove public properties 
        // They are only used by EqualConstraintResult
        // EqualConstraint should inject them into the constructor.

        /// <summary>
        /// Gets the tolerance for this comparison.
        /// </summary>
        /// <value>
        /// The tolerance.
        /// </value>
        public Tolerance Tolerance
        {
            get { return _tolerance; }
        }

        /// <summary>
        /// Gets a value indicating whether to compare case insensitive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if comparing case insensitive; otherwise, <c>false</c>.
        /// </value>
        public bool CaseInsensitive
        {
            get { return _comparer.IgnoreCase; }
        }

        /// <summary>
        /// Gets a value indicating whether or not to clip strings.
        /// </summary>
        /// <value>
        ///   <c>true</c> if set to clip strings otherwise, <c>false</c>.
        /// </value>
        public bool ClipStrings { get; private set; }

        /// <summary>
        /// Gets the failure points.
        /// </summary>
        /// <value>
        /// The failure points.
        /// </value>
        public IList<NUnitEqualityComparer.FailurePoint> FailurePoints
        {
            get { return _comparer.FailurePoints; }
        }

        #endregion

        #region Constraint Modifiers
        /// <summary>
        /// Flag the constraint to ignore case and return self.
        /// </summary>
        public EqualConstraint IgnoreCase
        {
            get
            {
                _comparer.IgnoreCase = true;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to suppress string clipping 
        /// and return self.
        /// </summary>
        public EqualConstraint NoClip
        {
            get
            {
                ClipStrings = false;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to compare arrays as collections
        /// and return self.
        /// </summary>
        public EqualConstraint AsCollection
        {
            get
            {
                _comparer.CompareAsCollection = true;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to use a tolerance when determining equality.
        /// </summary>
        /// <param name="amount">Tolerance value to be used</param>
        /// <returns>Self.</returns>
        public EqualConstraint Within(object amount)
        {
            if (!_tolerance.IsUnsetOrDefault)
                throw new InvalidOperationException("Within modifier may appear only once in a constraint expression");

            _tolerance = new Tolerance(amount);
            return this;
        }

#if !NETCF
        /// <summary>
        /// Flags the constraint to include <see cref="DateTimeOffset.Offset"/>
        /// property in comparison of two <see cref="DateTimeOffset"/> values.
        /// </summary>
        /// <remarks>
        /// Using this modifier does not allow to use the <see cref="Within"/>
        /// constraint modifier.
        /// </remarks>
        public EqualConstraint WithSameOffset
        {
            get 
            { 
                _comparer.WithSameOffset = true;
                return this;
            }
        }
#endif

        /// <summary>
        /// Switches the .Within() modifier to interpret its tolerance as
        /// a distance in representable _values (see remarks).
        /// </summary>
        /// <returns>Self.</returns>
        /// <remarks>
        /// Ulp stands for "unit in the last place" and describes the minimum
        /// amount a given value can change. For any integers, an ulp is 1 whole
        /// digit. For floating point _values, the accuracy of which is better
        /// for smaller numbers and worse for larger numbers, an ulp depends
        /// on the size of the number. Using ulps for comparison of floating
        /// point results instead of fixed tolerances is safer because it will
        /// automatically compensate for the added inaccuracy of larger numbers.
        /// </remarks>
        public EqualConstraint Ulps
        {
            get
            {
                _tolerance = _tolerance.Ulps;
                return this;
            }
        }

        /// <summary>
        /// Switches the .Within() modifier to interpret its tolerance as
        /// a percentage that the actual _values is allowed to deviate from
        /// the expected value.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint Percent
        {
            get
            {
                _tolerance = _tolerance.Percent;
                return this;
            }
        }

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in days.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint Days
        {
            get
            {
                _tolerance = _tolerance.Days;
                return this;
            }
        }

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in hours.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint Hours
        {
            get
            {
                _tolerance = _tolerance.Hours;
                return this;
            }
        }

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in minutes.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint Minutes
        {
            get
            {
                _tolerance = _tolerance.Minutes;
                return this;
            }
        }

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in seconds.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint Seconds
        {
            get
            {
                _tolerance = _tolerance.Seconds;
                return this;
            }
        }

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in milliseconds.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint Milliseconds
        {
            get
            {
                _tolerance = _tolerance.Milliseconds;
                return this;
            }
        }

        /// <summary>
        /// Causes the tolerance to be interpreted as a TimeSpan in clock ticks.
        /// </summary>
        /// <returns>Self</returns>
        public EqualConstraint Ticks
        {
            get
            {
                _tolerance = _tolerance.Ticks;
                return this;
            }
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint Using(IComparer comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint Using<T>(IComparer<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied Comparison object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint Using<T>(Comparison<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint Using(IEqualityComparer comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        /// <summary>
        /// Flag the constraint to use the supplied IEqualityComparer object.
        /// </summary>
        /// <param name="comparer">The IComparer object to use.</param>
        /// <returns>Self.</returns>
        public EqualConstraint Using<T>(IEqualityComparer<T> comparer)
        {
            _comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
            return this;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Test whether the constraint is satisfied by a given value
        /// </summary>
        /// <param name="actual">The value to be tested</param>
        /// <returns>True for success, false for failure</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            AdjustArgumentIfNeeded(ref actual);
            return new EqualConstraintResult(this, actual, _comparer.AreEqual(_expected, actual, ref _tolerance));
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get 
            { 
                System.Text.StringBuilder sb = new System.Text.StringBuilder(MsgUtils.FormatValue(_expected));

                if (_tolerance != null && !_tolerance.IsUnsetOrDefault)
                {
                    sb.Append(" +/- ");
                    sb.Append(MsgUtils.FormatValue(_tolerance.Value));
                    if (_tolerance.Mode != ToleranceMode.Linear)
                    {
                        sb.Append(" ");
                        sb.Append(_tolerance.Mode.ToString());
                    }
                }

                if (_comparer.IgnoreCase)
                    sb.Append(", ignoring case");

                return sb.ToString();
            }
        }

        #endregion

        #region Helper Methods

        // Currently, we only adjust for ArraySegments that have a
        // null array reference. Others could be added in the future.
        private void AdjustArgumentIfNeeded<T>(ref T arg)
        {
#if !NETCF && !SILVERLIGHT && !PORTABLE
            if (arg != null)
            {
                Type argType = arg.GetType();
                Type genericTypeDefinition = argType.GetTypeInfo().IsGenericType ? argType.GetGenericTypeDefinition() : null;

                if (genericTypeDefinition == typeof(ArraySegment<>) && argType.GetProperty("Array").GetValue(arg, null) == null)
                {
                    var elementType = argType.GetGenericArguments()[0];
                    var array = Array.CreateInstance(elementType, 0);
                    var ctor = argType.GetConstructor(new Type[] { array.GetType() });
                    arg = (T)ctor.Invoke(new object[] { array });
                }
            }
#endif
        }

        #endregion
    }
}
