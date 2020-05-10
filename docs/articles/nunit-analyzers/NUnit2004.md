# NUnit2004
## Consider using Assert.That(expr, Is.True) instead of Assert.True(expr).

| Topic    | Value
| :--      | :--
| Id       | NUnit2004
| Severity | Info
| Enabled  | True
| Category | Assertion
| Code     | [ClassicModelAssertUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/ClassicModelAssertUsage/ClassicModelAssertUsageAnalyzer.cs)


## Description

Consider using the constraint model, `Assert.That(expr, Is.True)`, instead of the classic model, `Assert.True(expr)`.

## Motivation

The classic Assert model contains less flexibility than the constraint model,
so this analyzer marks usages of `Assert.True` from the classic Assert model.

```csharp
[Test]
public void Test()
{
    Assert.True(expression);
}
```

## How to fix violations

The analyzer comes with a code fix that will replace `Assert.True(expression)` with
`Assert.That(expression, Is.True)`. So the code block above will be changed into.

```csharp
[Test]
public void Test()
{
    Assert.That(expression, Is.True);
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit2004 // Consider using Assert.That(expr, Is.True) instead of Assert.True(expr).
Code violating the rule here
#pragma warning restore NUnit2004 // Consider using Assert.That(expr, Is.True) instead of Assert.True(expr).
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit2004 // Consider using Assert.That(expr, Is.True) instead of Assert.True(expr).
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2004:Consider using Assert.That(expr, Is.True) instead of Assert.True(expr).",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
