using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Helper to work with <see cref="IConvertible">convertible</see> types.
    /// </summary>
    public class ConvertibleTypes : IEnumerable
    {
        private bool _dirty = true;
        private readonly List<Conversion> _conversions;

        #region Construct
        /// <summary>
        /// Construct instance.
        /// </summary>
        public ConvertibleTypes()
        {
            _conversions = new List<Conversion>();
        }
        #endregion

        #region IEnumerable
        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _conversions.GetEnumerator();
        }
        #endregion

        #region Collection initializers
        /// <summary>Add permitted <paramref name="conversion"/>.</summary>
        /// <param name="conversion">Conversion to be added.</param>
        public void Add(Conversion conversion)
        {
            if (_conversions.Contains(conversion)) return;

            _dirty = true;
            _conversions.Add(conversion);
        }

        /// <summary>
        /// Add permitted conversion from <paramref name="sourceType"/>
        /// to <paramref name="targetType"/>.
        /// </summary>
        /// <param name="sourceType">Type to convert from.</param>
        /// <param name="targetType">Target conversion type.</param>
        public void Add(Type sourceType, Type targetType)
        {
            var conversion = new Conversion(sourceType, targetType);
            Add(conversion);
        }

        /// <summary>
        /// Add permitted conversions from <paramref name="sourceType"/>
        /// to any type in <paramref name="targetTypes"/> list.
        /// </summary>
        /// <param name="sourceType">Type to convert from.</param>
        /// <param name="targetTypes">List of permitted target types.</param>
        public void Add(Type sourceType, params Type[] targetTypes)
        {
            for (var index = 0; index < targetTypes.Length; index++)
            {
                Add(sourceType, targetTypes[index]);
            }
        }

        /// <summary>
        /// Add permitted conversions from any type in <paramref name="sourceTypes"/>
        /// list to <paramref name="targetType"/>.
        /// </summary>
        /// <param name="sourceTypes">List of permitted types to convert from.</param>
        /// <param name="targetType">Target conversion type.</param>
        public void Add(Type[] sourceTypes, Type targetType)
        {
            for (var index = 0; index < sourceTypes.Length; index++)
            {
                Add(sourceTypes[index], targetType);
            }
        }

        /// <summary>
        /// Add permitted conversions from any type in <paramref name="sourceTypes"/>
        /// to any type in <paramref name="targetTypes"/> list.
        /// </summary>
        /// <param name="sourceTypes">List of permitted types to convert from.</param>
        /// <param name="targetTypes">List of permitted target types.</param>
        public void Add(Type[] sourceTypes, Type[] targetTypes)
        {
            foreach (var source in sourceTypes)
            {
                foreach (var target in targetTypes)
                {
                    if (source == target) continue;

                    Add(source, target);
                }
            }
        }
        #endregion

        /// <summary>
        /// Rebuild list of types for faster lookup.
        /// </summary>
        private void Rebuild()
        {
            _conversions.Sort();
            _dirty = false;
        }

        /// <summary>
        /// Validate all possible conversions.
        /// </summary>
        public void Validate()
        {
            for (var index = _conversions.Count - 1; index >= 0; index--)
            {
                try
                {
                    var conversion = _conversions[index];
                    var value = Activator.CreateInstance(conversion.SourceType);
                    Convert.ChangeType(value, conversion.TargetType);
                }
                catch
                {
                    _conversions.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Check that there is a conversion permitted between
        /// <paramref name="sourceType"/> and <paramref name="targetType"/>.
        /// </summary>
        /// <param name="sourceType">Type to convert from.</param>
        /// <param name="targetType">Target type for conversion.</param>
        public bool this[Type sourceType, Type targetType]
        {
            get
            {
                if (_dirty)
                {
                    Rebuild();
                }

                var conversion = new Conversion(sourceType, targetType);
                return _conversions.BinarySearch(conversion) >= 0;
            }
        }

        #region Predefined collections
        /// <summary>
        /// List of <see cref="IConvertible"/> types.
        /// </summary>
        public static readonly Type[] List = new Type[]
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(decimal),
            typeof(float),
            typeof(double),
            typeof(char),
            typeof(bool),
            typeof(DateTime),
            typeof(Enum),
            typeof(string)
        };

        private static readonly Lazy<ConvertibleTypes> _Numeric = new Lazy<ConvertibleTypes>(
            delegate ()
            {
                return new ConvertibleTypes()
                {
                    { typeof(decimal), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char) },
                    { typeof(double), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char), typeof(float) },
                    { typeof(float), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char), typeof(float) },
                    { typeof(ulong), typeof(byte), typeof(ushort), typeof(uint), typeof(char) },
                    { typeof(long), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(char) },
                    { typeof(uint), typeof(byte), typeof(ushort), typeof(char) },
                    { typeof(int), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(char) },
                    { typeof(ushort), typeof(byte), typeof(char) },
                    { typeof(short), typeof(byte) }
                };
            },
            true
        );

        /// <summary>
        /// Conversions defined by §6.1.2 (Implicit numeric conversions) of
        /// the specification.
        /// </summary>
        public static ConvertibleTypes Numeric
        {
            get { return _Numeric.Value; }
        }

        private static readonly Lazy<ConvertibleTypes> _All = new Lazy<ConvertibleTypes>(
            delegate ()
            {
                var result = new ConvertibleTypes()
                {
                    { List, List }
                };

                result.Validate();

                return result;
            },
            true
        );

        /// <summary>
        /// All possible conversions.
        /// </summary>
        public static ConvertibleTypes All
        {
            get { return _All.Value; }
        }

        private static readonly Lazy<ConvertibleTypes> _Internal = new Lazy<ConvertibleTypes>(
            delegate ()
            {
                return new ConvertibleTypes()
                {
                    { typeof(int), typeof(long), typeof(ulong), typeof(short), typeof(byte), typeof(sbyte), typeof(double), typeof(float), typeof(decimal) },
                    { new Type[] { typeof(short), typeof(byte), typeof(sbyte) }, typeof(int) },
                    { typeof(double), typeof(decimal) },
                    { typeof(float), typeof(decimal) },
                    { typeof(string), typeof(decimal) },
                    { typeof(string), typeof(DateTime) }
                };
            },
            true
        );

        /// <summary>
        /// <p>
        /// TODO:This is a slightly modified and refined version of possible
        /// conversions thas was merged from a lot of "if" statements spread
        /// across the codebase.
        /// </p>
        /// 
        /// <p>
        /// For instance there was implementation in TypeHelper. Weird, but
        /// having short/byte/sbyte to int conversion (that may cause
        /// <see cref="OverflowException"/>) it hasn't reverse conversion from
        /// short/byte/sbyte -> int that is absolutely safe. Types like
        /// long/ulong/uint were completely ignored.
        /// </p>
        /// 
        /// <p>
        /// Another implementation in TestCaseAttribute also had funny things.
        /// It was able to convert int -> double?, but can't to double itself.
        /// As well as conversion to other real numbers like float and decimal.
        /// </p>
        /// 
        /// <p>
        /// Implementation in ValuesAttribute was even more limited and had
        /// wrong handling of TimeSpan that isn't <see cref="IConvertible"/>
        /// at all.
        /// <see cref="IConvertible"/>.
        /// </p>
        /// 
        /// <p>
        /// There are also a lot of non-numeric conversions (involving char,
        /// bool, enum, string types) those might be also useful. And, of course,
        /// ToString() conversion. So it will be good to reconsider and unify
        /// the strategy for permitted type conversions and probably reuse some
        /// defined in this class like <see cref="Numeric"/> or <see cref="All"/>.
        /// </p>
        /// </summary>
        public static ConvertibleTypes Internal
        {
            get { return _Internal.Value; }
        }
        #endregion

        #region Nested types
        /// <summary>
        /// Describes conversion from the <see cref="SourceType"/> to the
        /// <see cref="TargetType"/>.
        /// </summary>
        public struct Conversion : IComparable<Conversion>
        {
            /// <summary>
            /// Source type we can convert from.
            /// </summary>
            public Type SourceType { get; }

            /// <summary>
            /// Target type for the conversion.
            /// </summary>
            public Type TargetType { get; }

            /// <summary>
            /// Construct instance using <paramref name="sourceType"/>
            /// and <paramref name="targetType"/>.
            /// </summary>
            /// <param name="sourceType">Source type we can convert from.</param>
            /// <param name="targetType">Target type for the conversion.</param>
            public Conversion(Type sourceType, Type targetType)
            {
                SourceType = sourceType;
                TargetType = targetType;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return string.Format("{0} -> {1}", SourceType, TargetType);
            }

            #region IComparable<Conversion>
            /// <inheritdoc />
            int IComparable<Conversion>.CompareTo(Conversion other)
            {
                var result = SourceType.GetHashCode().CompareTo(other.SourceType.GetHashCode());

                return result == 0
                        ? TargetType.GetHashCode().CompareTo(other.TargetType.GetHashCode())
                        : result
                    ;
            }
            #endregion
        }
        #endregion
    }
}
