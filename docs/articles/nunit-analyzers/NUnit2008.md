# NUnit2008
## Incorrect IgnoreCase usage.

| Topic    | Value
| :--      | :--
| Id       | NUnit2008
| Severity | Warning
| Enabled  | True
| Category | Assertion
| Code     | [IgnoreCaseUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/IgnoreCaseUsage/IgnoreCaseUsageAnalyzer.cs)


## Description

IgnoreCase modifier should only be used for string or char arguments. Using it on another type will not have any effect.

## Motivation

To bring developers' attention to a scenario in which their code is actually having no effect and may reveal that their test is not doing what they expect.

## How to fix violations

### Example Violation

```csharp
[Test]
public void Nunit2008SampleTest()
{
    var date = DateTime.Now;
    Assert.That(date, Is.Not.EqualTo(date.AddDays(1)).IgnoreCase);
}
```

### Explanation

Using IgnoreCase here doesn't make any sense, because the types we're comparing don't have the concept of case. Therefore, it's only suitable to use on textual primitives (e.g. `string` and `char`).

### Fix

Remove the errant call to `IgnoreCase`:

```csharp
[Test]
public void Nunit2008SampleTest()
{
    var date = DateTime.Now;
    Assert.That(date, Is.Not.EqualTo(date.AddDays(1)));
}
```

Or update the code to compare based on the primitives that are supported by `IgnoreCase`:

```csharp
[Test]
public void Nunit2008SampleTest()
{
    var date = DateTime.Now;
    Assert.That(date.ToString(), Is.Not.EqualTo(date.AddDays(1).ToString()).IgnoreCase);
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit2008 // Incorrect IgnoreCase usage.
Code violating the rule here
#pragma warning restore NUnit2008 // Incorrect IgnoreCase usage.
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit2008 // Incorrect IgnoreCase usage.
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2008:Incorrect IgnoreCase usage.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
