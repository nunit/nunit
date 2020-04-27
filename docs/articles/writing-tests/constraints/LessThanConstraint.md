**LessThanConstraint** tests that one value is less than another.

#### Constructor

```C#
LessThanConstraint(object expected)
```

#### Syntax

```C#
Is.LessThan(object expected)
Is.Negative // Equivalent to Is.LessThan(0)
```

#### Modifiers

```C#
...Using(IComparer comparer)
...Using<T>(IComparer<T> comparer)
...Using<T>(Comparison<T> comparer)
```

#### Examples of Use

```C#
Assert.That(3, Is.LessThan(7));
Assert.That(myOwnObject, 
    Is.LessThan(theExpected).Using(myComparer));
Assert.That(-5, Is.Negative);
```

