**CollectionSupersetConstraint** tests that one `IEnumerable` is a superset of another. If the actual value passed does not implement `IEnumerable`, an exception is thrown.

#### Constructor

```csharp
CollectionSupersetConstraint(IEnumerable)
```

#### Syntax

```csharp
Is.SupersetOf(IEnumerable)
```

#### Example of Use

```csharp
int[] iarray = new int[] { 1, 2, 3 };
Assert.That(iarray, Is.SupersetOf(new int[] { 1, 3 }));
```

