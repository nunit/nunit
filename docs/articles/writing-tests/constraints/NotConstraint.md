**NotConstraint** reverses the effect of another constraint. If the base constraint fails, NotConstraint succeeds. If the base constraint succeeds, NotConstraint fails.

#### Constructor

```C#
NotConstraint()
```

#### Syntax

```C#
Is.Not...
```

#### Examples of Use

```C#
Assert.That(collection, Is.Not.Unique);
Assert.That(2 + 2, Is.Not.EqualTo(5));
```
