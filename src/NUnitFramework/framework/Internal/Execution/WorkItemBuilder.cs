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
                var testDependencies = PrepareTestDependencies(suite);
                var sortedDependencies = TopologicalSort(testDependencies);

                int countOrderedItems = 0;

                foreach (var childTest in sortedDependencies)
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
            }
            return work;
        }

        private static List<ITest> TopologicalSort(Dictionary<ITest, ITest?> dependencyGraph)
        {
            var sorted = new List<ITest>();
            var visited = new HashSet<ITest>();
            foreach (var fixture in dependencyGraph.Keys)
                Visit(fixture);
            void Visit(ITest fixture)
            {
                if (!visited.Add(fixture))
                    return;
                if (dependencyGraph.TryGetValue(fixture, out ITest? dependency) && dependency is not null)
                    Visit(dependency);
                sorted.Add(fixture);
            }
            return sorted;
        }

        private static Dictionary<ITest, ITest?> PrepareTestDependencies(TestSuite suite)
        {
            var fixturesByType = new Dictionary<Type, Test>();
            var dependencyByFixture = new Dictionary<Test, Type>();

            var dependencyGraph = new Dictionary<ITest, ITest?>();

            foreach (var child in suite.Tests)
            {
                dependencyGraph[child] = null;

                if (child is not Test fixture || fixture.TypeInfo?.Type is null)
                    continue;

                fixturesByType[fixture.TypeInfo.Type] = fixture;

                if (fixture.Properties.Get(PropertyNames.DependsOnType) is Type dependencyType)
                    dependencyByFixture[fixture] = dependencyType;
            }

            foreach (var pair in dependencyByFixture)
            {
                if (fixturesByType.TryGetValue(pair.Value, out Test? dependencyFixture))
                    dependencyGraph[pair.Key] = dependencyFixture;
            }

            MarkInvalidDependenciesAsInvalid(fixturesByType, dependencyByFixture);
            MarkDependencyFeatureConflictsAsInvalid(dependencyGraph);

            foreach (var pair in dependencyByFixture)
            {
                var fixture = pair.Key;

                if (fixture.RunState != RunState.Runnable
                    || !fixturesByType.TryGetValue(pair.Value, out Test? dependencyFixture)
                    || dependencyFixture.RunState != RunState.Runnable)
                {
                    dependencyGraph[fixture] = null;
                }
            }

            return dependencyGraph;
        }

        private static void MarkInvalidDependenciesAsInvalid(Dictionary<Type, Test> fixturesByType, Dictionary<Test, Type> dependencyByFixture)
        {
            const string directReferenceReason = "Circular or self-referential test dependency detected.";
            const string baseReferenceReason = "Circular or self-referential test dependency detected via base class.";

            var invalidCircularDependencies = new HashSet<Test>();
            var visited = new HashSet<Test>();
            var visiting = new List<Test>();

            foreach (var fixture in dependencyByFixture.Keys)
                Visit(fixture);

            void Visit(Test fixture)
            {
                if (!visited.Add(fixture))
                    return;

                visiting.Add(fixture);

                if (dependencyByFixture.TryGetValue(fixture, out Type? dependencyType))
                {
                    if (IsSelfReferentialBaseDependency(fixture, dependencyType))
                    {
                        MarkInvalidCircularDependency(fixture, baseReferenceReason);
                    }
                    else if (fixturesByType.TryGetValue(dependencyType, out Test? dependencyFixture))
                    {
                        if (visiting.Contains(dependencyFixture))
                        {
                            int cycleStartIndex = visiting.IndexOf(dependencyFixture);
                            if (cycleStartIndex >= 0)
                                MarkCycle(visiting, cycleStartIndex);
                        }
                        else
                        {
                            Visit(dependencyFixture);
                        }

                        if (invalidCircularDependencies.Contains(dependencyFixture))
                            MarkInvalidCircularDependency(fixture, directReferenceReason);
                    }
                    else
                    {
                        // Dependency not found in the suite
                        fixture.MakeInvalid($"Test dependency {dependencyType} can not be found. Please verify it was configured correctly and was not filtered out.");
                    }
                }

                visiting.RemoveAt(visiting.Count - 1);
            }

            static bool IsSelfReferentialBaseDependency(Test fixture, Type dependencyType)
            {
                if (fixture.TypeInfo?.Type is not Type fixtureType)
                    return false;

                return fixtureType.IsSubclassOf(dependencyType);
            }

            void MarkInvalidCircularDependency(Test fixture, string reason)
            {
                if (invalidCircularDependencies.Add(fixture))
                    fixture.MakeInvalid(reason);
            }

            void MarkCycle(List<Test> dependencyPath, int cycleStartIndex)
            {
                for (int i = cycleStartIndex; i < dependencyPath.Count; i++)
                    MarkInvalidCircularDependency(dependencyPath[i], directReferenceReason);
            }
        }

        private static void MarkDependencyFeatureConflictsAsInvalid(Dictionary<ITest, ITest?> dependencyGraph)
        {
            const string orderReason = "Fixture may not participate in both DependsOn and Order chains.";
            const string parallelReason = "Fixture dependency chains may not include fixtures configured for parallel execution.";

            foreach (var pair in dependencyGraph)
            {
                if (pair.Key is not Test fixture || pair.Value is not Test dependencyFixture)
                    continue;

                MarkDependencyFeatureConflictsAsInvalid(fixture);
                MarkDependencyFeatureConflictsAsInvalid(dependencyFixture);
            }

            static void MarkDependencyFeatureConflictsAsInvalid(Test? fixture)
            {
                if (fixture is null)
                    return;

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
