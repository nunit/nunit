# NUnit2014
## Use SomeItemsConstraint.

| Topic    | Value
| :--      | :--
| Id       | NUnit2014
| Severity | Info
| Enabled  | True
| Category | Assertion
| Code     | [SomeItemsConstraintUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/ConstraintUsage/SomeItemsConstraintUsageAnalyzer.cs)


## Description

Using SomeItemsConstraint will lead to better assertion messages in case of failure.

## Motivation

Using `Does.Contain` (or `Does.Not.Contain`) constraint will lead to better assertion messages in case of failure, 
so this analyzer marks all usages of string `Contains` method where it is possible to replace 
with `Does.Contain` constraint.

```csharp
[Test]
public void Test()
{
    var actual = new List<int> {1,2,3};
    int expected = 1;
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
    var actual = new List<int> {1,2,3};
    int expected = 1;
    Assert.That(actual, Does.Contain(expected));
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit2014 // Use SomeItemsConstraint.
Code violating the rule here
#pragma warning restore NUnit2014 // Use SomeItemsConstraint.
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit2014 // Use SomeItemsConstraint.
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2014:Use SomeItemsConstraint.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
