**ExactTypeConstraint** tests that an object is an exact Type.

#### Constructor

```csharp
ExactTypeConstraint(Type)
```

#### Syntax

```csharp
Is.TypeOf(Type)
Is.TypeOf<T>()
```

#### Examples of Use

```csharp
Assert.That("Hello", Is.TypeOf(typeof(string)));
Assert.That("Hello", Is.Not.TypeOf(typeof(int)));

Assert.That("World", Is.TypeOf<string>());
```

