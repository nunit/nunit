# NUnit1003
## Too few arguments provided by TestCaseAttribute.

| Topic    | Value
| :--      | :--
| Id       | NUnit1003
| Severity | Error
| Enabled  | True
| Category | Structure
| Code     | [TestCaseUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/TestCaseUsage/TestCaseUsageAnalyzer.cs)


## Description

The number of arguments provided by a TestCaseAttribute must match the number of parameters of the method.

## Motivation

To prevent tests that will fail at runtime due to improper construction.

## How to fix violations

### Example Violation

```csharp
[TestCase("1")]
public void NUnit1003SampleTest(string parameter1, string parameter2)
{
    Assert.That(parameter1, Is.EqualTo("1"));
    Assert.That(parameter2, Is.EqualTo("2"));
}
```

### Explanation

In the sample above, the test expects two parameters (`(string parameter1, string parameter2)`), but only one argument is supplied by the test case (`TestCase("1")`).

### Fix

Either add the additional argument:

```csharp
[TestCase("1", "2")]
public void NUnit1003SampleTest(string parameter1, string parameter2)
{
    Assert.That(parameter1, Is.EqualTo("1"));
    Assert.That(parameter2, Is.EqualTo("2"));
}
```

Or remove the use of that parameter:

```csharp
[TestCase("1")]
public void NUnit1003SampleTest(string parameter1)
{
    Assert.That(parameter1, Is.EqualTo("1"));
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit1003 // Too few arguments provided by TestCaseAttribute.
Code violating the rule here
#pragma warning restore NUnit1003 // Too few arguments provided by TestCaseAttribute.
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit1003 // Too few arguments provided by TestCaseAttribute.
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", 
    "NUnit1003:Too few arguments provided by TestCaseAttribute.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
