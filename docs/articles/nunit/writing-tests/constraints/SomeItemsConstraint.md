**SomeItemsConstraint** applies a constraint to each item in an `IEnumerable`, succeeding if at least one of them succeeds. An exception is thrown if the actual value passed does not implement `IEnumerable`.

#### Constructor

```csharp
SomeItemsConstraint(Constraint itemConstraint)
```

#### Syntax

```csharp
Has.Some...
```

#### Examples of Use

```csharp
int[] iarray = new int[] { 1, 2, 3 };
string[] sarray = new string[] { "a", "b", "c" };
Assert.That(iarray, Has.Some.GreaterThan(2));
Assert.That(sarray, Has.Some.Length(1));
```

