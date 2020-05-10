# NUnit2023
## Invalid NullConstraint usage.

| Topic    | Value
| :--      | :--
| Id       | NUnit2023
| Severity | Warning
| Enabled  | True
| Category | Assertion
| Code     | [NullConstraintUsageAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/NullConstraintUsage/NullConstraintUsageAnalyzer.cs)


## Description

NullConstraint is allowed only for reference types or nullable value types.

## Motivation

Non-nullable value types cannot have `null` value, therefore `Is.Null` assertions will always fail (or will always pass for `Is.Not.Null`).

## How to fix violations

Use suitable constraint.

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit2023 // Invalid NullConstraint usage.
Code violating the rule here
#pragma warning restore NUnit2023 // Invalid NullConstraint usage.
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit2023 // Invalid NullConstraint usage.
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2023:Invalid NullConstraint usage.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
