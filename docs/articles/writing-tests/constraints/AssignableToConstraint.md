**AssignableToConstraint** tests that one type is assignable to another

#### Constructor

```C#
AssignableToConstraint(Type)
```

#### Syntax

```C#
Is.AssignableTo(Type)
Is.AssignableTo<T>()
```

#### Examples of Use

```C#
Assert.That("Hello", Is.AssignableTo(typeof(object)));
Assert.That(5, Is.Not.AssignableTo(typeof(string)));
```