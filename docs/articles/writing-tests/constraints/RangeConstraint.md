**RangeConstraint** tests that a value is in an (inclusive) range.

#### Constructor

```C#
RangeConstraint(IComparable from, IComparable to)
```

#### Syntax

```C#
Is.InRange(IComparable from, IComparable to)
```

#### Modifiers

```C#
...Using(IComparer comparer)
...Using<T>(IComparer<T> comparer)
...Using<T>(Comparison<T> comparer)
```

#### Examples of Use

```C#
int[] iarray = new int[] { 1, 2, 3 }

Assert.That(42, Is.InRange(1, 100));
Assert.That(iarray, Is.All.InRange(1, 3));
Assert.That(myOwnObject, 
    Is.InRange(lowExpected, highExpected).Using(myComparer));
```
