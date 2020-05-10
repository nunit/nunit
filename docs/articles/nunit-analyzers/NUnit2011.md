# NUnit2011
## Use ContainsConstraint.

| Topic    | Value
| :--      | :--
| Id       | NUnit2011
| Severity | Info
| Enabled  | True
| Category | Assertion
| Code     | [StringConstraintUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/ConstraintUsage/StringConstraintUsageAnalyzer.cs)


## Description

Using constraints instead of boolean methods will lead to better assertion messages in case of failure.

## Motivation

Using `Does.Contain` (or `Does.Not.Contain`) constraint will lead to better assertion messages in case of failure, 
so this analyzer marks all usages of string `Contains` method where it is possible to replace 
with `Does.Contain` constraint.

```csharp
[Test]
public void Test()
{
    string actual = "...";
    string expected = "...";
    Assert.True(actual.Contains(expected));
}
```

## How to fix violations

The analyzer comes with a code fix that will replace `Assert.True(actual.Contains(expected))` with
`Assert.That(actual, Does.Contain(expected))`. So the code block above will be changed into

```csharp
[Test]
public void Test()
{
    string actual = "...";
    string expected = "...";
    Assert.That(actual, Does.Contain(expected));
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit2011 // Use ContainsConstraint.
Code violating the rule here
#pragma warning restore NUnit2011 // Use ContainsConstraint.
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit2011 // Use ContainsConstraint.
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2011:Use ContainsConstraint.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
