The **EmptyDirectoryConstraint** tests if a Directory is empty.

#### Constructor

```C#
EmptyDirectoryConstraint()
```

#### Syntax

```C#
Is.Empty
```

#### Examples of Use

```C#
Assert.That(new DirectoryInfo(actual), Is.Empty);
Assert.That(new DirectoryInfo(actual), Is.Not.Empty);
```

**Note:** `Is.Empty` actually creates an `EmptyConstraint`. Subsequently applying it to a `DirectoryInfo` causes an `EmptyDirectoryConstraint` to be created.