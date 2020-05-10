# NUnit1013
## Async test method must have non-generic Task return type when no result is expected.

| Topic    | Value
| :--      | :--
| Id       | NUnit1013
| Severity | Error
| Enabled  | True
| Category | Structure
| Code     | [TestMethodUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/TestMethodUsage/TestMethodUsageAnalyzer.cs)


## Description

Async test method must have non-generic Task return type when no result is expected.

## Motivation

To prevent tests that will fail at runtime due to improper construction.

## How to fix violations

### Example Violation

```csharp
[TestCase(1)]
public async Task<string> Nunit1013SampleTest(int numberValue)
{

    return await ConvertNumber(numberValue);
}

public Task<string> ConvertNumber(int numberValue)
{
    return Task.FromResult(numberValue.ToString());
}
```

### Explanation

The NUnit `ExpectedResult` syntax is not used, so it's an error for this method to return something that isn't being checked.

### Fix

Utilize the `ExpectedResult` syntax:

```csharp
[TestCase(1, ExpectedResult = "1")]
public async Task<string> Nunit1013SampleTest(int numberValue)
{
    return await ConvertNumber(numberValue);
}

public Task<string> ConvertNumber(int numberValue)
{
    return Task.FromResult(numberValue.ToString());
}
```

Or, use an assertion and a generic `Task` rather than `Task<string>`:

```csharp
[TestCase(1)]
public async Task Nunit1013SampleTest(int numberValue)
{
    var result = await ConvertNumber(numberValue);
    Assert.That(result, Is.EqualTo("1"));
}

public Task<string> ConvertNumber(int numberValue)
{
    return Task.FromResult(numberValue.ToString());
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit1013 // Async test method must have non-generic Task return type when no result is expected.
Code violating the rule here
#pragma warning restore NUnit1013 // Async test method must have non-generic Task return type when no result is expected.
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit1013 // Async test method must have non-generic Task return type when no result is expected.
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", 
    "NUnit1013:Async test method must have non-generic Task return type when no result is expected.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
