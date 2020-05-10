# NUnit1014
## Async test method must have Task<T> return type when a result is expected

| Topic    | Value
| :--      | :--
| Id       | NUnit1014
| Severity | Error
| Enabled  | True
| Category | Structure
| Code     | [TestMethodUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/TestMethodUsage/TestMethodUsageAnalyzer.cs)


## Description

Async test method must have Task<T> return type when a result is expected

## Motivation

To prevent tests that will fail at runtime due to improper construction.

## How to fix violations

### Example Violation

```csharp
[TestCase(1, ExpectedResult = true)]
public async Task Nunit1014SampleTest(int numberValue)
{
    return;
}
```

### Explanation

The NUnit `ExpectedResult` syntax is used, so this method needs to return a type that matches the type of expected result you're looking for.

### Fix

Remove the `ExpectedResult` syntax:

```csharp
[TestCase(1)]
public async Task Nunit1014SampleTest(int numberValue)
{
    Assert.Pass();
}
```

Or, update the return task type to be what you're looking for, e.g. `Task<bool>` below:

```csharp
[TestCase(1, ExpectedResult = true)]
public async Task<bool> Nunit1014SampleTest(int numberValue)
{
    return Task.FromResult(true);
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit1014 // Async test method must have Task<T> return type when a result is expected
Code violating the rule here
#pragma warning restore NUnit1014 // Async test method must have Task<T> return type when a result is expected
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit1014 // Async test method must have Task<T> return type when a result is expected
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", 
    "NUnit1014:Async test method must have Task<T> return type when a result is expected",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
