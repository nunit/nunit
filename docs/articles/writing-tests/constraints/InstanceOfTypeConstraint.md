**InstanceOfTypeConstraint** tests that an object is of the type supplied or a derived type.

#### Constructor

```C#
InstanceOfTypeConstraint(Type)
```

#### Syntax

```C#
Is.InstanceOf(Type)
Is.InstanceOf<T>()
```

#### Examples of Use

```C#
Assert.That("Hello", Is.InstanceOf(typeof(string)));
Assert.That(5, Is.Not.InstanceOf(typeof(string)));

Assert.That(5, Is.Not.InstanceOf<string>());
```

