# NUnit1011
## TestCaseSource argument does not specify an existing member.

| Topic    | Value
| :--      | :--
| Id       | NUnit1011
| Severity | Error
| Enabled  | True
| Category | Structure
| Code     | [TestCaseSourceUsesStringAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/TestCaseSourceUsage/TestCaseSourceUsesStringAnalyzer.cs)


## Description

TestCaseSource argument does not specify an existing member. This will lead to an error at run-time.

## Motivation

To prevent tests that will fail at runtime due to improper construction.

## How to fix violations

### Example Violation

```csharp
[TestCaseSource("MyIncorrectTestSource")]
public void NUnit1011SampleTest(string stringValue)
{
    Assert.That(stringValue.Length, Is.EqualTo(3));
}

public static object[] MyTestSource()
{
    return new object[] {"One", "Two"};
}
```

### Explanation

In the example above, the test case source is named `MyIncorrectTestCaseSource`, but the test case source is actually named `MyTestSource`. Because the names don't match, this will be an error.

### Fix

Rename the `TestCaseSource` to match:

```csharp
[TestCaseSource("MyTestSource")]
public void NUnit1011SampleTest(string stringValue)
{
    Assert.That(stringValue.Length, Is.EqualTo(3));
}

public static object[] MyTestSource()
{
    return new object[] {"One", "Two"};
}
```

Or even better, use `nameof` so that the compiler may assist with mis-matched names in the future:

```csharp
[TestCaseSource(nameof(MyTestSource))]
public void NUnit1011SampleTest(string stringValue)
{
    Assert.That(stringValue.Length, Is.EqualTo(3));
}

public static object[] MyTestSource()
{
    return new object[] {"One", "Two"};
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit1011 // TestCaseSource argument does not specify an existing member.
Code violating the rule here
#pragma warning restore NUnit1011 // TestCaseSource argument does not specify an existing member.
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit1011 // TestCaseSource argument does not specify an existing member.
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", 
    "NUnit1011:TestCaseSource argument does not specify an existing member.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
