**GreaterThanOrEqualConstraint** tests that one value is greater than or equal to another.

#### Constructor

```C#
GreaterThanOrEqualConstraint(object expected)
```

#### Syntax

```C#
Is.GreaterThanOrEqualTo(object expected)
Is.AtLeast(object expected)
```

#### Modifiers

```C#
...Using(IComparer comparer)
...Using<T>(IComparer<T> comparer)
...Using<T>(Comparison<T> comparer)
```

#### Examples of Use

```C#
Assert.That(7, Is.GreaterThanOrEqualTo(3));
Assert.That(7, Is.AtLeast(3));
Assert.That(7, Is.GreaterThanOrEqualTo(7));
Assert.That(7, Is.AtLeast(7));
Assert.That(myOwnObject, 
    Is.GreaterThanOrEqualTo(theExpected).Using(myComparer));
```

