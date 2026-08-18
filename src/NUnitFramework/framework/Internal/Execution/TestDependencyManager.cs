// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Tests
{
    internal class TestDependencyManager
    {
        public Dictionary<ITest, ITest>? PrepareTestDependencies(TestSuite suite)
        {
            var dependencyGraph = BuildFixtureDependencyGraph(suite);
            if (dependencyGraph is null)
            {
                return null;
            }

            // Validate dependency graph
            MarkInvalidDependenciesAsInvalid(dependencyGraph);
            MarkDependencyFeatureConflictsAsInvalid(dependencyGraph);

            return dependencyGraph;

            static Dictionary<ITest, ITest>? BuildFixtureDependencyGraph(TestSuite suite)
            {
                var fixturesByType = new Dictionary<Type, Test>(suite.Tests.Count);
                var dependencyByFixture = new Dictionary<Test, Type>();

                foreach (var child in suite.Tests)
                {
                    if (child is not Test fixture || fixture.TypeInfo?.Type is null)
                        continue;

                    fixturesByType[fixture.TypeInfo.Type] = fixture;

                    if (fixture.Properties.Get(PropertyNames.DependsOnFixture) is Type dependencyType)
                        dependencyByFixture[fixture] = dependencyType;
                }

                // No need to create a dependency graph if there are no dependencies
                if (dependencyByFixture.Count == 0)
                {
                    return null;
                }

                // Build the dependency graph, which maps each fixture to its dependent fixture (if any)
                var dependencyGraph = new Dictionary<ITest, ITest>(suite.Tests.Count);

                foreach (var child in suite.Tests)
                {
                    if (child is not Test fixture || fixture.TypeInfo?.Type is null)
                        continue;

                    if (!dependencyByFixture.TryGetValue(fixture, out Type? dependencyType))
                        continue;

                    if (fixturesByType.TryGetValue(dependencyType, out Test? dependencyFixture))
                    {
                        dependencyGraph[fixture] = dependencyFixture;
                        fixture.DependantTest = dependencyFixture;
                        continue;
                    }

                    fixture.MakeInvalid($"Test dependency {dependencyType} can not be found. Please verify it was configured correctly and was not filtered out.");
                }

                return dependencyGraph;
            }
        }

        private static void MarkInvalidDependenciesAsInvalid(Dictionary<ITest, ITest> dependencyGraph)
        {
            const string directReferenceReason = "Circular or self-referential test dependency detected.";

            var invalidCircularDependencies = new HashSet<ITest>();
            var visited = new HashSet<ITest>();
            var visiting = new List<ITest>();

            foreach (ITest test in dependencyGraph.Keys)
                Visit(test);

            void Visit(ITest test)
            {
                if (!visited.Add(test))
                    return;

                visiting.Add(test);

                if (dependencyGraph.TryGetValue(test, out ITest? dependencyTest))
                {
                    if (visiting.Contains(dependencyTest))
                    {
                        int cycleStartIndex = visiting.IndexOf(dependencyTest);
                        if (cycleStartIndex >= 0)
                            MarkCycle(visiting, cycleStartIndex);
                    }
                    else
                    {
                        Visit(dependencyTest);
                    }

                    if (invalidCircularDependencies.Contains(dependencyTest))
                        MarkInvalidCircularDependency(test, directReferenceReason);
                }

                visiting.RemoveAt(visiting.Count - 1);
            }

            void MarkInvalidCircularDependency(ITest test, string reason)
            {
                if (invalidCircularDependencies.Add(test) && test is Test t)
                    t.MakeInvalid(reason);
            }

            void MarkCycle(List<ITest> dependencyPath, int cycleStartIndex)
            {
                for (int i = cycleStartIndex; i < dependencyPath.Count; i++)
                    MarkInvalidCircularDependency(dependencyPath[i], directReferenceReason);
            }
        }

        private static void MarkDependencyFeatureConflictsAsInvalid(Dictionary<ITest, ITest> dependencyGraph)
        {
            const string orderReason = "Test may not participate in both DependsOn and Order chains.";
            const string parallelReason = "Test dependency chains may not include tests configured for parallel execution.";

            foreach (var pair in dependencyGraph)
            {
                if (pair.Key is not Test test || pair.Value is not Test dependantTest)
                    continue;

                MarkDependencyFeatureConflictsAsInvalid(test);
                MarkDependencyFeatureConflictsAsInvalid(dependantTest);
            }

            static void MarkDependencyFeatureConflictsAsInvalid(Test test)
            {
                if (test.Properties.ContainsKey(PropertyNames.Order))
                    test.MakeInvalid(orderReason);

                if (IsConfiguredParallelWithOtherTests(test))
                    test.MakeInvalid(parallelReason);
            }

            static bool IsConfiguredParallelWithOtherTests(Test test)
            {
                var scope = test.GetEffectiveProperty(PropertyNames.ParallelScope, ParallelScope.Default);
                return scope.HasFlag(ParallelScope.Self) && !scope.HasFlag(ParallelScope.None);
            }
        }
    }
}
