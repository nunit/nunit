# NUnit2005
## Consider using Assert.That(expr2, Is.EqualTo(expr1)) instead of Assert.AreEqual(expr1, expr2).

| Topic    | Value
| :--      | :--
| Id       | NUnit2005
| Severity | Warning
| Enabled  | True
| Category | Assertion
| Code     | [ClassicModelAssertUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/ClassicModelAssertUsage/ClassicModelAssertUsageAnalyzer.cs)


## Description

Consider using the constraint model, `Assert.That(expr2, Is.EqualTo(expr1))`, instead of the classic model, `Assert.AreEqual(expr1, expr2)`.

## Motivation

The classic Assert model, `Assert.AreEqual(expected, actual)`, makes it easy to mix the `expected` and the `actual` parameter,
so this analyzer marks usages of `Assert.AreEqual` from the classic Assert model.

```csharp
[Test]
public void Test()
{
    Assert.AreEqual(expression1, expression2);
}
```

## How to fix violations

The analyzer comes with a code fix that will replace `Assert.AreEqual(expression1, expression2)` with
`Assert.That(expression2, Is.EqualTo(expression1))`. So the code block above will be changed into.

```csharp
[Test]
public void Test()
{
    Assert.That(expression2, Is.EqualTo(expression1));
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```csharp
#pragma warning disable NUnit2005 // Consider using Assert.That(expr2, Is.EqualTo(expr1)) instead of Assert.AreEqual(expr1, expr2).
Code violating the rule here
#pragma warning restore NUnit2005 // Consider using Assert.That(expr2, Is.EqualTo(expr1)) instead of Assert.AreEqual(expr1, expr2).
```

Or put this at the top of the file to disable all instances.
```csharp
#pragma warning disable NUnit2005 // Consider using Assert.That(expr2, Is.EqualTo(expr1)) instead of Assert.AreEqual(expr1, expr2).
```

### Via attribute `[SuppressMessage]`.

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2005:Consider using Assert.That(expr2, Is.EqualTo(expr1)) instead of Assert.AreEqual(expr1, expr2).",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
