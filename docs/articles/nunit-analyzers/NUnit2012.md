# NUnit2012
## Use StartsWithConstraint.

| Topic    | Value
| :--      | :--
| Id       | NUnit2012
| Severity | Info
| Enabled  | True
| Category | Assertion
| Code     | [StringConstraintUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/ConstraintUsage/StringConstraintUsageAnalyzer.cs)


## Description

Using constraints instead of boolean methods will lead to better assertion messages in case of failure.

## Motivation

Using `Does.StartWith` (or `Does.Not.StartWith`) constraint will lead to better assertion messages in case of failure, 
so this analyzer marks all usages of string `StartsWith` method where it is possible to replace 
with `Does.StartWith` constraint.

```csharp
[Test]
public void Test()
{
    string actual = "...";
    string expected = "...";
    Assert.True(actual.StartsWith(expected));
}
```

## How to fix violations

The analyzer comes with a code fix that will replace `Assert.True(actual.StartWith(expected))` with
`Assert.That(actual, Does.StartWith(expected))`. So the code block above will be changed into

```csharp
[Test]
public void Test()
{
    string actual = "...";
    string expected = "...";
    Assert.That(actual, Does.StartWith(expected));
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```csharp
#pragma warning disable NUnit2012 // Use StartsWithConstraint.
Code violating the rule here
#pragma warning restore NUnit2012 // Use StartsWithConstraint.
```

Or put this at the top of the file to disable all instances.
```csharp
#pragma warning disable NUnit2012 // Use StartsWithConstraint.
```

### Via attribute `[SuppressMessage]`.

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2012:Use StartsWithConstraint.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
