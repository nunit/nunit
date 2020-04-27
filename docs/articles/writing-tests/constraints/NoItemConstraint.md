**NoItemConstraint** applies a constraint to each item in a collection, succeeding only if all of them fail. An exception is thrown if the actual value passed does not implement `IEnumerable`.

#### Constructor

```csharp
NoItemConstraint(Constraint itemConstraint)
```

#### Syntax

```csharp
Has.None...
```

#### Examples of Use

```csharp
int[] iarray = new int[] { 1, 2, 3 };
string[] sarray = new string[] { "a", "b", "c" };
Assert.That(iarray, Has.None.Null);
Assert.That(sarray, Has.None.EqualTo("d"));
Assert.That(iarray, Has.None.LessThan(0));
```

