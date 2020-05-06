**DictionaryContainsKeyConstraint** is used to test whether a dictionary
contains an expected object as a key.

#### Constructor

```csharp
DictionaryContainsKeyConstraint(object)
```

#### Syntax

```csharp
Contains.Key(object)
Does.ContainKey(object)
Does.Not.ContainKey(object)
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
IDictionary<int, int> idict = new IDictionary<int, string> { { 1, 4 }, { 2, 5 } };

Assert.That(idict, Contains.Key(1));
Assert.That(idict, Does.ContainKey(2));
Assert.That(idict, Does.Not.ContainKey(3));
Assert.That(mydict, Contains.Key(myOwnObject).Using(myComparer));
```

**See also:**
 * [DictionaryContainsValueConstraint](DictionaryContainsValueConstraint.md)
