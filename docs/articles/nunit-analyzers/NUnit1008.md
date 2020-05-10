# NUnit1008
## Specifying ParallelScope.Self on assembly level has no effect.

| Topic    | Value
| :--      | :--
| Id       | NUnit1008
| Severity | Warning
| Enabled  | True
| Category | Structure
| Code     | [ParallelizableUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/ParallelizableUsage/ParallelizableUsageAnalyzer.cs)


## Description

Specifying ParallelScope.Self on assembly level has no effect.

## Motivation

Bring developers' attention to a scenario in which they may believe they are parallelizing something when in fact they are not and their efforts will have no effect.

## How to fix violations

### Example Violation

In `AssemblyInfo.cs`:

```csharp
[assembly: Parallelizable(ParallelScope.Self)]
```

### Explanation

`ParallelScope.Self` [only applies to classes and methods](https://github.com/nunit/docs/wiki/Parallelizable-Attribute), not to assemblies.

### Fix

Either remove it or change to a valid option, such as:

```csharp
[assembly: Parallelizable(ParallelScope.Children)]
```

Or:

```csharp
[assembly: Parallelizable(ParallelScope.Fixtures)]
```

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```csharp
#pragma warning disable NUnit1008 // Specifying ParallelScope.Self on assembly level has no effect.
Code violating the rule here
#pragma warning restore NUnit1008 // Specifying ParallelScope.Self on assembly level has no effect.
```

Or put this at the top of the file to disable all instances.
```csharp
#pragma warning disable NUnit1008 // Specifying ParallelScope.Self on assembly level has no effect.
```

### Via attribute `[SuppressMessage]`.

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", 
    "NUnit1008:Specifying ParallelScope.Self on assembly level has no effect.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
