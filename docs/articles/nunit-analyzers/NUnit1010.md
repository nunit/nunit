# NUnit1010
## No ParallelScope.Fixtures on a test method.

| Topic    | Value
| :--      | :--
| Id       | NUnit1010
| Severity | Error
| Enabled  | True
| Category | Structure
| Code     | [ParallelizableUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/ParallelizableUsage/ParallelizableUsageAnalyzer.cs)


## Description

One may not specify ParallelScope.Fixtures on a test method.

## Motivation

To prevent tests that will fail at runtime due to improper construction.

## How to fix violations

### Example Violation

```csharp
[Parallelizable(ParallelScope.Fixtures)]
[Test]
public void NUnit1010SampleTest()
{
    Assert.Pass();
}
```

### Explanation

In the sample above, `ParallelScope.Fixtures` is specified.

However, in the context of a test method, a scope of `Fixtures` does not make sense. This scope [only applies at the assembly or class level](https://github.com/nunit/docs/wiki/Parallelizable-Attribute).

### Fix

Remove the attribute:

```csharp
[Test]
public void NUnit1010SampleTest()
{
    Assert.Pass();
}
```

Or apply this attribute at the class level:

```csharp
[Parallelizable(ParallelScope.Fixtures)]
public class MyTests
{
    [Test]
    public void NUnit1010SampleTest()
    {
        Assert.Pass();
    }
}
```

Or use a different attribute that applies at the test level:

```csharp
[Parallelizable(ParallelScope.Self)]
[Test]
public void NUnit1010SampleTest()
{
    Assert.Pass();
}
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit1010 // No ParallelScope.Fixtures on a test method.
Code violating the rule here
#pragma warning restore NUnit1010 // No ParallelScope.Fixtures on a test method.
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit1010 // No ParallelScope.Fixtures on a test method.
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", 
    "NUnit1010:No ParallelScope.Fixtures on a test method.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
