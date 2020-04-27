**ThrowsNothingConstraint** asserts that the delegate passed as its argument does not throw an exception.

#### Constructor

```csharp
ThrowsNothingConstraint()
```

#### Syntax

```csharp
Throws.Nothing
```

#### Example of Use

```csharp
Assert.That(() => SomeMethod(actual), Throws.Nothing);
```
