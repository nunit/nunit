using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Class to configure how to compare properties
    /// </summary>
    public class PropertiesComparerConfiguration
    {
        /// <summary>
        /// The global fallback configuration for comparing properties.
        /// </summary>
        internal static PropertiesComparerConfiguration Default { get; } = new();

        /// <summary>
        /// Gets and sets the option to compare the same properties in different types.
        /// </summary>
        internal bool AllowComparingDifferentTypes { get; set; }

        /// <summary>
        /// Gets and sets the option to compare only matching properties, ignoring others.
        /// </summary>
        internal bool OnlyCompareCommonProperties { get; set; }

        /// <summary>
        /// Gets and sets the names of properties to exclude from comparison.
        /// </summary>
        internal HashSet<string>? PropertyNamesToExclude { get; set; }

        /// <summary>
        /// Gets and sets the names of properties to exclude from comparison.
        /// </summary>
        internal Dictionary<Type, HashSet<string>>? PropertyNamesToExcludeForType { get; set; }

        /// <summary>
        /// Gets and sets the names of properties to compare.
        /// </summary>
        internal HashSet<string>? PropertyNamesToUse { get; set; }

        /// <summary>
        /// Gets and sets the names of properties to compare.
        /// </summary>
        internal Dictionary<Type, HashSet<string>>? PropertyNamesToUseForType { get; set; }

        /// <summary>
        /// Gets and sets the mapping of property names in case of different types.
        /// </summary>
        internal Dictionary<string, string>? PropertyNameMap { get; set; }

        /// <summary>
        /// Gets and sets the mapping of property names in case of different types.
        /// </summary>
        internal Dictionary<Type, Dictionary<string, string>>? PropertyNameMapForType { get; set; }

        /// <summary>
        /// Gets and sets the mapping of property name to values.
        /// </summary>
        internal Dictionary<Type, Dictionary<string, object?>>? PropertyNameToValueMapForType { get; set; }

        /// <summary>
        /// Gets and sets the tolerance to apply for time values.
        /// </summary>
        internal Tolerance TimeSpanTolerance { get; set; } = Tolerance.Default;

        /// <summary>
        /// Gets and sets the tolerance to apply for numeric values.
        /// </summary>
        internal Tolerance FloatingPointTolerance { get; set; } = Tolerance.Default;

        /// <summary>
        /// Gets and sets the tolerance to apply for numeric values.
        /// </summary>
        internal Tolerance FixedPointTolerance { get; set; } = Tolerance.Default;

        /// <summary>
        /// Set the tolerance to apply based upon the type of the tolerance.
        /// </summary>
        /// <remarks>
        /// This method accepts a <see cref="TimeSpan"/>, a numeric value or a <see cref="Tolerance"/> instance.
        /// </remarks>
        /// <param name="amount"></param>
        protected void SetTolerance(object amount)
        {
            if (amount is not Tolerance instance)
                instance = new Tolerance(amount);

            if (instance.Amount is TimeSpan)
            {
                TimeSpanTolerance = instance;
            }
            else if (Numerics.IsFloatingPointNumeric(instance.Amount) || instance.Mode == ToleranceMode.Ulps)
            {
                FloatingPointTolerance = instance;
            }
            else
            {
                FixedPointTolerance = instance;
            }
        }
    }

    // new HashSet<string>(properties) is clearer to me then [.. properties]
    // I also have seen cases where the latter compiles to inferior code.
#pragma warning disable IDE0306 // Simplify collection initialization

    // PropertyNameMapping = new Dictionary<string, string>() clearly shows type.
    // PropertyNameMapping = [] does not. It looks more like an array than a dictionary.
