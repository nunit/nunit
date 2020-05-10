# NUnit2017
## Consider using Assert.That(expr, Is.Null) instead of Assert.IsNull(expr).

| Topic    | Value
| :--      | :--
| Id       | NUnit2017
| Severity | Info
| Enabled  | True
| Category | Assertion
| Code     | [ClassicModelAssertUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/ClassicModelAssertUsage/ClassicModelAssertUsageAnalyzer.cs)


## Description

Consider using the constraint model, `Assert.That(expr, Is.Null)`, instead of the classic model, `Assert.IsNull(expr)`.

## Motivation

The classic Assert model contains less flexibility than the constraint model,
so this analyzer marks usages of `Assert.IsNull` from the classic Assert model.

```csharp
[Test]
public void Test()
{
    object obj = null;
    Assert.IsNull(obj);
}
```

## How to fix violations

The analyzer comes with a code fix that will replace `Assert.IsNull(expression)` with
`Assert.That(expression, Is.Null)`. So the code block above will be changed into.

```csharp
[Test]
public void Test()
{
    object obj = null;
    Assert.That(obj, Is.Null);
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```csharp
#pragma warning disable NUnit2017 // Consider using Assert.That(expr, Is.Null) instead of Assert.IsNull(expr).
Code violating the rule here
#pragma warning restore NUnit2017 // Consider using Assert.That(expr, Is.Null) instead of Assert.IsNull(expr).
```

Or put this at the top of the file to disable all instances.
```csharp
#pragma warning disable NUnit2017 // Consider using Assert.That(expr, Is.Null) instead of Assert.IsNull(expr).
```

### Via attribute `[SuppressMessage]`.

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2017:Consider using Assert.That(expr, Is.Null) instead of Assert.IsNull(expr).",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
