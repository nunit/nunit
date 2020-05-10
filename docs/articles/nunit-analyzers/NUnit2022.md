# NUnit2022
## Missing property required for constraint.

| Topic    | Value
| :--      | :--
| Id       | NUnit2022
| Severity | Warning
| Enabled  | True
| Category | Assertion
| Code     | [MissingPropertyAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/MissingProperty/MissingPropertyAnalyzer.cs)


## Description

Provided 'actual' argument should have required property for constraint.

## Motivation

Using property constraints (e.g. `Has.Count.EqualTo(1)`, `Has.Property("Prop").EqualTo(expected)`, etc) 
makes sense only when provided actual argument has those properties defined.

```csharp
[Test]
public void Test()
{
    var enumerable = new [] {1,2,3}.Where(i => i > 1);

    // Actual argument type 'IEnumerable<int>' has no property 'Count'.
    Assert.That(enumerable, Has.Count.EqualTo(2));
}
```

## How to fix violations

Fix your property name, or use another constraint.

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```csharp
#pragma warning disable NUnit2022 // Missing property required for constraint.
Code violating the rule here
#pragma warning restore NUnit2022 // Missing property required for constraint.
```

Or put this at the top of the file to disable all instances.
```csharp
#pragma warning disable NUnit2022 // Missing property required for constraint.
```

### Via attribute `[SuppressMessage]`.

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2022:Missing property required for constraint.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
