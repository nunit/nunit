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

                if (dependencyByFixture.TryGetValue(fixture, out Type? dependencyType)
                    && fixturesByType.TryGetValue(dependencyType, out Test? dependencyFixture))
                {
                    dependencyGraph[fixture] = dependencyFixture;
                    fixture.DependantTest = dependencyFixture;
                }
            }

            // Validate dependency graph
            MarkInvalidDependenciesAsInvalid(fixturesByType, dependencyByFixture);
            MarkDependencyFeatureConflictsAsInvalid(dependencyGraph);

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
