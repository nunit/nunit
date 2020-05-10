# NUnit1012
## Async test method must have non-void return type.

| Topic    | Value
| :--      | :--
| Id       | NUnit1012
| Severity | Error
| Enabled  | True
| Category | Structure
| Code     | [TestMethodUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/TestMethodUsage/TestMethodUsageAnalyzer.cs)


## Description

Async test method must have non-void return type.

## Motivation

To prevent tests that will fail at runtime due to improper construction.

## How to fix violations

### Example Violation

```csharp
[Test]
public async void NUnit1012SampleTest()
{
    var result = await Task.FromResult(true);
    Assert.That(result, Is.True);
}
```

### Explanation

`async` methods should generally not return `void` in C#. For example if an exception is thrown (as they are in the case of an assertion violation), the exception is actually a part of the task object. If the return type is `void`, no such object exists, to the exception is effectively swallowed.

### Fix

Make the `async` test method return a `Task`:

```csharp
[Test]
public async Task NUnit1012SampleTest()
{
    var result = await Task.FromResult(true);
    Assert.That(result, Is.True);
}
```

Or modify the test to not use `async` behavior:

```csharp
[Test]
public void NUnit1012SampleTest()
{
    var result = true;
    Assert.That(result, Is.True);
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```csharp
#pragma warning disable NUnit1012 // Async test method must have non-void return type.
Code violating the rule here
#pragma warning restore NUnit1012 // Async test method must have non-void return type.
```

Or put this at the top of the file to disable all instances.
```csharp
#pragma warning disable NUnit1012 // Async test method must have non-void return type.
```

### Via attribute `[SuppressMessage]`.

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", 
    "NUnit1012:Async test method must have non-void return type.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
