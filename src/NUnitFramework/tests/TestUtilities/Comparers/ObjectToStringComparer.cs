// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;

namespace NUnit.TestUtilities.Comparers
{
    /// <summary>
    /// ObjectToStringComparer is used in testing the <see cref="Framework.Constraints.RangeConstraint"/> when the object does not implement the <see cref="IComparer"/>  interface.
    /// Compares them as numbers when both arguments are <see cref="int"/>, else it uses <seealso cref="string.CompareTo(string)"/>.
    /// </summary>
    public class ObjectToStringComparer : IComparer
    {
        public bool WasCalled { get; private set; }

        int IComparer.Compare(object? x, object? y)
        {
            WasCalled = true;

            string? xAsString = x?.ToString();
            string? yAsString = y?.ToString();
            if (xAsString is null)
            {
                if (yAsString is null)
                    return 0;
                else
                    return -1;
            }
            else if (yAsString is null)
            {
                return 1;
            }

            if (int.TryParse(xAsString, out int intX) && int.TryParse(yAsString, out int intY))
            {
                return intX.CompareTo(intY);
            }

            return xAsString.CompareTo(yAsString);
        }
    }

    public class ObjectToStringEqualityComparer : IEqualityComparer
    {
        private readonly IComparer _comparer = new ObjectToStringComparer();

        public bool WasCalled { get; private set; }

        public new bool Equals(object? x, object? y)
        {
            WasCalled = true;
            return _comparer.Compare(x, y) == 0;
        }

        public int GetHashCode(object obj)
        {
            return obj.ToString()!.GetHashCode();
        }
    }

    /// <summary>
    /// NoComparer is a test class without implementing the IComparer interface.
    /// Used to test the RangeConstraint.
    /// </summary>
    public class NoComparer
    {
        private readonly object _value;
        public NoComparer(object value)
        {
            _value = value;
        }
        public override string? ToString()
        {
            return _value.ToString();
        }
    }
}
