// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestUtilities.Comparers
{
    /// <summary>
    /// ObjectComparer is used in testing to ensure that only
    /// methods of the IComparer interface are used.
    /// </summary>
    public class ObjectComparer : IComparer
    {
        public bool WasCalled = false;
        public static readonly IComparer Default = new ObjectComparer();

        int IComparer.Compare(object x, object y)
        {
            WasCalled = true;
            return Comparer.Default.Compare(x, y);
        }
    }
}
