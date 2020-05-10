# NUnit1009
## No ParallelScope.Children on a non-parameterized test method.

| Topic    | Value
| :--      | :--
| Id       | NUnit1009
| Severity | Error
| Enabled  | True
| Category | Structure
| Code     | [ParallelizableUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/ParallelizableUsage/ParallelizableUsageAnalyzer.cs)


## Description

One may not specify ParallelScope.Children on a non-parameterized test method.

## Motivation

To prevent tests that will fail at runtime due to improper construction.

## How to fix violations

### Example Violation

```csharp
[Parallelizable(ParallelScope.Children)]
[Test]
public void NUnit1009SampleTest()
{
    Assert.Pass();
}
```

### Explanation

In the sample above, the `Parallelizable` attribute is used with `ParallelScope.Children`.

However, in a non-parameterized test, such as a `[Test]` and not a `[TestCase]`, there will be no children generated, and thus this type of parallelization does not make sense.

### Fix

Remove the attribute:

```csharp
[Test]
public void NUnit1009SampleTest()
{
    Assert.Pass();
}
```

Or, turn the test into one that will have children generated, such as a `TestCase`:

```csharp
[Parallelizable(ParallelScope.Children)] // These will now run in parallel
[TestCase(1)]
[TestCase(2)]
public void NUnit1009SampleTest(int numberValue)
{
    Assert.That(numberValue, Is.GreaterThan(0));
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```csharp
#pragma warning disable NUnit1009 // No ParallelScope.Children on a non-parameterized test method.
Code violating the rule here
#pragma warning restore NUnit1009 // No ParallelScope.Children on a non-parameterized test method.
```

Or put this at the top of the file to disable all instances.
```csharp
#pragma warning disable NUnit1009 // No ParallelScope.Children on a non-parameterized test method.
```

### Via attribute `[SuppressMessage]`.

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", 
    "NUnit1009:No ParallelScope.Children on a non-parameterized test method.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
