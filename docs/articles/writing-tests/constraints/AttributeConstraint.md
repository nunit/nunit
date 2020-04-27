**AttributeConstraint** tests for the existence of an attribute on a Type and then applies a constraint to that attribute.

#### Constructor

```C#
AttributeConstraint(Type type, IConstraint baseConstraint)
```

#### Syntax

```C#
Has.Attribute(typeof(TestFixtureAttribute))...
Has.Attribute<TestFixtureAttribute>()...
```

#### Examples of Use


```C#
Assert.That(someObject, Has.Attribute(typeof(TestFixtureAttribute))
    .Property("Description").EqualTo("My description"));
Assert.That(someObject, Has.Attribute<TestFixtureAttribute>()
    .Property("Description").EqualTo("My description"));
```

#### See also...
 * [[AttributeExistsConstraint]]
