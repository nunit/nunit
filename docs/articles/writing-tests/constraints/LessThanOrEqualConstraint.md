**LessThanOrEqualConstraint** tests that one value is less than or equal to another.

#### Constructor

```C#
LessThanOrEqualConstraint(object expected)
```

#### Syntax

```C#
Is.LessThanOrEqualTo(object expected)
Is.AtMost(object expected)
```

#### Modifiers

```C#
...Using(IComparer comparer)
...Using<T>(IComparer<T> comparer)
...Using<T>(Comparison<T> comparer)
```

#### Examples of Use

```C#
Assert.That(3, Is.LessThanOrEqualTo(7));
Assert.That(3, Is.AtMost(7));
Assert.That(3, Is.LessThanOrEqualTo(3));
Assert.That(3, Is.AtMost(3));
Assert.That(myOwnObject, 
    Is.LessThanOrEqualTo(theExpected).Using(myComparer));
```

