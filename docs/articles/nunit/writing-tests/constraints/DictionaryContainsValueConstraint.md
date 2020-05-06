**DictionaryContainsValueConstraint** is used to test whether a dictionary
contains an expected object as a value.

#### Constructor

```csharp
DictionaryContainsValueConstraint(object)
```

#### Syntax

```csharp
Contains.Value(object)
Does.ContainValue(object)
Does.Not.ContainValue(object)
```

#### Modifiers

```csharp
...Using(IComparer comparer)
...Using(IEqualityComparer comparer)
...Using<T>(IComparer<T> comparer)
...Using<T>(Comparison<T> comparer)
...Using<T>(Func<T, T, bool> comparer)
...Using<T>(IEqualityComparer<T> comparer)
...Using<TCollectionType, TMemberType>(Func<TCollectionType, TMemberType, bool> comparison)
```

#### Examples of Use

```csharp
IDictionary<int, int> idict = new IDictionary<int, int> { { 1, 4 }, { 2, 5 } };

Assert.That(idict, Contains.Value(4));
Assert.That(idict, Does.ContainValue(5));
Assert.That(idict, Does.Not.ContainValue(3));
Assert.That(mydict, Contains.Value(myOwnObject).Using(myComparer));
```

**See also:**
 * [DictionaryContainsKeyConstraint](DictionaryContainsKeyConstraint.md)
