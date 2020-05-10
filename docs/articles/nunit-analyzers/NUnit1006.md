# NUnit1006
## ExpectedResult must not be specified when the method returns void.

| Topic    | Value
| :--      | :--
| Id       | NUnit1006
| Severity | Error
| Enabled  | True
| Category | Structure
| Code     | [TestMethodUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/TestMethodUsage/TestMethodUsageAnalyzer.cs)


## Description

ExpectedResult must not be specified when the method returns void. This will lead to an error at run-time.

## Motivation

To prevent tests that will fail at runtime due to improper construction.

## How to fix violations

### Example Violation

```csharp
[TestCase(1, ExpectedResult = "1")]
public void NUnit1006SampleTest(int inputValue)
{
    return;
}
```

### Explanation

An `ExpectedResult` was defined, but the return type of the method in our sample is of type `void`, meaning it does not return a result.

### Fix

Either modify the `TestCase` to remove the `ExpectedResult`:

```csharp
[TestCase(1)]
public void NUnit1006SampleTest(int inputValue)
{
    Assert.That(inputValue, Is.EqualTo(1));
}
```

Or modify the return type of the test method:

```csharp
[TestCase(1, ExpectedResult = "1")]
public string NUnit1006SampleTest(int inputValue)
{
    return inputValue.ToString();
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit1006 // ExpectedResult must not be specified when the method returns void.
Code violating the rule here
#pragma warning restore NUnit1006 // ExpectedResult must not be specified when the method returns void.
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit1006 // ExpectedResult must not be specified when the method returns void.
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", 
    "NUnit1006:ExpectedResult must not be specified when the method returns void.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
