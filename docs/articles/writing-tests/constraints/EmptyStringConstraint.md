The **EmptyStringConstraint** tests if a string is empty.

#### Constructor

```csharp
EmptyStringConstraint()
```
 
#### Syntax

```csharp
Is.Empty
```

#### Examples of Use

```csharp
Assert.That(string.Empty, Is.Empty);
Assert.That("A String", Is.Not.Empty);
```

**Note:** `Is.Empty` actually creates an `EmptyConstraint`. Subsequently applying it to a `string` causes an `EmptyStringConstraint` to be created.
