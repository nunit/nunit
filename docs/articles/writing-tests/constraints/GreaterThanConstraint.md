**GreaterThanConstraint** tests that one value is greater than another.

#### Constructor

```C#
GreaterThanConstraint(object expected)
```

#### Syntax

```C#
Is.GreaterThan(object expected)
Is.Positive // Equivalent to Is.GreaterThan(0)
```

#### Modifiers

```C#
...Using(IComparer comparer)
...Using<T>(IComparer<T> comparer)
...Using<T>(Comparison<T> comparer)
```

#### Examples of Use

```C#
Assert.That(7, Is.GreaterThan(3));
Assert.That(myOwnObject, 
    Is.GreaterThan(theExpected).Using(myComparer));
Assert.That(42, Is.Positive);
```

