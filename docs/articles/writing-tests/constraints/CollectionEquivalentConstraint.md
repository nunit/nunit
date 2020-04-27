**CollectionEquivalentConstraint** tests that two `IEnumerables` are equivalent - that they contain
the same items, in any order. If the actual value passed does not implement `IEnumerable` an exception is thrown.

#### Constructor

```csharp
CollectionEquivalentConstraint(IEnumerable other)
```

#### Syntax

```csharp
Is.EquivalentTo(IEnumerable other)
```

#### Examples of Use

```csharp
int[] iarray = new int[] { 1, 2, 3 };
string[] sarray = new string[] { "a", "b", "c" };
Assert.That(new string[] { "c", "a", "b" }, Is.EquivalentTo(sarray));
Assert.That(new int[] { 1, 2, 2 }, Is.Not.EquivalentTo(iarray));
```

#### Notes

1. To compare items in order, use Is.EqualTo().

