**RangeConstraint** tests that a value is in an (inclusive) range.

#### Constructor

```csharp
RangeConstraint(IComparable from, IComparable to)
```

#### Syntax

```csharp
Is.InRange(IComparable from, IComparable to)
```

#### Modifiers

```csharp
...Using(IComparer comparer)
...Using<T>(IComparer<T> comparer)
...Using<T>(Comparison<T> comparer)
```

#### Examples of Use

```csharp
int[] iarray = new int[] { 1, 2, 3 }

Assert.That(42, Is.InRange(1, 100));
Assert.That(iarray, Is.All.InRange(1, 3));
Assert.That(myOwnObject, 
    Is.InRange(lowExpected, highExpected).Using(myComparer));
```
