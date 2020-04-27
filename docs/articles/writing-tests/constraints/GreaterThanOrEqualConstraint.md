**GreaterThanOrEqualConstraint** tests that one value is greater than or equal to another.

#### Constructor

```csharp
GreaterThanOrEqualConstraint(object expected)
```

#### Syntax

```csharp
Is.GreaterThanOrEqualTo(object expected)
Is.AtLeast(object expected)
```

#### Modifiers

```csharp
...Using(IComparer comparer)
...Using<T>(IComparer<T> comparer)
...Using<T>(Comparison<T> comparer)
```

#### Examples of Use

```csharp
Assert.That(7, Is.GreaterThanOrEqualTo(3));
Assert.That(7, Is.AtLeast(3));
Assert.That(7, Is.GreaterThanOrEqualTo(7));
Assert.That(7, Is.AtLeast(7));
Assert.That(myOwnObject, 
    Is.GreaterThanOrEqualTo(theExpected).Using(myComparer));
```

