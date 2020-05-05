**AttributeConstraint** tests for the existence of an attribute on a Type and then applies a constraint to that attribute.

#### Constructor

```csharp
AttributeConstraint(Type type, IConstraint baseConstraint)
```

#### Syntax

```csharp
Has.Attribute(typeof(TestFixtureAttribute))...
Has.Attribute<TestFixtureAttribute>()...
```

#### Examples of Use


```csharp
Assert.That(someObject, Has.Attribute(typeof(TestFixtureAttribute))
    .Property("Description").EqualTo("My description"));
Assert.That(someObject, Has.Attribute<TestFixtureAttribute>()
    .Property("Description").EqualTo("My description"));
```

#### See also...
 * [[AttributeExistsConstraint]]
