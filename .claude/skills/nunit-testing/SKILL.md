---
name: nunit-testing
description: Use when writing or modifying tests in NUnit's own test projects, or when making a behavioral change to production code that needs test coverage. Covers test structure, attribute choice, helper visibility, platform guards, and which test projects are real.
---

# NUnit testing

Every behavioral change to production code ships with a test. Bug fixes include a regression test that would have failed before the fix. New features cover the golden path plus edge cases a reviewer is likely to ask about (null, empty, boundary sizes, double-dispose, collections larger than common loop-unroll thresholds).

## Avoid modifying existing tests

If you modify an existing test it means you have potentially modified behavior, and even though its not a breaking contract change, it might be a breaking functional change, prefer adding new tests over modifying existing ones.

## Member references — use `nameof`, not strings

Hard-coded strings that refer to members are brittle: they don't move when the member is renamed, they don't surface in find-references, and they silently rot.

```csharp
// Avoid
Assert.That(result.TestCaseCount, Is.EqualTo(2));
Assert.That(context.CurrentTest.Name, Is.EqualTo("OneTimeSetup"));

// Prefer
Assert.That(result.TestCaseCount, Is.EqualTo(fixture.TestCaseCount));
Assert.That(context.CurrentTest.Name, Is.EqualTo(nameof(AsyncDisposableFixture.OneTimeSetup)));
```

## Parameterisation — prefer `[TestCase]` and `[Values]` over `[Theory]`

`[Theory]` triggers engine-side scanning for `[DataPoint]` sources and is rarely used in NUnit's own tests. Idiomatic parameterisation:

```csharp
[TestCase(1, ExpectedResult = 1)]
[TestCase(5, ExpectedResult = 120)]
public int Factorial(int n) => Compute(n);

[Test]
public void Addition([Values(0, 1, 5)] int a, [Values(0, 1, 5)] int b)
{
    Assert.That(Add(a, b), Is.EqualTo(a + b));
}
```

Use `[TestCase]` when you have a small fixed set of (input, expected) tuples. Use `[Values]` when you want the cartesian product of several parameter values.

## Timing-sensitive tests — `[Explicit]`, not flaky

Tests that depend on wall-clock timing (`Thread.Sleep`, `Task.Delay`, polling intervals) are unreliable on CI: GitHub runners can be slow and heterogeneous. If a test is timing-sensitive:

- Prefer rewriting to avoid wall-clock dependence (inject a virtual clock, assert on call counts, etc.).
- If you genuinely need wall-clock behaviour, mark the test `[Explicit]`. It will still run locally and in targeted runs, but won't flake the main CI.

## Platform-specific tests — gate with `[Platform]` or runtime checks

For features that only make sense on one OS (long paths, Windows registry, etc.):

```csharp
[Test]
[Platform(Exclude = "MACOSX")]
public void LongPathSupport() { ... }
```

For runtime-only detection use `OSPlatform.IsWindows10` or similar inside the test. Don't rely on `#if NETFRAMEWORK` alone — Mono runs `NETFRAMEWORK` binaries on Linux and macOS.

## Test helpers inside a fixture — `private` or `[NonTest]`

NUnit discovers public methods in a fixture as tests. A non-test helper you call from a test must be `private` (or `internal`), or decorated with `[NonTest]`. A `public` helper without `[Test]` triggers the analyzer and will be treated as a test candidate.

```csharp
[TestFixture]
public class MyFixture
{
    [Test]
    public void UsesHelper()
    {
        Assert.That(BuildInput(), Is.EqualTo("abc"));
    }

    // Private: not discovered as a test
    private string BuildInput() => "abc";
}
```

If the helper needs to be accessible from another test file, use `internal` plus `InternalsVisibleTo`, not `public`.

## Test data on the fixture — keep it `private`

`[TestCaseSource]` / `[ValueSource]` members used within a single fixture should be `private static` (or `private`). Public `SourceData` members are a common accidental test-discovery trigger.

```csharp
private static IEnumerable<int> Primes => new[] { 2, 3, 5, 7, 11 };

[TestCaseSource(nameof(Primes))]
public void IsPrime(int n) { ... }
```

## Double-disposal, null, empty — edge cases reviewers ask about

When adding tests for a type that owns a resource or a collection, cover:

- **Double-dispose**: calling `Dispose()` twice shouldn't throw.
- **Null / empty inputs**: what does the method return or throw?
- **Boundary sizes**: tuples > 7 elements, collections > typical stackalloc thresholds, deeply nested inputs.
- **Exception paths**: property getters that throw, converters that fail partway through.

## Test project layout — two kinds of projects

Test projects in this repo come in two flavours:

- **Real test projects** (`nunit.framework.tests-*`, `nunit.framework.legacy.tests-*`, `nunitlite.tests-*`) contain tests that should pass. New unit tests go here.
- **Fixture projects** contain test classes that are *intentionally broken* — failing assertions, exceptions in setup, duplicate names, etc. They exist as **input data** for integration tests that load them with the NUnit engine and assert on the resulting failures.

When adding a new test, put it in a real test project. When you see red in a fixture project, don't fix it — it's the expected shape of the data. If you need a new broken scenario, add a new fixture class in the fixture project rather than modifying an existing one.

## Regression tests cite the issue

A bug-fix PR's test should reproduce the original symptom. It's conventional to reference the issue in the test name or comment so the intent is clear:

```csharp
[Test]
public void TearDownOutputIsPreserved_Issue4598()
{
    // Regression: output written during TearDown used to be lost.
    ...
}
```
