# NUnit1005
## The type of ExpectedResult must match the return type.

| Topic    | Value
| :--      | :--
| Id       | NUnit1005
| Severity | Error
| Enabled  | True
| Category | Structure
| Code     | [TestMethodUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/TestMethodUsage/TestMethodUsageAnalyzer.cs)


## Description

The type of ExpectedResult must match the return type. This will lead to an error at run-time.

## Motivation

To prevent tests that will fail at runtime due to improper construction.

## How to fix violations

### Example Violation

```csharp
[TestCase(1, ExpectedResult = true)]
public int NUnit1005SampleTest(int inputValue)
{
    return inputValue;
}
```

### Explanation

The sample above uses NUnit's `ExpectedResult` syntax. It defines a result of `true` (a `bool`) but the return type of the method is `int`.

### Fix

Either modify the TestCase parameter:

```csharp
[TestCase(1, ExpectedResult = 1)]
public int NUnit1005SampleTest(int inputValue)
{
    return inputValue;
}
```

Or modify the return type and logic of the method:

```csharp
[TestCase(1, ExpectedResult = true)]
public bool NUnit1005SampleTest(int inputValue)
{
    return inputValue > 0;
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit1005 // The type of ExpectedResult must match the return type.
Code violating the rule here
#pragma warning restore NUnit1005 // The type of ExpectedResult must match the return type.
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit1005 // The type of ExpectedResult must match the return type.
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", 
    "NUnit1005:The type of ExpectedResult must match the return type.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
