# NUnit1007
## Method has non-void return type, but no result is expected in ExpectedResult.

| Topic    | Value
| :--      | :--
| Id       | NUnit1007
| Severity | Error
| Enabled  | True
| Category | Structure
| Code     | [TestMethodUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/TestMethodUsage/TestMethodUsageAnalyzer.cs)


## Description

Method has non-void return type, but no result is expected in ExpectedResult.

## Motivation

To prevent tests that will fail at runtime due to improper construction.

## How to fix violations

### Example Violation

```csharp
[TestCase(1)]
public string NUnit1007SampleTest(int inputValue)
{
    return "";
}
```

### Explanation

No `ExpectedResult` was defined, but the return type of the method in our sample is of type `string`, meaning it does indeed return a result and we should use the `ExpectedResult` syntax in order to capture it.

### Fix

Either modify the `TestCase` to add an `ExpectedResult`:

```csharp
[TestCase(1, ExpectedResult = "")]
public string NUnit1007SampleTest(int inputValue)
{
    return "";
}
```

Or modify the return type of the test method to be `void`:

```csharp
[TestCase(1)]
public void NUnit1007SampleTest(int inputValue)
{
    return Assert.That(inputValue, Is.EqualTo(1));
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```csharp
#pragma warning disable NUnit1007 // Method has non-void return type, but no result is expected in ExpectedResult.
Code violating the rule here
#pragma warning restore NUnit1007 // Method has non-void return type, but no result is expected in ExpectedResult.
```

Or put this at the top of the file to disable all instances.
```csharp
#pragma warning disable NUnit1007 // Method has non-void return type, but no result is expected in ExpectedResult.
```

### Via attribute `[SuppressMessage]`.

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", 
    "NUnit1007:Method has non-void return type, but no result is expected in ExpectedResult.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
