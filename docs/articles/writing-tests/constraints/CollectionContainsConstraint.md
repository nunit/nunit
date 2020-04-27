**CollectionContainsConstraint** tests that an `IEnumerable` contains an object. If the actual value passed does not implement `IEnumerable`, an exception is thrown.

#### Constructor

```csharp
CollectionContainsConstraint(object)
```

#### Syntax

```csharp
Has.Member(object)
Contains.Item(object)
Does.Contain(object)
```

#### Modifiers

```csharp
...Using(IComparer comparer)
...Using<T>(IComparer<T> comparer)
...Using<T>(Comparison<T> comparer)
```

#### Examples of Use

```csharp
int[] iarray = new int[] { 1, 2, 3 };
string[] sarray = new string[] { "a", "b", "c" };
Assert.That(iarray, Has.Member(3));
Assert.That(sarray, Has.Member("b"));
Assert.That(sarray, Contains.Item("c"));
Assert.That(sarray, Has.No.Member("x"));
Assert.That(iarray, Does.Contain(3));
```

#### Note

`Has.Member()`, `Contains.Item()` and `Does.Contain()` work the same as `Has.Some.EqualTo()`. The last statement generates a [[SomeItemsConstraint]] based on an [[EqualConstraint]] and offers additional options such as ignoring case or specifying a tolerance. The syntax on this page may be viewed as a shortcut for specifying simpler cases.
