// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// NUnitComparer encapsulates NUnit's default behavior
    /// in comparing two objects.
    /// </summary>
    public sealed class NUnitComparer : IComparer
    {
        /// <summary>
        /// Returns the default NUnitComparer.
        /// </summary>
        public static NUnitComparer Default
        {
            get { return new NUnitComparer(); }
        }

        /// <summary>
        /// Compares two objects
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            if (x == null)
                return y == null ? 0 : -1;
            else if (y == null)
                return +1;

            if (Numerics.IsNumericType(x) && Numerics.IsNumericType(y))
                return Numerics.Compare(x, y);

            Type xType = x.GetType();
            Type yType = y.GetType();

            // If we use BindingFlags.ExactBinding it will prevent us finding CompareTo(object)
            // It however also prevents finding CompareTo(TBase) when called with TDerived
            // Nor will it find CompareTo(int) when called with a short.
            // We fallback to explicitly exclude CompareTo(object)
            static bool IsIComparable(MethodInfo method) => method.GetParameters()[0].ParameterType == typeof(object);

            MethodInfo method = xType.GetMethod("CompareTo", new Type[] { yType });
            if (method != null && !IsIComparable(method))
                return (int)method.Invoke(x, new object[] { y });

            method = yType.GetMethod("CompareTo", new Type[] { xType });
            if (method != null && !IsIComparable(method))
                return -(int)method.Invoke(y, new object[] { x });

            if (x is IComparable xComparable)
                return xComparable.CompareTo(y);

            if (y is IComparable yComparable)
                return -yComparable.CompareTo(x);

            throw new ArgumentException("Neither value implements IComparable or IComparable<T>");
        }
    }
}
