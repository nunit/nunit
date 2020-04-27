**AttributeExistsConstraint** tests for the existence of an attribute on a Type.

#### Constructor

```csharp
AttributeExistsConstraint(Type type)
```

#### Syntax

```csharp
Has.Attribute(typeof(TestFixtureAttribute))
Has.Attribute<TestFixtureAttribute>()
```


#### Examples of Use


```csharp
Assert.That(someObject, Has.Attribute(typeof(TestFixtureAttribute)));
Assert.That(someObject, Has.Attribute<TestFixtureAttribute>());
```