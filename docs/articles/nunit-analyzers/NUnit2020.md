# NUnit2020
## Incompatible types for SameAs constraint.

| Topic    | Value
| :--      | :--
| Id       | NUnit2020
| Severity | Warning
| Enabled  | True
| Category | Assertion
| Code     | [SameAsIncompatibleTypesAnalyzer](https://github.com/nunit/nunit.analyzers/blob/0.2.0/src/nunit.analyzers/SameAsIncompatibleTypes/SameAsIncompatibleTypesAnalyzer.cs)


## Description

Provided actual and expected arguments cannot have same type, therefore SameAs assertion will always fail.

## Motivation

```csharp
class Foo { }
class Bar { }

var foo = new Foo();
var bar = new Bar();

Assert.That(foo, Is.SameAs(bar));
```

There is no way that the same instance can be of type Foo and type Bar, therefore such assertion will always fail.

## How to fix violations

Fix your assertion (i.e. fix actual or expected value, or choose another constraint)

<!-- start generated config severity -->
## Configure severity

### Via ruleset file.

Configure the severity per project, for more info see [MSDN](https://msdn.microsoft.com/en-us/library/dd264949.aspx).

### Via #pragma directive.
```C#
#pragma warning disable NUnit2020 // Incompatible types for SameAs constraint.
Code violating the rule here
#pragma warning restore NUnit2020 // Incompatible types for SameAs constraint.
```

Or put this at the top of the file to disable all instances.
```C#
#pragma warning disable NUnit2020 // Incompatible types for SameAs constraint.
```

### Via attribute `[SuppressMessage]`.

```C#
[System.Diagnostics.CodeAnalysis.SuppressMessage("Assertion", 
    "NUnit2020:Incompatible types for SameAs constraint.",
    Justification = "Reason...")]
```
<!-- end generated config severity -->
