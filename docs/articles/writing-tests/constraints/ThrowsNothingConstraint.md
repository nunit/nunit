**ThrowsNothingConstraint** asserts that the delegate passed as its argument does not throw an exception.

#### Constructor

```C#
ThrowsNothingConstraint()
```

#### Syntax

```C#
Throws.Nothing
```

#### Example of Use

```C#
Assert.That(() => SomeMethod(actual), Throws.Nothing);
```