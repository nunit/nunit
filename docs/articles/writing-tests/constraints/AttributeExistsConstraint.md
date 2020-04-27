**AttributeExistsConstraint** tests for the existence of an attribute on a Type.

#### Constructor

```C#
AttributeExistsConstraint(Type type)
```

#### Syntax

```C#
Has.Attribute(typeof(TestFixtureAttribute))
Has.Attribute<TestFixtureAttribute>()
```


#### Examples of Use


```C#
Assert.That(someObject, Has.Attribute(typeof(TestFixtureAttribute)));
Assert.That(someObject, Has.Attribute<TestFixtureAttribute>());
```