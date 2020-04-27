**CollectionSupersetConstraint** tests that one `IEnumerable` is a superset of another. If the actual value passed does not implement `IEnumerable`, an exception is thrown.

#### Constructor

```C#
CollectionSupersetConstraint(IEnumerable)
```

#### Syntax

```C#
Is.SupersetOf(IEnumerable)
```

#### Example of Use

```C#
int[] iarray = new int[] { 1, 2, 3 };
Assert.That(iarray, Is.SupersetOf(new int[] { 1, 3 }));
```

