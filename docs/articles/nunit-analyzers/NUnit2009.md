# NUnit2009
## Same value provided as actual and expected argument.

| Topic    | Value
| :--      | :--
| Id       | NUnit2009
| Severity | Warning
| Enabled  | True
| Category | Structure
| Code     | [SameActualExpectedValueAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/SameActualExpectedValue/SameActualExpectedValueAnalyzer.cs)


## Description

The same value has been provided as both actual and expected argument. This indicates a coding error.

## Motivation

To bring developers' attention to a situation in which their code may not be operating as expected and their test may not be testing what they expect.

## How to fix violations

### Sample Violation

```csharp
[Test]
public void Nunit2009SampleTest()
{
    var x = 1;
    Assert.That(x, Is.EqualTo(x));
}
```

### Explanation

In the above example, the test will always be correct, because we're comparing the same value. That is to say, we're not actually testing anything.

### Fix

Ensure the `expected` and `actual` values come from different places.

```csharp
[Test]
public void Nunit2009SampleTest()
{
    var x = 1;
    Assert.That(x, Is.EqualTo(1));
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```csharp
#pragma warning disable NUnit2009 // Same value provided as actual and expected argument.
Code violating the rule here
#pragma warning restore NUnit2009 // Same value provided as actual and expected argument.
```

Or put this at the top of the file to disable all instances.
```csharp
#pragma warning disable NUnit2009 // Same value provided as actual and expected argument.
```

### Via attribute `[SuppressMessage]`.

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", 
    "NUnit2009:Same value provided as actual and expected argument.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
