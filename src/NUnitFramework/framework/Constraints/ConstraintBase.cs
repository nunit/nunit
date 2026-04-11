// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Text;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// The ConstraintBase class is the base of all built-in constraints
    /// within NUnit. It provides the operator overloads used to combine
    /// constraints.
    /// </summary>
    public abstract class ConstraintBase
    {
        private readonly Lazy<string> _displayName;

        #region Constructor

        /// <summary>
        /// Construct a constraint with optional arguments
        /// </summary>
        /// <param name="args">Arguments to be saved</param>
        protected ConstraintBase(params object?[] args)
        {
            Arguments = args;

            _displayName = new Lazy<string>(() =>
            {
                var type = GetType();
                var displayName = type.Name;
                if (type.IsGenericType)
                    displayName = displayName.Substring(0, displayName.Length - 2);
                if (displayName.EndsWith("Constraint", StringComparison.Ordinal))
                    displayName = displayName.Substring(0, displayName.Length - 10);
                return displayName;
            });
        }

        #endregion

        #region Properties

        /// <summary>
        /// The display name of this Constraint for use by ToString().
        /// The default value is the name of the constraint with
        /// trailing "Constraint" removed. Derived classes may set
        /// this to another name in their constructors.
        /// </summary>
        public virtual string DisplayName => _displayName.Value;

        /// <inheritdoc/>
        public abstract string Description { get; }

        /// <summary>
        /// Arguments provided to this Constraint, for use in
        /// formatting the description.
        /// </summary>
        public object?[] Arguments { get; }

        #endregion

        #region ToString Override

        /// <inheritdoc/>
        public override string ToString() => GetStringRepresentation();

        /// <summary>
        /// Returns the string representation of this constraint and the passed in arguments
        /// </summary>
        protected string GetStringRepresentation(IEnumerable arguments)
        {
            var sb = new StringBuilder();

            sb.Append('<');
            sb.Append(DisplayName.ToLower());

            foreach (object? arg in arguments)
            {
                sb.Append(' ');
                sb.Append(Displayable(arg));
            }

            sb.Append('>');

            return sb.ToString();

            static string Displayable(object? o)
            {
                if (o is null)
                    return "null";
                else if (o is string s)
                    return $"\"{s}\"";
                else
                    return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", o);
            }
        }

        /// <summary>
        /// Returns the string representation of this constraint
        /// </summary>
        protected virtual string GetStringRepresentation()
            => GetStringRepresentation(Arguments);

        #endregion
    }
}
