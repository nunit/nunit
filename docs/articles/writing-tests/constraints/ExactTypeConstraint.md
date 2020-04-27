**ExactTypeConstraint** tests that an object is an exact Type.

#### Constructor

```C#
ExactTypeConstraint(Type)
```

#### Syntax

```C#
Is.TypeOf(Type)
Is.TypeOf<T>()
```

#### Examples of Use

```C#
Assert.That("Hello", Is.TypeOf(typeof(string)));
Assert.That("Hello", Is.Not.TypeOf(typeof(int)));

Assert.That("World", Is.TypeOf<string>());
```

