**CollectionSubsetConstraint** tests that one `IEnumerable` is a subset of another. If the actual value passed does not implement `IEnumerable`, an exception is thrown.

#### Constructor

```C#
CollectionSubsetConstraint(IEnumerable)
```

#### Syntax

```C#
Is.SubsetOf(IEnumerable)
```

#### Example of Use

```C#
int[] iarray = new int[] { 1, 3 };
Assert.That(iarray, Is.SubsetOf(new int[] { 1, 2, 3 }));
```

