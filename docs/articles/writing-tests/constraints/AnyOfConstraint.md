**AnyOfConstraint** is used to determine whether a value is equal to any of the expected values.

#### Constructor

```csharp
AnyOfConstraint(object[] expected)
```

#### Syntax

```csharp
Is.AnyOf(object[] expected)
```

#### Modifiers

```csharp
...Using(IComparer comparer)
...Using<T>(IEqualityComparer comparer)
...Using<T>(Func<T, T, bool>)
...Using<T>(IComparer<T> comparer)
...Using<T>(Comparison<T> comparer)
...Using<T>(IEqualityComparer<T> comparer)
```

#### Examples of Use

```csharp
int[] iarray = new int[] { 0, -1, 42, 100 }

Assert.That(42, Is.AnyOf(iarray));
Assert.That(myOwnObject, Is.AnyOf(myArray).Using(myComparer));
``` 
