# NUnit2013
## Use EndsWithConstraint.

| Topic    | Value
| :--      | :--
| Id       | NUnit2013
| Severity | Info
| Enabled  | True
| Category | Assertion
| Code     | [StringConstraintUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/ConstraintUsage/StringConstraintUsageAnalyzer.cs)


## Description

Using constraints instead of boolean methods will lead to better assertion messages in case of failure.

## Motivation

Using `Does.EndWith` (or `Does.Not.EndWith`) constraint will lead to better assertion messages in case of failure, 
so this analyzer marks all usages of string `EndsWith` method where it is possible to replace 
with `Does.EndWith` constraint.

```csharp
[Test]
public void Test()
{
    string actual = "...";
    string expected = "...";
    Assert.True(actual.EndsWith(expected));
}
```

## How to fix violations

The analyzer comes with a code fix that will replace `Assert.True(actual.EndsWith(expected))` with
`Assert.That(actual, Does.EndWith(expected))`. So the code block above will be changed into

```csharp
[Test]
public void Test()
{
    string actual = "...";
    string expected = "...";
    Assert.That(actual, Does.EndWith(expected));
}

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```csharp
#pragma warning disable NUnit2013 // Use EndsWithConstraint.
Code violating the rule here
#pragma warning restore NUnit2013 // Use EndsWithConstraint.
```

Or put this at the top of the file to disable all instances.
```csharp
#pragma warning disable NUnit2013 // Use EndsWithConstraint.
```

### Via attribute `[SuppressMessage]`.

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2013:Use EndsWithConstraint.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
