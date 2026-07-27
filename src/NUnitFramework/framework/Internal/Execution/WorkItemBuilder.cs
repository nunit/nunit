// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Abstractions;
using NUnit.Framework.Internal.Extensions;

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
        public static WorkItem? CreateWorkItem(ITest test, ITestFilter filter, bool recursive = false)
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
        internal static WorkItem? CreateWorkItem(ITest test, ITestFilter filter, IDebugger debugger, bool recursive = false, bool root = true)
        {
            // Run filter on leaf nodes only
            // use the presence of leaf nodes as an indicator that parent need to be created
            // Always create a workitem for the root node
            if (test is not TestSuite suite)
            {
                if (root || filter.Pass(test))
                {
                    return new SimpleWorkItem((TestMethod)test, filter, debugger);
                }
                return null;
            }

            CompositeWorkItem? work = root ? new CompositeWorkItem(suite, filter) : null;

            if (recursive)
            {
                PrepareFixtureDependencies(suite);

                int countOrderedItems = 0;

                foreach (var childTest in suite.Tests)
                {
                    var childItem = CreateWorkItem(childTest, filter, debugger, recursive, root: false);
                    if (childItem is null)
                        continue;

                    work ??= new CompositeWorkItem(suite, filter);

                    if (childItem.TargetApartment == ApartmentState.Unknown && work.TargetApartment != ApartmentState.Unknown)
                        childItem.TargetApartment = work.TargetApartment;

                    if (childTest.Properties.ContainsKey(PropertyNames.Order))
                    {
                        work.Children.Insert(0, childItem);
                        countOrderedItems++;
                    }
                    else
                    {
                        work.Children.Add(childItem);
                    }
                }

                if (countOrderedItems > 1)
                    work!.Children.Sort(0, countOrderedItems, new WorkItemOrderComparer());

                if (work is not null)
                    ReorderChildrenByDependency(work.Children);
            }
            return work;
        }

        private static void PrepareFixtureDependencies(TestSuite suite)
        {
            var fixturesByType = new Dictionary<Type, Test>();
            var dependencyByFixture = new Dictionary<Test, Type>();

            foreach (var child in suite.Tests)
            {
                if (child is not Test fixture || fixture.TypeInfo?.Type is null)
                    continue;

                fixturesByType[fixture.TypeInfo.Type] = fixture;

                if (fixture.Properties.Get(PropertyNames.DependsOn) is Type dependencyType)
                    dependencyByFixture[fixture] = dependencyType;
            }

            MarkCircularDependenciesAsInvalid(fixturesByType, dependencyByFixture);
            MarkDependencyConflictsAsInvalid(fixturesByType, dependencyByFixture);
        }

        private static void MarkCircularDependenciesAsInvalid(Dictionary<Type, Test> fixturesByType, Dictionary<Test, Type> dependencyByFixture)
        {
            var visited = new HashSet<Test>();
            var visiting = new HashSet<Test>();
            var stack = new List<Test>();

            foreach (var fixture in dependencyByFixture.Keys)
                Visit(fixture);

            void Visit(Test fixture)
            {
                if (visited.Contains(fixture))
                    return;

                visited.Add(fixture);
                visiting.Add(fixture);
                stack.Add(fixture);

                if (dependencyByFixture.TryGetValue(fixture, out Type? dependencyType)
                    && fixturesByType.TryGetValue(dependencyType, out Test? dependencyFixture))
                {
                    if (visiting.Contains(dependencyFixture))
                    {
                        int cycleStartIndex = stack.IndexOf(dependencyFixture);
                        if (cycleStartIndex >= 0)
                            MarkCycle(stack, cycleStartIndex);
                    }
                    else
                    {
                        Visit(dependencyFixture);
                    }
                }

                stack.RemoveAt(stack.Count - 1);
                visiting.Remove(fixture);
            }

            static void MarkCycle(List<Test> dependencyPath, int cycleStartIndex)
            {
                const string reason = "Circular DependsOn dependency detected.";

                for (int i = cycleStartIndex; i < dependencyPath.Count; i++)
                    dependencyPath[i].MakeInvalid(reason);
            }
        }

        private static void MarkDependencyConflictsAsInvalid(Dictionary<Type, Test> fixturesByType, Dictionary<Test, Type> dependencyByFixture)
        {
            const string orderReason = "Fixture may not participate in both DependsOn and Order chains.";
            const string parallelReason = "Fixture dependency chains may not include fixtures configured for parallel execution.";

            var dependencyParticipants = new HashSet<Test>(dependencyByFixture.Keys);

            foreach (var dependency in dependencyByFixture.Values)
            {
                if (fixturesByType.TryGetValue(dependency, out Test? dependencyFixture))
                    dependencyParticipants.Add(dependencyFixture);
            }

            foreach (var fixture in dependencyParticipants)
            {
                if (fixture.Properties.ContainsKey(PropertyNames.Order))
                    fixture.MakeInvalid(orderReason);
                if (IsConfiguredParallelWithOtherFixtures(fixture))
                    fixture.MakeInvalid(parallelReason);
            }

            static bool IsConfiguredParallelWithOtherFixtures(Test fixture)
            {
                var scope = fixture.Properties.TryGet(PropertyNames.ParallelScope, ParallelScope.Default);
                return scope.HasFlag(ParallelScope.Self) && !scope.HasFlag(ParallelScope.None);
            }
        }

        private static void ReorderChildrenByDependency(List<WorkItem> children)
        {
            if (children.Count < 2)
                return;

            bool moved;
            int remainingPasses = children.Count * children.Count;

            do
            {
                moved = false;

                var indexByType = new Dictionary<Type, int>();
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i].Test is Test fixture && fixture.TypeInfo?.Type is Type fixtureType)
                        indexByType[fixtureType] = i;
                }

                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i].Test is not Test fixture || fixture.RunState == RunState.NotRunnable || fixture.Properties.Get(PropertyNames.DependsOn) is not Type dependencyType)
                        continue;

                    if (!indexByType.TryGetValue(dependencyType, out int dependencyIndex) || dependencyIndex < i)
                        continue;

                    if (children[dependencyIndex].Test is Test dependencyFixture && dependencyFixture.RunState == RunState.NotRunnable)
                        continue;

                    if (dependencyIndex == i)
                        continue;

                    var dependentChild = children[i];
                    children.RemoveAt(i);

                    if (dependencyIndex > i)
                        dependencyIndex--;

                    children.Insert(dependencyIndex + 1, dependentChild);
                    moved = true;
                    break;
                }
            }
            while (moved && remainingPasses-- > 0);
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
            public int Compare(WorkItem? x, WorkItem? y)
            {
                if (x is null && y is null)
                    return 0;
                if (x is null)
                    return -1;
                if (y is null)
                    return 1;

                var xKey = x.Test.Properties.TryGet(PropertyNames.Order, int.MaxValue);
                var yKey = y.Test.Properties.TryGet(PropertyNames.Order, int.MaxValue);

                return xKey.CompareTo(yKey);
            }
        }
    }
}
