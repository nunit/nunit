// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// WorkItemBuilder class knows how to build a tree of work items from a tree of tests
    /// </summary>
    public static class WorkItemBuilder
    {
        #region Static Factory Method

        /// <summary>
        /// Creates a work item.
        /// </summary>
        /// <param name="test">The test for which this WorkItem is being created.</param>
        /// <param name="filter">The filter to be used in selecting any child Tests.</param>
        /// <param name="recursive">True if child work items should be created and added.</param>
        /// <returns></returns>
        static public WorkItem CreateWorkItem(ITest test, ITestFilter filter, bool recursive = false)
        {
            return CreateWorkItem(test, filter, new DebuggerProxy(), recursive);
        }

        /// <summary>
        /// Creates a work item.
        /// </summary>
        /// <param name="test">The test for which this WorkItem is being created.</param>
        /// <param name="filter">The filter to be used in selecting any child Tests.</param>
        /// <param name="debugger">An <see cref="IDebugger" /> instance.</param>
        /// <param name="recursive">True if child work items should be created and added.</param>
        /// <param name="root"><see langword="true"/> if work item needs to be created unconditionally, if <see langword="false"/> <see langword="null"/> will be returned for tests that don't match the filter.</param>
        /// <returns></returns>
        internal static WorkItem CreateWorkItem(ITest test, ITestFilter filter, IDebugger debugger, bool recursive = false, bool root = true)
        {
            // Run filter on leaf nodes only
            // use the presence of leaf nodes as an indicator that parent need to be created
            // Always create a workitem for the root node
            TestSuite suite = test as TestSuite;
            if (suite == null)
            {
                if (root || filter.Pass(test))
                {
                    return new SimpleWorkItem((TestMethod)test, filter, debugger);
                }
                return null;
            }

            CompositeWorkItem work = root ? new CompositeWorkItem(suite, filter): null;

            if (recursive)
            {
                int countOrderedItems = 0;

                foreach (var childTest in suite.Tests)
                {
                    var childItem = CreateWorkItem(childTest, filter, debugger, recursive, root: false);
                    if (childItem == null) continue;

                    work ??= new CompositeWorkItem(suite, filter);

                    if (childItem.TargetApartment == ApartmentState.Unknown && work.TargetApartment != ApartmentState.Unknown)
                        childItem.TargetApartment = work.TargetApartment;

                    if (childTest.Properties.ContainsKey(PropertyNames.Order))
                    {
                        work.Children.Insert(0, childItem);
                        countOrderedItems++;
                    }
                    else
                        work.Children.Add(childItem);
                }

                if (countOrderedItems > 0)
                    work.Children.Sort(0, countOrderedItems, new WorkItemOrderComparer());

            }
            return work;
        }

        #endregion

        private class WorkItemOrderComparer : IComparer<WorkItem>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
            public int Compare(WorkItem x, WorkItem y)
            {
                var xKey = int.MaxValue;
                var yKey = int.MaxValue;

                if (x.Test.Properties.ContainsKey(PropertyNames.Order))
                    xKey = (int)x.Test.Properties[PropertyNames.Order][0];

                if (y.Test.Properties.ContainsKey(PropertyNames.Order))
                    yKey = (int)y.Test.Properties[PropertyNames.Order][0];

                return xKey.CompareTo(yKey);
            }
        }
    }
}
