The `PropertyExistsConstraint` tests for the existence of a named property on an object.

#### Constructor

```C#
PropertyExistsConstraint(string name)
```

#### Syntax

```C#
Has.Property(string)
```

#### Examples of Use

```C#
Assert.That(someObject, Has.Property("Version"));
```

#### See also...
 * [[PropertyConstraint]]

