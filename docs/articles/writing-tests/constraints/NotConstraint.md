**NotConstraint** reverses the effect of another constraint. If the base constraint fails, NotConstraint succeeds. If the base constraint succeeds, NotConstraint fails.

#### Constructor

```csharp
NotConstraint()
```

#### Syntax

```csharp
Is.Not...
```

#### Examples of Use

```csharp
Assert.That(collection, Is.Not.Unique);
Assert.That(2 + 2, Is.Not.EqualTo(5));
```