#pragma warning disable IDE0028 // Simplify collection initialization

    /// <summary>
    /// Class to configure how to compare properties. Non-generic untyped version.
    /// </summary>
    public class PropertiesComparerConfigurationUntyped : PropertiesComparerConfiguration
    {
        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.AllowComparingDifferentTypes"/> property.
        /// </summary>
        /// <returns>Self.</returns>
        public PropertiesComparerConfigurationUntyped AllowDifferentTypes()
        {
            AllowComparingDifferentTypes = true;
            return this;
        }

        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.OnlyCompareCommonProperties"/> property.
        /// </summary>
        /// <remarks>Implies <see cref="AllowDifferentTypes" /></remarks>
        /// <returns>Self.</returns>
        public PropertiesComparerConfigurationUntyped CompareOnlyCommonProperties()
        {
            OnlyCompareCommonProperties = true;
            return AllowDifferentTypes();
        }

        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.PropertyNamesToUse"/> property.
        /// </summary>
        /// <remarks>
        /// The names are not related to one type, but common across all nested compared types.
        /// This could be used to only compare `id` of types instead of each property.
        /// </remarks>
        /// <param name="properties">List of properties to use for comparison.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfigurationUntyped Using(params string[] properties)
        {
            PropertyNamesToUse = new HashSet<string>(properties);
            return this;
        }

        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.PropertyNamesToExclude"/> property.
        /// </summary>
        /// <param name="properties">List of properties to use for comparison.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfigurationUntyped Excluding(params string[] properties)
        {
            PropertyNamesToExclude = new HashSet<string>(properties);
            return this;
        }

        /// <summary>
        /// Updates the <see cref="PropertiesComparerConfiguration.PropertyNameMap"/> property.
        /// </summary>
        /// <remarks>Implies <see cref="AllowDifferentTypes" /></remarks>
        /// <param name="from">The name to map from.</param>
        /// <param name="to">The name to map to.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfigurationUntyped Map(string from, string to)
        {
            PropertyNameMap ??= new Dictionary<string, string>();
            PropertyNameMap.Add(from, to);
            return AllowDifferentTypes();
        }

        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.PropertyNameMap"/> property.
        /// </summary>
        /// <remarks>Implies <see cref="AllowDifferentTypes" /></remarks>
        /// <param name="properties">List of properties to map for comparison.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfigurationUntyped Map(params (string From, string To)[] properties)
        {
            PropertyNameMap = properties.ToDictionary(x => x.From, x => x.To);
            return AllowDifferentTypes();
        }

        /// <summary>
        /// Specify a tolerance for all numeric comparisons.
        /// </summary>
        /// <param name="amount">The tolerance to apply.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfigurationUntyped Within(object amount)
        {
            SetTolerance(amount);
            return this;
        }
    }

    /// <summary>
    /// Generic version of <see cref="PropertiesComparerConfiguration"/> to allow
    /// specifiying properties using expression syntax instead of strings.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertiesComparerConfiguration<T> : PropertiesComparerConfiguration
    {
        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.AllowComparingDifferentTypes"/> property.
        /// </summary>
        /// <returns>Self.</returns>
        public PropertiesComparerConfiguration<T> AllowDifferentTypes()
        {
            AllowComparingDifferentTypes = true;
            return this;
        }

        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.OnlyCompareCommonProperties"/> property.
        /// </summary>
        /// <remarks>Implies <see cref="AllowDifferentTypes" /></remarks>
        /// <returns>Self.</returns>
        public PropertiesComparerConfiguration<T> CompareOnlyCommonProperties()
        {
            OnlyCompareCommonProperties = true;
            return AllowDifferentTypes();
        }

        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.PropertyNamesToUseForType"/> property.
        /// </summary>
        /// <param name="properties">List of properties to use for comparison.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfiguration<T> Using(params Expression<Func<T, object?>>[] properties)
        {
            return Using<T>(properties);
        }

        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.PropertyNamesToUseForType"/> property.
        /// </summary>
        /// <param name="properties">List of properties to use for comparison.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfiguration<T> Using<TFrom>(params Expression<Func<TFrom, object?>>[] properties)
        {
            PropertyNamesToUseForType ??= new Dictionary<Type, HashSet<string>>();
            PropertyNamesToUseForType[typeof(TFrom)] = new HashSet<string>(properties.Select(GetNameFromExpression));
            return AllowDifferentTypes();
        }

        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.PropertyNamesToExclude"/> property.
        /// </summary>
        /// <param name="properties">List of properties to use for comparison.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfiguration<T> Excluding(params Expression<Func<T, object?>>[] properties)
        {
            return Excluding<T>(properties);
        }

        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.PropertyNamesToExclude"/> property.
        /// </summary>
        /// <param name="properties">List of properties to use for comparison.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfiguration<T> Excluding<TFrom>(params Expression<Func<TFrom, object?>>[] properties)
        {
            PropertyNamesToExcludeForType ??= new Dictionary<Type, HashSet<string>>();
            PropertyNamesToExcludeForType[typeof(TFrom)] = new HashSet<string>(properties.Select(GetNameFromExpression));
            return AllowDifferentTypes();
        }

        /// <summary>
        /// Updates the <see cref="PropertiesComparerConfiguration.PropertyNameMapForType"/> property.
        /// </summary>
        /// <remarks>Implies <see cref="AllowDifferentTypes" /></remarks>
        /// <param name="from">The name to map from.</param>
        /// <param name="to">The name to map to.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfiguration<T> Map<TTo>(Expression<Func<T, object?>> from, Expression<Func<TTo, object?>> to)
        {
            return Map<T, TTo>(from, to);
        }

        /// <summary>
        /// Updates the <see cref="PropertiesComparerConfiguration.PropertyNameMapForType"/> property.
        /// </summary>
        /// <remarks>Implies <see cref="AllowDifferentTypes" /></remarks>
        /// <param name="from">The name to map from.</param>
        /// <param name="to">The name to map to.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfiguration<T> Map<TFrom, TTo>(Expression<Func<TFrom, object?>> from, Expression<Func<TTo, object?>> to)
        {
            PropertyNameMapForType ??= new Dictionary<Type, Dictionary<string, string>>();
            if (!PropertyNameMapForType.TryGetValue(typeof(TFrom), out Dictionary<string, string>? nameMapping))
            {
                nameMapping = new Dictionary<string, string>();
                PropertyNameMapForType.Add(typeof(TFrom), nameMapping);
            }
            nameMapping.Add(GetNameFromExpression(from), GetNameFromExpression(to));
            return AllowDifferentTypes();
        }

        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.PropertyNameMapForType"/> property.
        /// </summary>
        /// <remarks>Implies <see cref="AllowDifferentTypes" /></remarks>
        /// <param name="properties">List of properties to map for comparison.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfiguration<T> Map<TTo>(params (Expression<Func<T, object?>> From, Expression<Func<TTo, object?>> To)[] properties)
        {
            return Map<T, TTo>(properties);
        }

        /// <summary>
        /// Set the <see cref="PropertiesComparerConfiguration.PropertyNameMapForType"/> property.
        /// </summary>
        /// <remarks>Implies <see cref="AllowDifferentTypes" /></remarks>
        /// <param name="properties">List of properties to map for comparison.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfiguration<T> Map<TFrom, TTo>(params (Expression<Func<TFrom, object?>> From, Expression<Func<TTo, object?>> To)[] properties)
        {
            PropertyNameMapForType ??= new Dictionary<Type, Dictionary<string, string>>();
            PropertyNameMapForType[typeof(TFrom)] = properties.ToDictionary(
                x => GetNameFromExpression(x.From), x => GetNameFromExpression(x.To));
            return AllowDifferentTypes();
        }

        /// <summary>
        /// Updates the <see cref="PropertiesComparerConfiguration.PropertyNameToValueMapForType"/> property.
        /// </summary>
        /// <remarks>Implies <see cref="AllowDifferentTypes" /></remarks>
        /// <param name="from">The name to map from.</param>
        /// <param name="value">The value to use for a propery with name <paramref name="from"/>.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfiguration<T> Map<TFrom>(Expression<Func<TFrom, object?>> from, object? value)
        {
            PropertyNameToValueMapForType ??= new Dictionary<Type, Dictionary<string, object?>>();
            if (!PropertyNameToValueMapForType.TryGetValue(typeof(TFrom), out Dictionary<string, object?>? nameToValueMapping))
            {
                nameToValueMapping = new Dictionary<string, object?>();
                PropertyNameToValueMapForType.Add(typeof(TFrom), nameToValueMapping);
            }
            nameToValueMapping.Add(GetNameFromExpression(from), value);
            return AllowDifferentTypes();
        }

        /// <summary>
        /// Specify a tolerance for all numeric comparisons.
        /// </summary>
        /// <param name="amount">The tolerance to apply.</param>
        /// <returns>Self.</returns>
        public PropertiesComparerConfiguration<T> Within(object amount)
        {
            SetTolerance(amount);
            return this;
        }

        private static string GetNameFromExpression<TExpression>(Expression<Func<TExpression, object?>> expression)
        {
            Expression body = expression.Body;

            // We only expect a single member access, but it might include in implicit cast to object.
            if (body is UnaryExpression unaryExpresion &&
                unaryExpresion.NodeType == ExpressionType.Convert &&
                unaryExpresion.Type == typeof(object))
            {
                body = unaryExpresion.Operand;
            }

            if (body is MemberExpression memberExpression)
                return memberExpression.Member.Name;

            throw new ArgumentException("Expression must be a member expression", nameof(expression));
        }
    }

#pragma warning restore IDE0028 // Simplify collection initialization
#pragma warning restore IDE0306 // Simplify collection initialization
}
