// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Internal.Execution
{
    internal abstract class TestDependencyManager
    {
        protected const string FileCycleReason = "Circular or self-referential test dependency detected.";
        protected const string FailCycleBaseReason = "Circular or self-referential test dependency detected via base class.";
        protected const string InvalidOrderReason = "Test may not participate in both DependsOn and Order chains.";
        protected const string InvalidParallelReason = "Test dependency chains may not include tests configured for parallel execution.";

        protected abstract Dictionary<ITest, ITest>? BuildDependencyGraph(TestSuite parent);

        public static TestDependencyManager? Create(TestSuite test)
        {
            if (test.Tests.Count == 0)
                return null;

            var child = test.Tests[0];

            if (child is TestFixture)
            {
                return new FixtureDependencyManager();
            }
            else if (child is TestMethod or ParameterizedMethodSuite)
            {
                return new MethodDependencyManager();
            }

            return null;
        }

        public Dictionary<ITest, ITest>? PrepareTestDependencies(TestSuite test)
        {
            Dictionary<ITest, ITest>? dependencyGraph = BuildDependencyGraph(test);
            if (dependencyGraph is null)
                return null;

            // Validate dependency graph
            MarkInvalidDependenciesAsInvalid(dependencyGraph);
            MarkDependencyFeatureConflictsAsInvalid(dependencyGraph);

            return dependencyGraph;
        }

        private static void MarkInvalidDependenciesAsInvalid(Dictionary<ITest, ITest> dependencyGraph)
        {
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
                        MarkInvalidCircularDependency(test, FileCycleReason);
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
                    MarkInvalidCircularDependency(dependencyPath[i], FileCycleReason);
            }
        }

        private static void MarkDependencyFeatureConflictsAsInvalid(Dictionary<ITest, ITest> dependencyGraph)
        {
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
                    test.MakeInvalid(InvalidOrderReason);

                if (IsConfiguredParallelWithOtherTests(test))
                    test.MakeInvalid(InvalidParallelReason);
            }

            static bool IsConfiguredParallelWithOtherTests(Test test)
            {
                var scope = test.GetEffectiveProperty(PropertyNames.ParallelScope, ParallelScope.Default);
                return scope.HasFlag(ParallelScope.Self) && !scope.HasFlag(ParallelScope.None);
            }
        }
    }

    internal class MethodDependencyManager : TestDependencyManager
    {
        protected override Dictionary<ITest, ITest>? BuildDependencyGraph(TestSuite parent)
        {
            var testsByName = new Dictionary<string, Test>(parent.Tests.Count);
            var dependencyByMethod = new Dictionary<Test, string>();

            foreach (var child in parent.Tests)
            {
                if (child is not Test test)
                    continue;

                testsByName[test.Name] = test;

                if (test.Properties.Get(PropertyNames.DependsOnMethod) is string dependencyMethod)
                    dependencyByMethod[test] = dependencyMethod;
            }

            if (dependencyByMethod.Count == 0)
                return null;

            var dependencyGraph = new Dictionary<ITest, ITest>(parent.Tests.Count);

            foreach (var child in parent.Tests)
            {
                if (child is not Test test || !dependencyByMethod.TryGetValue(test, out string? dependencyMethod))
                    continue;

                if (testsByName.TryGetValue(dependencyMethod, out Test? dependencyTest))
                {
                    dependencyGraph[test] = dependencyTest;
                    test.DependantTest = dependencyTest;
                    continue;
                }

                test.MakeInvalid($"Test dependency {dependencyMethod} can not be found. Please verify it was configured correctly and was not filtered out.");
            }

            return dependencyGraph;
        }
    }

    internal class FixtureDependencyManager : TestDependencyManager
    {
        protected override Dictionary<ITest, ITest>? BuildDependencyGraph(TestSuite parent)
        {
            var fixturesByType = new Dictionary<Type, Test>(parent.Tests.Count);
            var dependencyByFixture = new Dictionary<Test, Type>();

            foreach (var child in parent.Tests)
            {
                if (child is not Test test)
                    continue;

                if (test.TypeInfo?.Type is Type fixtureType)
                    fixturesByType[fixtureType] = test;

                if (test.Properties.Get(PropertyNames.DependsOnFixture) is Type dependencyType)
                    dependencyByFixture[test] = dependencyType;
            }

            if (dependencyByFixture.Count == 0)
                return null;

            var dependencyGraph = new Dictionary<ITest, ITest>(parent.Tests.Count);

            foreach (var child in parent.Tests)
            {
                if (child is not Test test || !dependencyByFixture.TryGetValue(test, out Type? dependencyType))
                    continue;

                // Check for self-referential base dependency first
                if (test.TypeInfo?.Type is Type fixtureType && fixtureType.IsSubclassOf(dependencyType))
                {
                    test.MakeInvalid(FailCycleBaseReason);
                    continue;
                }

                if (fixturesByType.TryGetValue(dependencyType, out Test? dependencyFixture))
                {
                    dependencyGraph[test] = dependencyFixture;
                    test.DependantTest = dependencyFixture;
                    continue;
                }

                test.MakeInvalid($"Test dependency {dependencyType} can not be found. Please verify it was configured correctly and was not filtered out.");
            }

            return dependencyGraph;
        }
    }
}
