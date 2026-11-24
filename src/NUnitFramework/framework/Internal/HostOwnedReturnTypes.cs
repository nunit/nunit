// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Threading;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Return types that the test host runs itself, from
    /// <c>FrameworkPackageSettings.HostOwnedReturnTypes</c>. While an assembly is being built,
    /// test, setup and teardown methods returning these types are accepted instead of being
    /// marked not runnable. NUnit's own runner never runs them.
    /// </summary>
    internal static class HostOwnedReturnTypes
    {
        // Scoped to one DefaultTestAssemblyBuilder.Build call: the builders below it
        // (TestFixture constructor, NUnitTestCaseBuilder) have no access to the load settings,
        // and several assemblies may be built in parallel.
        private static readonly AsyncLocal<HashSet<string>?> Current = new();

        internal static IDisposable BeginBuild(IEnumerable<string>? typeNames)
        {
            var names = typeNames is null ? null : new HashSet<string>(typeNames, StringComparer.Ordinal);
            var scope = new Scope(Current.Value);
            Current.Value = names is { Count: > 0 } ? names : null;
            return scope;
        }

        internal static bool Contains(Type returnType)
        {
            var names = Current.Value;

            // Types NUnit runs itself (void, unit, awaitables) keep NUnit's own rules.
            if (names is null || !IsNotRunnableByNUnit(returnType))
                return false;

            // A generic type matches by its own full name or by its definition's,
            // so "System.Collections.Generic.IEnumerator`1" covers every IEnumerator<T>.
            return (returnType.FullName is not null && names.Contains(returnType.FullName))
                || (returnType.IsGenericType && returnType.GetGenericTypeDefinition().FullName is { } definition
                    && names.Contains(definition));
        }

        /// <summary>
        /// True for a return type NUnit cannot run itself: not void/unit and not awaitable.
        /// Such a method only gets past load-time validation as a host-owned method
        /// (or as a test with an expected result).
        /// </summary>
        internal static bool IsNotRunnableByNUnit(Type returnType) =>
            !Reflect.IsVoidOrUnit(returnType) && !AwaitAdapter.IsAwaitable(returnType);

        internal static string NotRunByNUnitMessage(Type returnType) =>
            $"NUnit cannot run a test method returning '{returnType.FullName ?? returnType.ToString()}' without an expected result. " +
            "Only a test host that runs this return type itself can run it (see FrameworkPackageSettings.HostOwnedReturnTypes).";

        internal static string SetUpTearDownNotRunByNUnitMessage(Type returnType) =>
            $"NUnit cannot run a setup or teardown method returning '{returnType.FullName ?? returnType.ToString()}'. " +
            "Only a test host that runs this return type itself can run it (see FrameworkPackageSettings.HostOwnedReturnTypes).";

        private sealed class Scope : IDisposable
        {
            private readonly HashSet<string>? _previous;

            public Scope(HashSet<string>? previous) => _previous = previous;

            public void Dispose() => Current.Value = _previous;
        }
    }
}
