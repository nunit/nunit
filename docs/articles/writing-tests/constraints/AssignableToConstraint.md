**AssignableToConstraint** tests that one type is assignable to another

#### Constructor

```csharp
AssignableToConstraint(Type)
```

#### Syntax

```csharp
Is.AssignableTo(Type)
Is.AssignableTo<T>()
```

#### Examples of Use

```csharp
Assert.That("Hello", Is.AssignableTo(typeof(object)));
Assert.That(5, Is.Not.AssignableTo(typeof(string)));
```