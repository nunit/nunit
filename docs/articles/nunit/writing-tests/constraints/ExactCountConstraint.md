**ExactCountConstraint** has two functions. 

In its simplest use, it simply verifies the number of items in an array, collection or `IEnumerable`, providing a way to count items that is independent of any `Length` or `Count` property.

When used with another constraint, it applies that constraint to each item in the array, collection or  `IEnumerable`, succeeding if the specified number of items succeed. 

An exception is thrown if the actual value passed does not implement `IEnumerable`.

#### Constructor

```csharp
ExactCountConstraint(int expectedCount)
ExactCountConstraint(int expectedCount, Constraint itemConstraint)
```

#### Syntax

```csharp
Has.Exactly(int expectedCount)...
```

#### Examples of Use

```csharp
int[] array = new int[] { 1, 2, 3 };

Assert.That(array, Has.Exactly(3).Items);
Assert.That(array, Has.Exactly(2).Items.GreaterThan(1));
Assert.That(array, Has.Exactly(3).LessThan(100));
Assert.That(array, Has.Exactly(2).Items.EqualTo(1).Or.EqualTo(3));
Assert.That(array, Has.Exactly(1).EqualTo(1).And.Exactly(1).EqualTo(3));
```

> [!NOTE]
> The keyword `Items` is optional when used before a constraint but required when  merely counting items with no constraint specified.

#### See also...
 * [[PropertyConstraint]] - For constraints on the `Count` or `Length` property, e.g. `Has.Count.GreaterThan(10)` or `Has.Length.EqualTo(6)`.
