**AssignableFromConstraint** tests that one type is assignable from another

#### Constructor

```C#
AssignableFromConstraint(Type)
```

#### Syntax

```C#
Is.AssignableFrom(Type)
Is.AssignableFrom<T>()
```

#### Examples of Use

```C#
Assert.That("Hello", Is.AssignableFrom(typeof(string)));
Assert.That(5, Is.Not.AssignableFrom(typeof(string)));
```

