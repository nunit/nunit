# NUnit2018
## Consider using Assert.That(expr, Is.Not.Null) instead of Assert.NotNull(expr).

| Topic    | Value
| :--      | :--
| Id       | NUnit2018
| Severity | Info
| Enabled  | True
| Category | Assertion
| Code     | [ClassicModelAssertUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/ClassicModelAssertUsage/ClassicModelAssertUsageAnalyzer.cs)


## Description

Consider using the constraint model, `Assert.That(expr, Is.Not.Null)`, instead of the classic model, `Assert.NotNull(expr)`.

## Motivation

The classic Assert model contains less flexibility than the constraint model,
so this analyzer marks usages of `Assert.NotNull` from the classic Assert model.

```csharp
[Test]
public void Test()
{
    object obj = null;
    Assert.NotNull(obj);
}
```

## How to fix violations

The analyzer comes with a code fix that will replace `Assert.NotNull(expression)` with
`Assert.That(expression, Is.Not.Null)`. So the code block above will be changed into.

```csharp
[Test]
public void Test()
{
    object obj = null;
    Assert.That(obj, Is.Not.Null);
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```csharp
#pragma warning disable NUnit2018 // Consider using Assert.That(expr, Is.Not.Null) instead of Assert.NotNull(expr).
Code violating the rule here
#pragma warning restore NUnit2018 // Consider using Assert.That(expr, Is.Not.Null) instead of Assert.NotNull(expr).
```

Or put this at the top of the file to disable all instances.
```csharp
#pragma warning disable NUnit2018 // Consider using Assert.That(expr, Is.Not.Null) instead of Assert.NotNull(expr).
```

### Via attribute `[SuppressMessage]`.

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2018:Consider using Assert.That(expr, Is.Not.Null) instead of Assert.NotNull(expr).",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
