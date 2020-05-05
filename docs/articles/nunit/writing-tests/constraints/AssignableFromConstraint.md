**AssignableFromConstraint** tests that one type is assignable from another

#### Constructor

```csharp
AssignableFromConstraint(Type)
```

#### Syntax

```csharp
Is.AssignableFrom(Type)
Is.AssignableFrom<T>()
```

#### Examples of Use

```csharp
Assert.That("Hello", Is.AssignableFrom(typeof(string)));
Assert.That(5, Is.Not.AssignableFrom(typeof(string)));
```

