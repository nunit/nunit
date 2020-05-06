The `PropertyExistsConstraint` tests for the existence of a named property on an object.

#### Constructor

```csharp
PropertyExistsConstraint(string name)
```

#### Syntax

```csharp
Has.Property(string)
```

#### Examples of Use

```csharp
Assert.That(someObject, Has.Property("Version"));
```

#### See also...
 * [PropertyConstraint](PropertyConstraint.md)

