# NUnit2015
## Consider using Assert.That(expr2, Is.SameAs(expr1)) instead of Assert.AreSame(expr1, expr2).

| Topic    | Value
| :--      | :--
| Id       | NUnit2015
| Severity | Warning
| Enabled  | True
| Category | Assertion
| Code     | [ClassicModelAssertUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/ClassicModelAssertUsage/ClassicModelAssertUsageAnalyzer.cs)


## Description

Consider using the constraint model, Assert.That(expr2, Is.SameAs(expr1)), instead of the classic model, Assert.AreSame(expr1, expr2).

## Motivation

The assert `Assert.AreSame` from the classic Assert model makes it easy to confuse the `expected` and the `actual` argument,
so this analyzer marks usages of `Assert.AreSame`.

```csharp
[Test]
public void Test()
{
    Assert.AreSame(expected, actual);
}
```

## How to fix violations

The analyzer comes with a code fix that will replace `Assert.AreSame(expected, actual)` with
`Assert.That(actual, Is.SameAs(expected))`. So the code block above will be changed into.

```csharp
[Test]
public void Test()
{
    Assert.That(actual, Is.SameAs(expected));
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit2015 // Consider using Assert.That(expr2, Is.SameAs(expr1)) instead of Assert.AreSame(expr1, expr2).
Code violating the rule here
#pragma warning restore NUnit2015 // Consider using Assert.That(expr2, Is.SameAs(expr1)) instead of Assert.AreSame(expr1, expr2).
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit2015 // Consider using Assert.That(expr2, Is.SameAs(expr1)) instead of Assert.AreSame(expr1, expr2).
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2015:Consider using Assert.That(expr2, Is.SameAs(expr1)) instead of Assert.AreSame(expr1, expr2).",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
