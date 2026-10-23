// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;

namespace NUnit.Framework.Tests.TestUtilities
{
    /// <summary>
    /// Class used for testing classes that inherit from Tuple (and hence ITuple)
    /// </summary>
    /// <remarks>
    /// Equals comparer should call this method's Equals and not use the ITuple comparer.
    /// Format should call this method's ToString and not the FormatTuple.
    /// </remarks>
    internal sealed class MyTuple : Tuple<int>, IStructuralEquatable
    {
        public MyTuple(string name, int item1)
            : base(item1)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"{Name}: {base.ToString()}";
        }

        // NUnit calls IStructuralEquatable.Equals before Object.Equals
        // We need to therefore implement IStructuralEquatable here
        // as otherwise the Tuple<>'s implementation will be used, which will ignore the Name property.
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            if (other is MyTuple otherTuple)
            {
                return comparer.Equals(Name, otherTuple.Name) &&
                       comparer.Equals(Item1, otherTuple.Item1);
            }

            return false;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return HashCode.Combine(
                comparer.GetHashCode(Name),
                comparer.GetHashCode(Item1));
        }

        public override bool Equals(object? obj)
        {
            if (obj is MyTuple other)
            {
                return Name == other.Name && base.Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, base.GetHashCode());
        }
    }
}
